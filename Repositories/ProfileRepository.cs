using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFOgloszenia.Models;

namespace WPFOgloszenia.Repositories {
    public class ProfileRepository {
        public static async Task CreateIfNotExistsAsync() {
            using SqlConnection connection = new SqlConnection(App.connectionString);
            await connection.OpenAsync();

            string tableExistsQuery = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Profiles'";
            using SqlCommand tableExistsCommand = new(tableExistsQuery, connection);
            object? tableExistsResult = await tableExistsCommand.ExecuteScalarAsync();

            if (tableExistsResult == null) {
                string createTableQuery = @"
                    CREATE TABLE Profiles
                    (
                        ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                        Name NVARCHAR(MAX),
                        Surname NVARCHAR(MAX),
                        Email NVARCHAR(MAX)
                    );";

                using SqlCommand createTableCommand = new(createTableQuery, connection);
                await createTableCommand.ExecuteNonQueryAsync();
                await SeedAsync();
            }
        }
        public static async Task<int> CreateAsync(ProfileModel profile) {
            using SqlConnection connection = new SqlConnection(App.connectionString);
            await connection.OpenAsync();

            string query = @"
                INSERT INTO Profiles (Name, Surname, Email)
                VALUES (@Name, @Surname, @Email);
                SELECT SCOPE_IDENTITY();";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", profile.Name);
            command.Parameters.AddWithValue("@Surname", profile.Surname);
            command.Parameters.AddWithValue("@Email", profile.Email);

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        public static async Task<List<ProfileModel>> GetAllAsync() {
            using SqlConnection connection = new SqlConnection(App.connectionString);
            await connection.OpenAsync();

            string query = "SELECT * FROM Profiles";
            using SqlCommand command = new SqlCommand(query, connection);
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            List<ProfileModel> profiles = new List<ProfileModel>();
            while (await reader.ReadAsync()) {
                ProfileModel profile = new ProfileModel {
                    ID = (int)reader["ID"],
                    Name = reader["Name"].ToString(),
                    Surname = reader["Surname"].ToString(),
                    Email = reader["Email"].ToString()
                };
                profiles.Add(profile);
            }

            return profiles;
        }

        public static async Task<ProfileModel?> GetByIdAsync(int id) {
            using SqlConnection connection = new SqlConnection(App.connectionString);
            await connection.OpenAsync();

            string query = "SELECT * FROM Profiles WHERE ID = @ID";
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);

            using SqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) {
                ProfileModel profile = new ProfileModel {
                    ID = (int)reader["ID"],
                    Name = reader["Name"].ToString(),
                    Surname = reader["Surname"].ToString(),
                    Email = reader["Email"].ToString()
                };

                return profile;
            } else {
                return null;
            }
        }

        public static async Task<bool> UpdateAsync(ProfileModel profile) {
            using SqlConnection connection = new SqlConnection(App.connectionString);
            await connection.OpenAsync();

            string query = @"
                UPDATE Profiles
                SET Name = @Name, Surname = @Surname, Email = @Email
                WHERE ID = @ID";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID", profile.ID);
            command.Parameters.AddWithValue("@Name", profile.Name);
            command.Parameters.AddWithValue("@Surname", profile.Surname);
            command.Parameters.AddWithValue("@Email", profile.Email);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<bool> DeleteAsync(int id) {
            using SqlConnection connection = new SqlConnection(App.connectionString);
            await connection.OpenAsync();

            string query = "DELETE FROM Profiles WHERE ID = @ID";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public static async Task SeedAsync() {
            List<ProfileModel> profiles = new List<ProfileModel>
            {
                new ProfileModel { Name = "John", Surname = "Doe", Email = "john.doe@example.com" },
                new ProfileModel { Name = "Jane", Surname = "Smith", Email = "jane.smith@example.com" },
                new ProfileModel { Name = "Bob", Surname = "Johnson", Email = "bob.johnson@example.com" }
            };

            foreach (ProfileModel profile in profiles)
                await CreateAsync(profile);
        }
    }
}
