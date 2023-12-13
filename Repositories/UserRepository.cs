using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFOgloszenia.Models;
using WPFOgloszenia.Supports;

namespace WPFOgloszenia.Repositories {
    public class UserRepository {
        public static async Task CreateIfNotExistsAsync() {
            using SqlConnection connection = new SqlConnection(App.connectionString);
            await connection.OpenAsync();

            string tableExistsQuery = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users'";
            using SqlCommand tableExistsCommand = new(tableExistsQuery, connection);
            object? tableExistsResult = await tableExistsCommand.ExecuteScalarAsync();

            if (tableExistsResult == null) {
                string createTableQuery = @"
                    CREATE TABLE Users
                    (
                        ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                        Login NVARCHAR(MAX),
                        Password NVARCHAR(MAX),
                        Permission INT,
                        ProfileID INT NULL FOREIGN KEY REFERENCES Profiles(ID),
                        CompanyID INT NULL FOREIGN KEY REFERENCES Companies(ID)
                    );";

                using SqlCommand createTableCommand = new(createTableQuery, connection);
                await createTableCommand.ExecuteNonQueryAsync();
                //await SeedAsync();
            }
        }

        public static async Task<int> CreateAsync(UserModel user) {
            using SqlConnection connection = new SqlConnection(App.connectionString);
            await connection.OpenAsync();

            string query = @"
                INSERT INTO Users (Login, Password, ProfileID, Permission,CompanyID)
                VALUES (@Login, @Password, @ProfileID, @Permission, @CompanyID);
                SELECT SCOPE_IDENTITY();";

            using SqlCommand command = new(query, connection);
            if (user.CompanyID.HasValue) {
                command.Parameters.AddWithValue("@CompanyID", user.CompanyID);
            } else {
                command.Parameters.AddWithValue("@CompanyID", DBNull.Value);
            }
            command.Parameters.AddWithValue("@Login", user.Login);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@ProfileID", user.ProfileID);
            command.Parameters.AddWithValue("@Permission", user.Permission);

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        public static async Task<List<UserModel>> GetAllAsync() {
            using SqlConnection connection = new SqlConnection(App.connectionString);
            await connection.OpenAsync();

            string query = "SELECT * FROM Users";
            using SqlCommand command = new SqlCommand(query, connection);
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            List<UserModel> users = new List<UserModel>();
            while (await reader.ReadAsync()) {
                UserModel user = new UserModel {
                    ID = (int)reader["ID"],
                    Login = reader["Login"].ToString(),
                    Password = reader["Password"].ToString(),
                    Profile = await ProfileRepository.GetByIdAsync((int)reader["ProfileID"]),
                    Permission = (int)reader["Permission"]
                };
                users.Add(user);
            }

            return users;
        }

        public static async Task<UserModel?> GetOneAsync(string login, string password) {
            using SqlConnection connection = new(App.connectionString);
            await connection.OpenAsync();
            string query = "SELECT TOP(1) * FROM Users WHERE Login=@Login";
            SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Login", login);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) {
                UserModel? user = new() {
                    ID = (int)reader["ID"],
                    Login = reader["Login"].ToString(),
                    Password = reader["Password"].ToString(),
                    Profile = await ProfileRepository.GetByIdAsync((int)reader["ProfileID"]),
                    Permission = (int)reader["Permission"],
                    CompanyID = reader["CompanyID"] != DBNull.Value ? (int)reader["CompanyID"] : (int?)null,
                    Company = reader["CompanyID"] != DBNull.Value ? await CompanyRepository.GetByIdAsync((int)reader["CompanyID"]) : null,
                };
                if (PasswordHandling.VerifyPassword(password, user.Password ?? ""))
                    return user;
                else
                    return null;
            }
            return null;
        }

        public static async Task SetUserCompany(int? companyID, int? UserID) {
            using SqlConnection connection = new(App.connectionString);
            await connection.OpenAsync();
            string query = "Update Users SET CompanyID=@CompanyID WHERE ID=@UserID";
            SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@CompanyID", companyID);
            command.Parameters.AddWithValue("@UserID", UserID);
            await command.ExecuteNonQueryAsync();
        }

        public static async Task<UserModel?> GetOneAsync(int? ID) {
            using SqlConnection connection = new(App.connectionString);
            await connection.OpenAsync();
            string query = "SELECT TOP(1) * FROM Users WHERE ID=@ID";
            SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@ID", ID);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) {
                UserModel? user = new() {
                    ID = (int)reader["ID"],
                    Login = reader["Login"].ToString(),
                    Password = reader["Password"].ToString(),
                    Profile = await ProfileRepository.GetByIdAsync((int)reader["ProfileID"]),
                    Permission = (int)reader["Permission"],
                    CompanyID = reader["CompanyID"] != DBNull.Value ? (int)reader["CompanyID"] : (int?)null,
                    Company = reader["CompanyID"] != DBNull.Value ? await CompanyRepository.GetByIdAsync((int)reader["CompanyID"]) : null,
                };
                return user;
            }
            return null;
        }

        public static async Task<bool> IsLoginExists(string login) {
            using SqlConnection connection = new(App.connectionString);
            await connection.OpenAsync();
            string query = "SELECT TOP(1) * FROM Users WHERE Login=@Login";
            SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Login", login);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return true;
            return false;
        }
    }
}
