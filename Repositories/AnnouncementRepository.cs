using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPFOgloszenia.Models;
using WPFOgloszenia.Repositories;

namespace WPFOgloszenia.Repositories {
    public static class AnnouncementRepository {
        private static readonly string connectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=Ogloszenia;Integrated Security=True";
        public static async Task CreateIfNotExistsAsync() {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string tableExistsQuery = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Announcement'";
            using SqlCommand tableExistsCommand = new(tableExistsQuery, connection);
            object tableExistsResult = await tableExistsCommand.ExecuteScalarAsync();

            if (tableExistsResult == null) {
                string createTableQuery = @"
            CREATE TABLE Announcement
            (
                ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                Title NVARCHAR(MAX),
                Description NVARCHAR(MAX),
                CategoryID INT,
                CompanyID INT,
                Position NVARCHAR(MAX),
                MinWage DECIMAL,
                MaxWage DECIMAL
            );";

                using SqlCommand createTableCommand = new(createTableQuery, connection);
                await createTableCommand.ExecuteNonQueryAsync().WaitAsync(TimeSpan.FromMilliseconds(5000));
                await SeedAsync();
            }


        }

        public static async Task<int> CreateAsync(AnnouncementModel announcement) {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string query = @"
                INSERT INTO Announcement (Title, Description, CategoryID, CompanyID, Position, MinWage, MaxWage)
                VALUES (@Title, @Description, @CategoryID, @CompanyID, @Position, @MinWage, @MaxWage);
                SELECT SCOPE_IDENTITY();";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Title", announcement.Title);
            command.Parameters.AddWithValue("@Description", announcement.Description);
            command.Parameters.AddWithValue("@CategoryID", announcement.CategoryID);
            command.Parameters.AddWithValue("@CompanyID", announcement.CompanyID);
            command.Parameters.AddWithValue("@Position", announcement.Position);
            command.Parameters.AddWithValue("@MinWage", announcement.MinWage);
            command.Parameters.AddWithValue("@MaxWage", announcement.MaxWage);

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        public static async Task<List<AnnouncementModel>> GetAllAsync() {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string query = @"
                SELECT Announcement.ID, Announcement.Title, Announcement.Description, Announcement.CategoryID,
                Announcement.CompanyID, Announcement.Position, Announcement.MinWage, Announcement.MaxWage,
                Categories.Name AS CategoryName, Companies.Name AS CompanyName, Companies.NIP,Companies.ImageLink,Companies.Description
                FROM Announcement
                INNER JOIN Categories ON Announcement.CategoryID = Categories.ID
                INNER JOIN Companies ON Announcement.CompanyID = Companies.ID";

            using SqlCommand command = new(query, connection);
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            List<AnnouncementModel> announcements = new();
            while (await reader.ReadAsync()) {
                AnnouncementModel announcement = new() {
                    ID = (int)reader["ID"],
                    Title = reader["Title"].ToString(),
                    Description = reader["Description"].ToString(),
                    CategoryID = (int)reader["CategoryID"],
                    CompanyID = (int)reader["CompanyID"],
                    Position = reader["Position"].ToString(),
                    MinWage = (decimal)reader["MinWage"],
                    MaxWage = (decimal)reader["MaxWage"],
                    Category = new CategoryModel {
                        ID = (int)reader["CategoryID"],
                        Name = reader["CategoryName"].ToString()
                    },
                    Company = new Company {
                        ID = (int)reader["CompanyID"],
                        Name = reader["CompanyName"].ToString(),
                        NIP = (int)reader["NIP"],
                        Description = reader["Description"].ToString(),
                        ImageLink = reader["ImageLink"].ToString(),
                    }
                };
                announcements.Add(announcement);
            }

            return announcements;
        }
        public static async Task<List<AnnouncementModel>> GetByTitleAsync(string title) {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string query = @"
        SELECT Announcement.ID, Announcement.Title, Announcement.Description, Announcement.CategoryID,
        Announcement.CompanyID, Announcement.Position, Announcement.MinWage, Announcement.MaxWage,
        Categories.Name AS CategoryName, Companies.Name AS CompanyName
        FROM Announcement
        INNER JOIN Categories ON Announcement.CategoryID = Categories.ID
        INNER JOIN Companies ON Announcement.CompanyID = Companies.ID
        WHERE Announcement.Title LIKE @Title";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Title", $"%{title}%"); // Użyj '%' jako symbol wieloznacznego dopasowania

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            List<AnnouncementModel> announcements = new();
            while (await reader.ReadAsync()) {
                AnnouncementModel announcement = new() {
                    ID = (int)reader["ID"],
                    Title = reader["Title"].ToString(),
                    Description = reader["Description"].ToString(),
                    CategoryID = (int)reader["CategoryID"],
                    CompanyID = (int)reader["CompanyID"],
                    Position = reader["Position"].ToString(),
                    MinWage = (decimal)reader["MinWage"],
                    MaxWage = (decimal)reader["MaxWage"],
                    Category = new CategoryModel {
                        ID = (int)reader["CategoryID"],
                        Name = reader["CategoryName"].ToString()
                    },
                    Company = new Company {
                        ID = (int)reader["CompanyID"],
                        Name = reader["CompanyName"].ToString(),
                        ImageLink = reader["ImageLink"].ToString(),
                        NIP = (int)reader["NIP"],
                        Description = reader["Description"].ToString(),
                    }
                };
                announcements.Add(announcement);
            }

