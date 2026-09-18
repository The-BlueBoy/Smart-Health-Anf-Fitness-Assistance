using System;
using System.Collections.Generic;

namespace SmartHealthFitnessAssistant.Models
{
    // ---------- Enumerations used by the profile ----------
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public enum ActivityLevel
    {
        Sedentary,      // little or no exercise
        Light,          // 1-3 days a week
        Moderate,       // 3-5 days a week
        Active,         // 6-7 days a week
        VeryActive      // physical job + daily training
    }

    public enum FitnessGoal
    {
        LoseBodyFat,
        BuildStrength,
        IncreaseFlexibility,
        GeneralWellness
    }

    public enum DietPreference
    {
        Vegetarian,
        Vegan,
        Eggetarian,
        NonVegetarian
    }

    /// <summary>
    /// Represents one user of the Smart Health and Fitness Assistant.
    /// Demonstrates: CLASSES, PROPERTIES (with backing fields) and VALIDATION.
    /// </summary>
    public class UserProfile
    {
        // ---------- Private backing fields ----------
        private string _fullName;
        private int _age;
        private double _heightCm;
        private double _weightKg;
        private double _sleepHours;
        private int _workoutDaysPerWeek;

        // ---------- Constants used for validation ----------
        public const int MinAge = 10;
        public const int MaxAge = 100;
        public const double MinHeightCm = 100;
        public const double MaxHeightCm = 250;
        public const double MinWeightKg = 25;
        public const double MaxWeightKg = 250;

        // ---------- Constructors ----------
        public UserProfile()
        {
            // Sensible defaults so the dashboard never renders empty
            _fullName = "Guest";
            _age = 21;
            _heightCm = 170;
            _weightKg = 65;
            _sleepHours = 7;
            _workoutDaysPerWeek = 3;

            Gender = Gender.Male;
            ActivityLevel = ActivityLevel.Moderate;
            Goal = FitnessGoal.GeneralWellness;
            Diet = DietPreference.Vegetarian;
            Notes = string.Empty;
        }

        public UserProfile(string fullName, int age, double heightCm, double weightKg)
            : this()
        {
            FullName = fullName;
            Age = age;
            HeightCm = heightCm;
            WeightKg = weightKg;
        }

        // ---------- Properties with validation inside the setters ----------

        /// <summary>Full name of the user. Letters and spaces only.</summary>
        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name is required.");

                string trimmed = value.Trim();

                if (trimmed.Length < 2 || trimmed.Length > 50)
                    throw new ArgumentException("Name must be between 2 and 50 characters.");

                foreach (char c in trimmed)
                {
                    if (!char.IsLetter(c) && c != ' ' && c != '.' && c != '\'')
                        throw new ArgumentException("Name may only contain letters and spaces.");
                }

                _fullName = trimmed;
            }
        }

        /// <summary>Age in completed years.</summary>
        public int Age
        {
            get { return _age; }
            set
            {
                if (value < MinAge || value > MaxAge)
                    throw new ArgumentOutOfRangeException(
                        "Age",
                        string.Format("Age must be between {0} and {1} years.", MinAge, MaxAge));

                _age = value;
            }
        }

        /// <summary>Height in centimetres.</summary>
        public double HeightCm
        {
            get { return _heightCm; }
            set
            {
                if (value < MinHeightCm || value > MaxHeightCm)
                    throw new ArgumentOutOfRangeException(
                        "HeightCm",
                        string.Format("Height must be between {0} and {1} cm.", MinHeightCm, MaxHeightCm));

                _heightCm = value;
            }
        }

        /// <summary>Body weight in kilograms.</summary>
        public double WeightKg
        {
            get { return _weightKg; }
            set
            {
                if (value < MinWeightKg || value > MaxWeightKg)
                    throw new ArgumentOutOfRangeException(
                        "WeightKg",
                        string.Format("Weight must be between {0} and {1} kg.", MinWeightKg, MaxWeightKg));

                _weightKg = value;
            }
        }

        /// <summary>Average hours of sleep per night.</summary>
        public double SleepHours
        {
            get { return _sleepHours; }
            set
            {
                if (value < 3 || value > 14)
                    throw new ArgumentOutOfRangeException("SleepHours", "Sleep must be between 3 and 14 hours.");

                _sleepHours = value;
            }
        }

        /// <summary>How many days per week the user plans to train.</summary>
        public int WorkoutDaysPerWeek
        {
            get { return _workoutDaysPerWeek; }
            set
            {
                if (value < 0 || value > 7)
                    throw new ArgumentOutOfRangeException("WorkoutDaysPerWeek", "Workout days must be between 0 and 7.");

                _workoutDaysPerWeek = value;
            }
        }

        // ---------- Auto-implemented properties ----------
        public int ProfileId { get; set; }
        public Gender Gender { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public FitnessGoal Goal { get; set; }
        public DietPreference Diet { get; set; }

        /// <summary>Optional free text, e.g. "knee pain", "office job", "no gym access".</summary>
        public string Notes { get; set; }

        // ---------- Read-only (computed) properties ----------

        /// <summary>Height expressed in metres, handy for the BMI formula.</summary>
        public double HeightMeters
        {
            get { return HeightCm / 100.0; }
        }

        /// <summary>Friendly label used in the dashboard heading.</summary>
        public string GoalDisplayName
        {
            get
            {
                switch (Goal)
                {
                    case FitnessGoal.LoseBodyFat: return "Lose Body Fat";
                    case FitnessGoal.BuildStrength: return "Build Strength";
                    case FitnessGoal.IncreaseFlexibility: return "Increase Flexibility";
                    default: return "General Wellness";
                }
            }
        }

        // ---------- Methods ----------

        /// <summary>
        /// Collects every validation problem instead of throwing,
        /// so the Web Form can show all messages at once.
        /// </summary>
        public List<string> Validate()
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(_fullName))
                errors.Add("Name is required.");

            if (_age < MinAge || _age > MaxAge)
                errors.Add(string.Format("Age must be between {0} and {1}.", MinAge, MaxAge));

            if (_heightCm < MinHeightCm || _heightCm > MaxHeightCm)
                errors.Add(string.Format("Height must be between {0} and {1} cm.", MinHeightCm, MaxHeightCm));

            if (_weightKg < MinWeightKg || _weightKg > MaxWeightKg)
                errors.Add(string.Format("Weight must be between {0} and {1} kg.", MinWeightKg, MaxWeightKg));

            if (_sleepHours < 3 || _sleepHours > 14)
                errors.Add("Sleep must be between 3 and 14 hours.");

            if (_workoutDaysPerWeek < 0 || _workoutDaysPerWeek > 7)
                errors.Add("Workout days must be between 0 and 7.");

            if (!string.IsNullOrEmpty(Notes) && Notes.Length > 300)
                errors.Add("Notes cannot be longer than 300 characters.");

            return errors;
        }

        /// <summary>True when the profile contains no validation errors.</summary>
        public bool IsValid()
        {
            return Validate().Count == 0;
        }

        /// <summary>Compact one-line summary that is sent to the AI model.</summary>
        public override string ToString()
        {
            return string.Format(
                "{0}, {1} years, {2}, {3} cm, {4} kg, activity: {5}, goal: {6}, diet: {7}, sleep: {8} h, trains {9} day(s)/week",
                FullName, Age, Gender, HeightCm, WeightKg, ActivityLevel,
                GoalDisplayName, Diet, SleepHours, WorkoutDaysPerWeek);
        }
    }
}
