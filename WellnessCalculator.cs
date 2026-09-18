using System;
using System.Collections.Generic;
using SmartHealthFitnessAssistant.Models;

namespace SmartHealthFitnessAssistant.Services
{
    /// <summary>
    /// All health maths lives here. Demonstrates: METHODS (static, overloaded,
    /// private helpers) kept separate from the data classes.
    ///
    /// NOTE: every formula below is a widely used general-population estimate.
    /// The output is general wellness information, not medical advice.
    /// </summary>
    public static class WellnessCalculator
    {
        // Lowest calorie figure the app will ever display, so the assistant
        // never suggests an unsafe intake.
        private const double AbsoluteCalorieFloorFemale = 1200;
        private const double AbsoluteCalorieFloorMale = 1500;

        /// <summary>Body Mass Index = weight (kg) / height (m)^2.</summary>
        public static double CalculateBmi(double weightKg, double heightCm)
        {
            if (heightCm <= 0)
                throw new ArgumentException("Height must be greater than zero.", "heightCm");

            double heightM = heightCm / 100.0;
            return Math.Round(weightKg / (heightM * heightM), 1);
        }

        /// <summary>Overloaded version that accepts the whole profile.</summary>
        public static double CalculateBmi(UserProfile profile)
        {
            if (profile == null) throw new ArgumentNullException("profile");
            return CalculateBmi(profile.WeightKg, profile.HeightCm);
        }

        /// <summary>Standard WHO BMI band.</summary>
        public static string GetBmiCategory(double bmi)
        {
            if (bmi < 18.5) return "Below healthy range";
            if (bmi < 25.0) return "Healthy range";
            if (bmi < 30.0) return "Above healthy range";
            return "Well above healthy range";
        }

        /// <summary>Mifflin-St Jeor basal metabolic rate.</summary>
        public static double CalculateBmr(UserProfile profile)
        {
            if (profile == null) throw new ArgumentNullException("profile");

            double bmr = (10 * profile.WeightKg)
                       + (6.25 * profile.HeightCm)
                       - (5 * profile.Age);

            switch (profile.Gender)
            {
                case Gender.Male:
                    bmr += 5;
                    break;
                case Gender.Female:
                    bmr -= 161;
                    break;
                default:
                    bmr -= 78;   // midpoint of the two constants
                    break;
            }

            return Math.Round(bmr);
        }

        /// <summary>Multiplier applied to BMR based on daily activity.</summary>
        public static double GetActivityFactor(ActivityLevel level)
        {
            switch (level)
            {
                case ActivityLevel.Sedentary: return 1.2;
                case ActivityLevel.Light: return 1.375;
                case ActivityLevel.Moderate: return 1.55;
                case ActivityLevel.Active: return 1.725;
                case ActivityLevel.VeryActive: return 1.9;
                default: return 1.2;
            }
        }

        /// <summary>Total daily energy expenditure.</summary>
        public static double CalculateTdee(UserProfile profile)
        {
            return Math.Round(CalculateBmr(profile) * GetActivityFactor(profile.ActivityLevel));
        }

        /// <summary>Daily calorie target adjusted for the selected goal, with a safety floor.</summary>
        public static double CalculateTargetCalories(UserProfile profile, double tdee)
        {
            double target;

            switch (profile.Goal)
            {
                case FitnessGoal.LoseBodyFat:
                    target = tdee - 400;         // gentle, sustainable deficit
                    break;
                case FitnessGoal.BuildStrength:
                    target = tdee + 250;         // small surplus to support training
                    break;
                default:
                    target = tdee;               // maintenance
                    break;
            }

            double floor = (profile.Gender == Gender.Female)
                ? AbsoluteCalorieFloorFemale
                : AbsoluteCalorieFloorMale;

            if (target < floor) target = floor;

            return Math.Round(target);
        }

        /// <summary>Protein target in grams per day (grams per kg of body weight).</summary>
        public static double CalculateProteinGrams(UserProfile profile)
        {
            double perKg;

            switch (profile.Goal)
            {
                case FitnessGoal.BuildStrength: perKg = 1.6; break;
                case FitnessGoal.LoseBodyFat: perKg = 1.4; break;
                default: perKg = 1.1; break;
            }

            return Math.Round(profile.WeightKg * perKg);
        }

        /// <summary>Roughly 35 ml of fluid per kg of body weight.</summary>
        public static double CalculateWaterLitres(double weightKg)
        {
            return Math.Round((weightKg * 35.0) / 1000.0, 1);
        }

        /// <summary>Age predicted maximum heart rate.</summary>
        public static int CalculateMaxHeartRate(int age)
        {
            return 220 - age;
        }

        /// <summary>Healthy weight window for the user's height (BMI 18.5 - 24.9).</summary>
        public static void CalculateHealthyWeightRange(double heightCm, out double lowKg, out double highKg)
        {
            double heightM = heightCm / 100.0;
            lowKg = Math.Round(18.5 * heightM * heightM, 1);
            highKg = Math.Round(24.9 * heightM * heightM, 1);
        }

