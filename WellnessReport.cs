using System;
using System.Collections.Generic;

namespace SmartHealthFitnessAssistant.Models
{
    /// <summary>
    /// Holds every value calculated by <see cref="Services.WellnessCalculator"/>.
    /// This is a plain data class made entirely of PROPERTIES.
    /// </summary>
    public class WellnessReport
    {
        public WellnessReport()
        {
            Suggestions = new List<string>();
            GeneratedOn = DateTime.Now;
        }

        // Body composition
        public double Bmi { get; set; }
        public string BmiCategory { get; set; }
        public string BmiBadgeClass { get; set; }   // css class used by the dashboard
        public double HealthyWeightLowKg { get; set; }
        public double HealthyWeightHighKg { get; set; }

        // Energy
        public double BmrCalories { get; set; }     // basal metabolic rate
        public double TdeeCalories { get; set; }    // total daily energy expenditure
        public double TargetCalories { get; set; }  // adjusted for the chosen goal

        // Macros and hydration
        public double ProteinGrams { get; set; }
        public double CarbGrams { get; set; }
        public double FatGrams { get; set; }
        public double WaterLitres { get; set; }

        // Activity
        public int MaxHeartRate { get; set; }
        public int TrainingZoneLowBpm { get; set; }
        public int TrainingZoneHighBpm { get; set; }
        public int RecommendedStepsPerDay { get; set; }
        public int WeeklyActiveMinutes { get; set; }
        public double SleepTargetHours { get; set; }

        // Rule-based (non AI) tips shown next to the AI answer
        public List<string> Suggestions { get; set; }

        public DateTime GeneratedOn { get; set; }

        /// <summary>Percentage of the daily calorie target already planned, used by the progress bar.</summary>
        public int CalorieBarPercent
        {
            get
            {
                if (TdeeCalories <= 0) return 0;
                double percent = (TargetCalories / TdeeCalories) * 100.0;
                if (percent < 0) percent = 0;
                if (percent > 100) percent = 100;
                return (int)Math.Round(percent);
            }
        }
    }
}
