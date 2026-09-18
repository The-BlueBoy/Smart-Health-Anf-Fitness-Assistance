using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using SmartHealthFitnessAssistant.Models;
using SmartHealthFitnessAssistant.Services;

namespace SmartHealthFitnessAssistant
{
    public partial class Default : System.Web.UI.Page
    {
        // Kept between the click handler and the async task.
        private UserProfile _profile;
        private WellnessReport _report;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                litWelcomeName.Text = "Guest";
                HighlightGoalCard(FitnessGoal.GeneralWellness);
                BuildCalorieBars(0);
            }
        }

        // ------------------------------------------------------------------
        // Custom (server side) validation for the optional notes box
        // ------------------------------------------------------------------
        protected void cvNotes_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string value = args.Value == null ? string.Empty : args.Value.Trim();
            args.IsValid = value.Length <= 300;
        }

        // ------------------------------------------------------------------
        // Main button: validate -> build profile -> calculate -> call AI
        // ------------------------------------------------------------------
        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            Page.Validate("wellness");
            if (!Page.IsValid)
                return;

            HideMessages();

            // ---------- 1. Build the object (property setters validate again) ----------
            List<string> errors = new List<string>();
            _profile = BuildProfileFromForm(errors);

            if (_profile == null || errors.Count > 0)
            {
                ShowServerErrors(errors);
                return;
            }

            // ---------- 2. Second line of defence: object level validation ----------
            List<string> modelErrors = _profile.Validate();
            if (modelErrors.Count > 0)
            {
                ShowServerErrors(modelErrors);
                return;
            }

            // ---------- 3. C# calculations ----------
            try
            {
                _report = WellnessCalculator.BuildReport(_profile);
            }
            catch (Exception ex)
            {
                ShowServerErrors(new List<string> { "Could not calculate your report: " + ex.Message });
                return;
            }

            BindDashboard(_profile, _report);

            // ---------- 4. AI feature (runs asynchronously) ----------
            RegisterAsyncTask(new PageAsyncTask(CallOllamaAsync));
        }

        /// <summary>Sends the calculated data to Ollama and shows the reply.</summary>
        private async Task CallOllamaAsync()
        {
            OllamaService ai = new OllamaService();

            // Always show students what was sent.
            string prompt = ai.BuildPrompt(_profile, _report);
            litPrompt.Text = Server.HtmlEncode(prompt);
            pnlPrompt.Visible = true;

            try
            {
                string answer = await ai.GetWellnessSuggestionAsync(_profile, _report);

                if (string.IsNullOrWhiteSpace(answer))
                {
                    ShowAiError("The model returned an empty response. Please try again.");
                    return;
                }

                litAiResponse.Text = FormatAsHtml(answer);
                lblAiStatus.Text = "Ollama - " + ai.ModelName;
                lblAiStatus.CssClass = "chip chip-ok";
            }
            catch (ApplicationException ex)
            {
                ShowAiError(ex.Message);
            }
            catch (Exception ex)
            {
                ShowAiError("Unexpected error while contacting the AI model: " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // Reset
        // ------------------------------------------------------------------
        protected void btnReset_Click(object sender, EventArgs e)
        {
            UserProfile profileToSave = Session["Profile"] as UserProfile;

            if (profileToSave == null)
            {
                Response.Redirect(Request.Url.AbsolutePath);
                return;
            }

            try
            {
                ProfileRepository repository = new ProfileRepository();
                int newId = repository.InsertProfile(profileToSave);
                profileToSave.ProfileId = newId;
            }
            catch (Exception ex)
            {
                ShowServerErrors(new List<string> {
            "Could not save to the database: " + ex.Message
        });
                return;
            }

            Session.Clear();
            Response.Redirect(Request.Url.AbsolutePath);
        }

        // ==================================================================
        //  Helpers
        // ==================================================================

        /// <summary>
        /// Reads every control and fills a <see cref="UserProfile"/>.
        /// Any setter that rejects a value adds a readable message to <paramref name="errors"/>.
        /// </summary>
        private UserProfile BuildProfileFromForm(List<string> errors)
        {
            UserProfile profile = new UserProfile();

            TrySet(errors, "Name", () => profile.FullName = txtName.Text);

            int age;
            if (int.TryParse(txtAge.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out age))
                TrySet(errors, "Age", () => profile.Age = age);
            else
                errors.Add("Age must be a whole number.");

            double height;
            if (double.TryParse(txtHeight.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out height))
                TrySet(errors, "Height", () => profile.HeightCm = height);
            else
                errors.Add("Height must be a number in centimetres.");

            double weight;
            if (double.TryParse(txtWeight.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out weight))
                TrySet(errors, "Weight", () => profile.WeightKg = weight);
            else
                errors.Add("Weight must be a number in kilograms.");

            double sleep;
            if (double.TryParse(txtSleep.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out sleep))
                TrySet(errors, "Sleep", () => profile.SleepHours = sleep);
            else
                errors.Add("Sleep hours must be a number.");

            int days;
            if (int.TryParse(txtWorkoutDays.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out days))
                TrySet(errors, "Training days", () => profile.WorkoutDaysPerWeek = days);
            else
                errors.Add("Training days must be a whole number.");

            profile.Gender = ParseEnum(ddlGender.SelectedValue, Gender.Other);
            profile.ActivityLevel = ParseEnum(ddlActivity.SelectedValue, ActivityLevel.Moderate);
            profile.Goal = ParseEnum(ddlGoal.SelectedValue, FitnessGoal.GeneralWellness);
            profile.Diet = ParseEnum(ddlDiet.SelectedValue, DietPreference.Vegetarian);
            profile.Notes = txtNotes.Text == null ? string.Empty : txtNotes.Text.Trim();

            return profile;
        }

        /// <summary>Runs a property assignment and converts any exception into a friendly message.</summary>
        private static void TrySet(List<string> errors, string fieldName, Action assignment)
        {
            try
            {
                assignment();
            }
            catch (Exception ex)
            {
                errors.Add(fieldName + ": " + ex.Message.Split('\n')[0]);
            }
        }

        private static T ParseEnum<T>(string value, T fallback) where T : struct
        {
            T result;
            return Enum.TryParse(value, true, out result) ? result : fallback;
        }

        /// <summary>Pushes every calculated number onto the dashboard.</summary>
        private void BindDashboard(UserProfile profile, WellnessReport report)
        {
            litWelcomeName.Text = Server.HtmlEncode(profile.FullName.Split(' ')[0]);
            litGoalChip.Text = Server.HtmlEncode(profile.GoalDisplayName);
            HighlightGoalCard(profile.Goal);

            // goal card sub-lines now carry real data
            litStrengthSets.Text = report.ProteinGrams + " g protein / day";
            litFatSets.Text = report.TargetCalories + " kcal / day";
            litFlexSets.Text = report.WeeklyActiveMinutes + " active min / week";

            litActiveMinutes.Text = report.WeeklyActiveMinutes + " min";
            litSteps.Text = report.RecommendedStepsPerDay.ToString("N0");
            litSleepTarget.Text = report.SleepTargetHours + " h";

            // BMI card
            litBmi.Text = report.Bmi.ToString("0.0");
            lblBmiCategory.Text = report.BmiCategory;
            lblBmiCategory.CssClass = "badge " + report.BmiBadgeClass;
            litHealthyRange.Text = report.HealthyWeightLowKg + " - " + report.HealthyWeightHighKg + " kg";

            // Calories card
            litTargetCalories.Text = report.TargetCalories.ToString("N0");
            litTdee.Text = report.TdeeCalories.ToString("N0");
            litProtein.Text = report.ProteinGrams.ToString("N0") + " g";
            litWater.Text = report.WaterLitres + " L";
            BuildCalorieBars(report.CalorieBarPercent);

            // Heart rate card
            litHrLow.Text = report.TrainingZoneLowBpm.ToString();
            litHrRange.Text = report.TrainingZoneLowBpm + " - " + report.TrainingZoneHighBpm + " bpm zone";
            litMaxHr.Text = report.MaxHeartRate + " bpm";

            // Rule based tips
            rptTips.DataSource = report.Suggestions;
            rptTips.DataBind();
            pnlNoTips.Visible = report.Suggestions.Count == 0;
        }

        /// <summary>Builds the 30 little bars used by the calories progress graphic.</summary>
        private void BuildCalorieBars(int percent)
        {
            const int total = 30;
            int filled = (int)Math.Round(total * (percent / 100.0));

            List<CalorieBar> bars = new List<CalorieBar>();
            for (int i = 0; i < total; i++)
                bars.Add(new CalorieBar(i < filled));

            rptBars.DataSource = bars;
            rptBars.DataBind();
        }

        private void HighlightGoalCard(FitnessGoal goal)
        {
            goalStrength.Attributes["class"] = "goal goal-purple" + (goal == FitnessGoal.BuildStrength ? " selected" : "");
            goalFat.Attributes["class"] = "goal goal-yellow" + (goal == FitnessGoal.LoseBodyFat ? " selected" : "");
            goalFlex.Attributes["class"] = "goal goal-dark" + (goal == FitnessGoal.IncreaseFlexibility ? " selected" : "");
        }

        /// <summary>Converts the model's plain text into simple, safe HTML.</summary>
        private string FormatAsHtml(string text)
        {
            string encoded = Server.HtmlEncode(text);

            string[] lines = encoded.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (string raw in lines)
            {
                string line = raw.Trim();

                if (line.Length == 0)
                    continue;

                if (line.StartsWith("-"))
                    sb.Append("<p class=\"ai-bullet\">").Append(line.TrimStart('-').Trim()).Append("</p>");
                else if (line.Length < 45 && !line.EndsWith("."))
                    sb.Append("<h4>").Append(line.TrimEnd(':')).Append("</h4>");
                else
                    sb.Append("<p>").Append(line).Append("</p>");
            }

            return sb.ToString();
        }

        private void HideMessages()
        {
            pnlServerErrors.Visible = false;
            pnlAiError.Visible = false;
        }

        private void ShowServerErrors(List<string> errors)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder("<ul>");
            foreach (string error in errors)
                sb.Append("<li>").Append(Server.HtmlEncode(error)).Append("</li>");
            sb.Append("</ul>");

            litServerErrors.Text = sb.ToString();
            pnlServerErrors.Visible = true;
        }

        private void ShowAiError(string message)
        {
            litAiError.Text = Server.HtmlEncode(message);
            pnlAiError.Visible = true;
            lblAiStatus.Text = "Ollama - unavailable";
            lblAiStatus.CssClass = "chip chip-warn";
            litAiResponse.Text =
                "<p class=\"muted\">The AI suggestions could not be loaded, but your calculated figures " +
                "and the rule-based Quick Tips on the right are still available.</p>";
        }
    }
}
