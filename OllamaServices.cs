using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using SmartHealthFitnessAssistant.Models;

namespace SmartHealthFitnessAssistant.Services
{
    /// <summary>
    /// AI FEATURE (Option A - Ollama, local model, no API key required).
    ///
    /// Flow:  User input -> C# calculation (WellnessCalculator)
    ///                   -> prompt built here
    ///                   -> Ollama /api/generate
    ///                   -> suggestion displayed on the Web Form
    /// </summary>
    public class OllamaService
    {
        // One HttpClient for the whole application (recommended practice).
        private static readonly HttpClient Client = CreateClient();

        private readonly string _baseUrl;
        private readonly string _model;

        public OllamaService()
        {
            _baseUrl = ReadSetting("OllamaUrl", "http://localhost:11434");
            _model = ReadSetting("OllamaModel", "llama3.2");
        }

        /// <summary>Model name currently configured, shown on the dashboard.</summary>
        public string ModelName
        {
            get { return _model; }
        }

        private static HttpClient CreateClient()
        {
            HttpClient client = new HttpClient();
            // Local models can take a while on the first run, so allow plenty of time.
            client.Timeout = TimeSpan.FromMinutes(3);
            return client;
        }

        private static string ReadSetting(string key, string fallback)
        {
            string value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        /// <summary>
        /// Turns the profile and the calculated numbers into a clear prompt.
        /// Exposed publicly so the page can show students exactly what was sent.
        /// </summary>
        public string BuildPrompt(UserProfile profile, WellnessReport report)
        {
            if (profile == null) throw new ArgumentNullException("profile");
            if (report == null) throw new ArgumentNullException("report");

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("You are a friendly general wellness assistant inside a college project.");
            sb.AppendLine("Give safe, general, everyday lifestyle suggestions only.");
            sb.AppendLine("Do not diagnose, do not prescribe, and do not suggest extreme diets, fasting or very low calorie intakes.");
            sb.AppendLine("If anything sounds like a medical concern, suggest speaking to a qualified doctor.");
            sb.AppendLine();

            sb.AppendLine("USER PROFILE");
            sb.AppendLine("Name: " + profile.FullName);
            sb.AppendLine("Age: " + profile.Age);
            sb.AppendLine("Gender: " + profile.Gender);
            sb.AppendLine("Height: " + profile.HeightCm + " cm");
            sb.AppendLine("Weight: " + profile.WeightKg + " kg");
            sb.AppendLine("Activity level: " + profile.ActivityLevel);
            sb.AppendLine("Primary goal: " + profile.GoalDisplayName);
            sb.AppendLine("Diet preference: " + profile.Diet);
            sb.AppendLine("Average sleep: " + profile.SleepHours + " hours per night");
            sb.AppendLine("Planned training days: " + profile.WorkoutDaysPerWeek + " per week");

            if (!string.IsNullOrWhiteSpace(profile.Notes))
                sb.AppendLine("Extra notes from the user: " + profile.Notes.Trim());

            sb.AppendLine();
            sb.AppendLine("VALUES ALREADY CALCULATED BY THE C# APPLICATION");
            sb.AppendLine("BMI: " + report.Bmi + " (" + report.BmiCategory + ")");
            sb.AppendLine("BMR: " + report.BmrCalories + " kcal");
            sb.AppendLine("Maintenance calories: " + report.TdeeCalories + " kcal");
            sb.AppendLine("Daily calorie target: " + report.TargetCalories + " kcal");
            sb.AppendLine("Protein target: " + report.ProteinGrams + " g");
            sb.AppendLine("Water target: " + report.WaterLitres + " litres");
            sb.AppendLine("Training heart rate zone: " + report.TrainingZoneLowBpm + " to " + report.TrainingZoneHighBpm + " bpm");
            sb.AppendLine("Daily step target: " + report.RecommendedStepsPerDay);

            sb.AppendLine();
            sb.AppendLine("TASK");
            sb.AppendLine("Write a short, encouraging wellness plan for this person using these headings exactly:");
            sb.AppendLine("Overview");
            sb.AppendLine("Weekly Movement Plan");
            sb.AppendLine("Everyday Nutrition Ideas");
            sb.AppendLine("Sleep and Recovery");
            sb.AppendLine("One Habit To Start This Week");
            sb.AppendLine();
            sb.AppendLine("Rules: use plain text, no markdown symbols such as * or #, keep each section to two or three short sentences, ");
            sb.AppendLine("respect the stated diet preference, and stay under 300 words in total.");

            return sb.ToString();
        }

        /// <summary>
        /// Sends the prompt to Ollama and returns the generated text.
        /// Throws <see cref="ApplicationException"/> with a readable message on failure.
        /// </summary>
        public async Task<string> GetWellnessSuggestionAsync(UserProfile profile, WellnessReport report)
        {
            string prompt = BuildPrompt(profile, report);
            return await SendPromptAsync(prompt);
        }

        /// <summary>Low level call to POST {OllamaUrl}/api/generate.</summary>
        public async Task<string> SendPromptAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be empty.", "prompt");

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            Dictionary<string, object> requestBody = new Dictionary<string, object>();
            requestBody["model"] = _model;
            requestBody["prompt"] = prompt;
            requestBody["stream"] = false;            // we want one complete reply

            Dictionary<string, object> options = new Dictionary<string, object>();
            options["temperature"] = 0.7;
            options["num_predict"] = 500;
            requestBody["options"] = options;

            string json = serializer.Serialize(requestBody);
            string url = _baseUrl.TrimEnd('/') + "/api/generate";

            try
            {
                using (StringContent content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (HttpResponseMessage response = await Client.PostAsync(url, content))
                {
                    string raw = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ApplicationException(
                            "Ollama replied with status " + (int)response.StatusCode +
                            ". Check that the model '" + _model + "' is installed (run: ollama pull " + _model + ").");
                    }

                    Dictionary<string, object> parsed =
                        serializer.Deserialize<Dictionary<string, object>>(raw);

                    if (parsed != null && parsed.ContainsKey("response"))
                    {
                        string text = Convert.ToString(parsed["response"]);
                        return CleanUp(text);
                    }

                    throw new ApplicationException("Ollama returned an unexpected response format.");
                }
            }
            catch (HttpRequestException)
            {
                throw new ApplicationException(
                    "Could not reach Ollama at " + _baseUrl +
                    ". Make sure Ollama is installed and running (open a command prompt and type: ollama serve).");
            }
            catch (TaskCanceledException)
            {
                throw new ApplicationException(
                    "The AI request timed out. The first response from a local model can be slow, please try again.");
            }
        }

        /// <summary>Removes stray markdown characters so the text renders neatly on the page.</summary>
        private static string CleanUp(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            return text.Replace("**", string.Empty)
                       .Replace("##", string.Empty)
                       .Replace("* ", "- ")
                       .Trim();
        }
    }
}
