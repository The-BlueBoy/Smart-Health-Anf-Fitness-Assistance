using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SmartHealthFitnessAssistant.Models;

namespace SmartHealthFitnessAssistant.Services
{
    /// <summary>
    /// Handles saving and retrieving <see cref="UserProfile"/> records
    /// from the WellnessDb database using plain ADO.NET (no ORM),
    /// so the SQL is explicit and easy to explain in a viva.
    /// </summary>
    public class ProfileRepository
    {
        private readonly string _connectionString;

        public ProfileRepository()
        {
            _connectionString = ConfigurationManager
                .ConnectionStrings["WellnessDbConnection"]
                .ConnectionString;
        }

        /// <summary>Inserts a new row and returns the generated ProfileId.</summary>
        public int InsertProfile(UserProfile profile)
        {
            if (profile == null) throw new ArgumentNullException("profile");

            const string sql = @"
                INSERT INTO dbo.UserProfiles
                    (FullName, Age, Gender, HeightCm, WeightKg, ActivityLevel,
                     Goal, Diet, SleepHours, WorkoutDaysPerWeek, Notes)
                VALUES
                    (@FullName, @Age, @Gender, @HeightCm, @WeightKg, @ActivityLevel,
                     @Goal, @Diet, @SleepHours, @WorkoutDaysPerWeek, @Notes);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                AddProfileParameters(command, profile);

                connection.Open();
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        /// <summary>Returns every saved profile, most recent first.</summary>
        public List<UserProfile> GetAllProfiles()
        {
            const string sql = @"
                SELECT ProfileId, FullName, Age, Gender, HeightCm, WeightKg,
                       ActivityLevel, Goal, Diet, SleepHours, WorkoutDaysPerWeek, Notes
                FROM dbo.UserProfiles
                ORDER BY CreatedOn DESC;";

            List<UserProfile> profiles = new List<UserProfile>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        profiles.Add(MapReaderToProfile(reader));
                    }
                }
            }

            return profiles;
        }

        /// <summary>Returns one profile by id, or null if it does not exist.</summary>
        public UserProfile GetProfileById(int profileId)
        {
            const string sql = @"
                SELECT ProfileId, FullName, Age, Gender, HeightCm, WeightKg,
                       ActivityLevel, Goal, Diet, SleepHours, WorkoutDaysPerWeek, Notes
                FROM dbo.UserProfiles
                WHERE ProfileId = @ProfileId;";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@ProfileId", SqlDbType.Int).Value = profileId;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        return MapReaderToProfile(reader);
                }
            }

            return null;
        }

        /// <summary>Deletes one saved profile. Returns true if a row was removed.</summary>
        public bool DeleteProfile(int profileId)
        {
            const string sql = "DELETE FROM dbo.UserProfiles WHERE ProfileId = @ProfileId;";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@ProfileId", SqlDbType.Int).Value = profileId;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ---------- private helpers ----------

        private static void AddProfileParameters(SqlCommand command, UserProfile profile)
        {
            command.Parameters.Add("@FullName", SqlDbType.NVarChar, 50).Value = profile.FullName;
            command.Parameters.Add("@Age", SqlDbType.Int).Value = profile.Age;
            command.Parameters.Add("@Gender", SqlDbType.NVarChar, 10).Value = profile.Gender.ToString();
            command.Parameters.Add("@HeightCm", SqlDbType.Float).Value = profile.HeightCm;
            command.Parameters.Add("@WeightKg", SqlDbType.Float).Value = profile.WeightKg;
            command.Parameters.Add("@ActivityLevel", SqlDbType.NVarChar, 20).Value = profile.ActivityLevel.ToString();
            command.Parameters.Add("@Goal", SqlDbType.NVarChar, 30).Value = profile.Goal.ToString();
            command.Parameters.Add("@Diet", SqlDbType.NVarChar, 20).Value = profile.Diet.ToString();
            command.Parameters.Add("@SleepHours", SqlDbType.Float).Value = profile.SleepHours;
            command.Parameters.Add("@WorkoutDaysPerWeek", SqlDbType.Int).Value = profile.WorkoutDaysPerWeek;

            object notesValue = string.IsNullOrEmpty(profile.Notes) ? (object)DBNull.Value : profile.Notes;
            command.Parameters.Add("@Notes", SqlDbType.NVarChar, 300).Value = notesValue;
        }

        private static UserProfile MapReaderToProfile(SqlDataReader reader)
        {
            UserProfile profile = new UserProfile();

            profile.ProfileId = reader.GetInt32(reader.GetOrdinal("ProfileId"));
            profile.FullName = reader.GetString(reader.GetOrdinal("FullName"));
            profile.Age = reader.GetInt32(reader.GetOrdinal("Age"));
            profile.Gender = (Gender)Enum.Parse(typeof(Gender), reader.GetString(reader.GetOrdinal("Gender")));
            profile.HeightCm = reader.GetDouble(reader.GetOrdinal("HeightCm"));
            profile.WeightKg = reader.GetDouble(reader.GetOrdinal("WeightKg"));
            profile.ActivityLevel = (ActivityLevel)Enum.Parse(typeof(ActivityLevel), reader.GetString(reader.GetOrdinal("ActivityLevel")));
            profile.Goal = (FitnessGoal)Enum.Parse(typeof(FitnessGoal), reader.GetString(reader.GetOrdinal("Goal")));
            profile.Diet = (DietPreference)Enum.Parse(typeof(DietPreference), reader.GetString(reader.GetOrdinal("Diet")));
            profile.SleepHours = reader.GetDouble(reader.GetOrdinal("SleepHours"));
            profile.WorkoutDaysPerWeek = reader.GetInt32(reader.GetOrdinal("WorkoutDaysPerWeek"));

            int notesOrdinal = reader.GetOrdinal("Notes");
            profile.Notes = reader.IsDBNull(notesOrdinal) ? string.Empty : reader.GetString(notesOrdinal);

            return profile;
        }
    }
}