        /// <summary>
        /// Main entry point: turns a validated profile into a complete report.
        /// </summary>
        public static WellnessReport BuildReport(UserProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException("profile");

            List<string> errors = profile.Validate();
            if (errors.Count > 0)
                throw new InvalidOperationException("Profile is not valid: " + string.Join(" ", errors.ToArray()));

            WellnessReport report = new WellnessReport();

            // --- body composition ---
            report.Bmi = CalculateBmi(profile);
            report.BmiCategory = GetBmiCategory(report.Bmi);
            report.BmiBadgeClass = GetBmiBadgeClass(report.Bmi);

            double low, high;
            CalculateHealthyWeightRange(profile.HeightCm, out low, out high);
            report.HealthyWeightLowKg = low;
            report.HealthyWeightHighKg = high;

            // --- energy ---
            report.BmrCalories = CalculateBmr(profile);
            report.TdeeCalories = CalculateTdee(profile);
            report.TargetCalories = CalculateTargetCalories(profile, report.TdeeCalories);

            // --- macros and hydration ---
            report.ProteinGrams = CalculateProteinGrams(profile);
            report.FatGrams = Math.Round((report.TargetCalories * 0.27) / 9.0);
            double remaining = report.TargetCalories - (report.ProteinGrams * 4) - (report.FatGrams * 9);
            report.CarbGrams = Math.Round(Math.Max(remaining, 0) / 4.0);
            report.WaterLitres = CalculateWaterLitres(profile.WeightKg);

            // --- activity ---
            report.MaxHeartRate = CalculateMaxHeartRate(profile.Age);
            report.TrainingZoneLowBpm = (int)Math.Round(report.MaxHeartRate * 0.60);
            report.TrainingZoneHighBpm = (int)Math.Round(report.MaxHeartRate * 0.80);
            report.RecommendedStepsPerDay = GetStepTarget(profile.ActivityLevel);
            report.WeeklyActiveMinutes = (profile.Goal == FitnessGoal.LoseBodyFat) ? 225 : 150;
            report.SleepTargetHours = (profile.Age <= 25) ? 8.0 : 7.5;

            // --- rule based tips ---
            report.Suggestions = GetGeneralSuggestions(profile, report);

            return report;
        }

        /// <summary>
        /// Simple rule-based suggestions. These are shown even when Ollama is
        /// offline, so the dashboard is never empty.
        /// </summary>
        public static List<string> GetGeneralSuggestions(UserProfile profile, WellnessReport report)
        {
            List<string> tips = new List<string>();

            switch (profile.Goal)
            {
                case FitnessGoal.LoseBodyFat:
                    tips.Add("Aim for a steady pace of change rather than a rapid one; small weekly adjustments are easier to keep up.");
                    tips.Add("Build meals around a protein source, plenty of vegetables and a whole-grain carbohydrate.");
                    break;
                case FitnessGoal.BuildStrength:
                    tips.Add("Train each major muscle group about twice a week and add a little weight or one more repetition over time.");
                    tips.Add("Spread protein across the day instead of eating most of it in a single meal.");
                    break;
                case FitnessGoal.IncreaseFlexibility:
                    tips.Add("Hold each stretch for 30 to 60 seconds and breathe normally instead of bouncing.");
                    tips.Add("Mobility work responds well to short daily sessions rather than one long weekly session.");
                    break;
                default:
                    tips.Add("A mix of walking, two short strength sessions and some stretching covers most general wellness bases.");
                    break;
            }

            if (profile.WorkoutDaysPerWeek == 0)
                tips.Add("Starting with two 20-minute sessions a week is usually more sustainable than jumping straight to daily training.");
            else if (profile.WorkoutDaysPerWeek >= 6)
                tips.Add("With six or more training days, keep at least one genuinely easy day so your body can recover.");

            if (profile.SleepHours < 7)
                tips.Add("Sleep is doing a lot of the recovery work. Moving bedtime earlier by 20 minutes is an easy first step.");

            if (profile.ActivityLevel == ActivityLevel.Sedentary)
                tips.Add("If you sit for long stretches, a two-minute walk every hour adds up quickly over a week.");

            switch (profile.Diet)
            {
                case DietPreference.Vegan:
                    tips.Add("Combine pulses, tofu, soya and grains across the day to cover your protein target on a vegan plan.");
                    break;
                case DietPreference.Vegetarian:
                case DietPreference.Eggetarian:
                    tips.Add("Dal, paneer, curd, soya chunks and sprouts are convenient protein sources on a vegetarian plan.");
                    break;
                case DietPreference.NonVegetarian:
                    tips.Add("Lean chicken, fish and eggs make it straightforward to reach your protein target.");
                    break;
            }

            tips.Add(string.Format("Try to drink around {0} litres of water across the day, more on hot days or heavy training days.", report.WaterLitres));

            return tips;
        }

        // ---------- private helpers ----------

        private static string GetBmiBadgeClass(double bmi)
        {
            if (bmi >= 18.5 && bmi < 25.0) return "badge-good";
            if (bmi < 18.5) return "badge-warn";
            return "badge-warn";
        }

        private static int GetStepTarget(ActivityLevel level)
        {
            switch (level)
            {
                case ActivityLevel.Sedentary: return 6000;
                case ActivityLevel.Light: return 7500;
                case ActivityLevel.Moderate: return 9000;
                case ActivityLevel.Active: return 10000;
                case ActivityLevel.VeryActive: return 12000;
                default: return 8000;
            }
        }
    }
}