            return announcements;
        }
        public static async Task<AnnouncementModel> GetByIdAsync(int id) {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string query = @"
                SELECT Announcement.ID, Announcement.Title, Announcement.Description, Announcement.CategoryID,
                Announcement.CompanyID, Announcement.Position, Announcement.MinWage, Announcement.MaxWage,
                Categories.Name AS CategoryName, Companies.Name AS CompanyName, Companies.NIP,Companies.ImageLink,Companies.Description
                FROM Announcement
                INNER JOIN Categories ON Announcement.CategoryID = Categories.ID
                INNER JOIN Companies ON Announcement.CompanyID = Companies.ID
                WHERE Announcement.ID = @ID";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@ID", id);

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync()) {
                AnnouncementModel announcement = new() {
                    ID = (int)reader["ID"],
                    Title = reader["Title"].ToString(),
                    Description = reader["Description"].ToString(),
                    CategoryID = (int)reader["CategoryID"],
                    CompanyID = (int)reader["CompanyID"],
                    Position = reader["Position"].ToString(),
                    MinWage = (decimal)reader["MinWage"],
                    MaxWage = (decimal)reader["MaxWage"],
                    Category = new CategoryModel {
                        ID = (int)reader["CategoryID"],
                        Name = reader["CategoryName"].ToString()
                    },
                    Company = new Company {
                        ID = (int)reader["CompanyID"],
                        Name = reader["CompanyName"].ToString(),
                        ImageLink = reader["ImageLink"].ToString(),
                        NIP = (int)reader["NIP"],
                        Description = reader["Description"].ToString(),
                    }
                };

                return announcement;
            } else {
                return null;
            }
        }

        public static async Task<bool> UpdateAsync(AnnouncementModel announcement) {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string query = @"
                UPDATE Announcement
                SET Title = @Title, Description = @Description, CategoryID = @CategoryID, CompanyID = @CompanyID,
                Position = @Position, MinWage = @MinWage, MaxWage = @MaxWage
                WHERE ID = @ID";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@ID", announcement.ID);
            command.Parameters.AddWithValue("@Title", announcement.Title);
            command.Parameters.AddWithValue("@Description", announcement.Description);
            command.Parameters.AddWithValue("@CategoryID", announcement.CategoryID);
            command.Parameters.AddWithValue("@CompanyID", announcement.CompanyID);
            command.Parameters.AddWithValue("@Position", announcement.Position);
            command.Parameters.AddWithValue("@MinWage", announcement.MinWage);
            command.Parameters.AddWithValue("@MaxWage", announcement.MaxWage);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<bool> DeleteAsync(int id) {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string query = "DELETE FROM Announcement WHERE ID = @ID";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@ID", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }
        private static async Task SeedAsync() {
            List<Company> companies = await CompanyRepository.GetAllAsync();
            List<CategoryModel> categories = await CategoryRepository.GetAllAsync();

            List<AnnouncementModel> announcements = new() {
                new AnnouncementModel {
                    Title = "Software Engineer Position",
                    Description = "We are hiring a skilled software engineer for our development team.",
                    CategoryID = categories[0].ID,
                    CompanyID = companies[0].ID,
                    Position = "Software Engineer",
                    MinWage = 50000,
                    MaxWage = 70000,
                },
                new AnnouncementModel {
                    Title = "Software Engineer Position",
                    Description = "We are hiring a skilled software engineer for our development team.",
                    CategoryID = categories[0].ID,
                    CompanyID = companies[0].ID,
                    Position = "Software Engineer",
                    MinWage = 60000,
                    MaxWage = 70000,
                },
                new AnnouncementModel {
                    Title = "Software Engineer Position",
                    Description = "We are hiring a skilled software engineer for our development team.",
                    CategoryID = categories[0].ID,
                    CompanyID = companies[0].ID,
                    Position = "Software Engineer",
                    MinWage = 70000,
                    MaxWage = 70000,
                },
            };

            foreach (AnnouncementModel item in announcements)
                await CreateAsync(item);
        }

    }
}