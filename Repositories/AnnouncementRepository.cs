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

            string tableExistsQuery = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Announcements'";
            using SqlCommand tableExistsCommand = new(tableExistsQuery, connection);
            object? tableExistsResult = await tableExistsCommand.ExecuteScalarAsync();

            if (tableExistsResult == null) {
                string createTableQuery = @"
                    CREATE TABLE Announcements
            (
                ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                Title NVARCHAR(MAX),
                Description NVARCHAR(MAX),
                CategoryID INT,
                CompanyID INT,
                        TypeOfWorkID INT,
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
                INSERT INTO Announcements (Title, Description, CategoryID, CompanyID, TypeOfWorkID, Position, MinWage, MaxWage)
                VALUES (@Title, @Description, @CategoryID, @CompanyID,@TypeOfWorkID, @Position, @MinWage, @MaxWage);
                SELECT SCOPE_IDENTITY();";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Title", announcement.Title);
            command.Parameters.AddWithValue("@Description", announcement.Description);
            command.Parameters.AddWithValue("@CategoryID", announcement.CategoryID);
            command.Parameters.AddWithValue("@CompanyID", announcement.CompanyID);
            command.Parameters.AddWithValue("@TypeOfWorkID", announcement.TypeOfWorkID);
            command.Parameters.AddWithValue("@Position", announcement.Position);
            command.Parameters.AddWithValue("@MinWage", announcement.MinWage);
            command.Parameters.AddWithValue("@MaxWage", announcement.MaxWage);

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        public static async Task<List<AnnouncementModel>> GetAllAsync(string title="") {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string query = @"
                SELECT
                Announcements.ID,
                Announcements.Title,
                Announcements.Description,
                Announcements.CategoryID,
                Announcements.CompanyID,
                Announcements.Position,
                Announcements.MinWage,
                Announcements.MaxWage,
                Categories.Name AS CategoryName,
                Companies.Name AS CompanyName,
                Companies.NIP,
                Companies.Description AS CompanyDescription,
                Companies.ImageLink,
                TypeOfWorks.ID AS TypeOfWorkID,
                TypeOfWorks.Name AS TypeOfWorkName
            FROM
                Announcements
            INNER JOIN Categories ON Announcements.CategoryID = Categories.ID
            INNER JOIN Companies ON Announcements.CompanyID = Companies.ID
            INNER JOIN TypeOfWorks ON Announcements.TypeOfWorkID = TypeOfWorks.ID
            WHERE Announcements.Title LIKE @Title_
            ";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Title_", $"%{title}%");
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
                    TypeOfWork=new TypeOfWork() {
                        ID = (int)reader["TypeOfWorkID"],
                        Name = reader["TypeOfWorkName"].ToString(),
                    }
                };
                announcements.Add(announcement);
            }

            return announcements;
        }
        public static async Task<AnnouncementModel?> GetByIdAsync(int id) {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string query = @"
                SELECT
                Announcements.ID,
                Announcements.Title,
                Announcements.Description,
                Announcements.CategoryID,
                Announcements.CompanyID,
                Announcements.Position,
                Announcements.MinWage,
                Announcements.MaxWage,
                Categories.Name AS CategoryName,
                Companies.Name AS CompanyName,
                Companies.NIP,
                Companies.Description AS CompanyDescription,
                Companies.ImageLink,
                TypeOfWorks.ID AS TypeOfWorkID,
                TypeOfWorks.Name AS TypeOfWorkName
            FROM
                Announcements
            INNER JOIN Categories ON Announcements.CategoryID = Categories.ID
            INNER JOIN Companies ON Announcements.CompanyID = Companies.ID
            INNER JOIN TypeOfWorks ON Announcements.TypeOfWorkID = TypeOfWorks.ID
            WHERE Announcements.ID=@ID";

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
                    },
                    TypeOfWork = new TypeOfWork() {
                        ID = (int)reader["TypeOfWorkID"],
                        Name = reader["TypeOfWorkName"].ToString(),
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
                SET Title = @Title, Description = @Description, CategoryID = @CategoryID, CompanyID = @CompanyID, TypeOfWorkID=@TypeOFWorkID
                Position = @Position, MinWage = @MinWage, MaxWage = @MaxWage
                WHERE ID = @ID";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@ID", announcement.ID);
            command.Parameters.AddWithValue("@Title", announcement.Title);
            command.Parameters.AddWithValue("@Description", announcement.Description);
            command.Parameters.AddWithValue("@CategoryID", announcement.CategoryID);
            command.Parameters.AddWithValue("@CompanyID", announcement.CompanyID);
            command.Parameters.AddWithValue("@TypeOfWorkID", announcement.TypeOfWorkID);
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
        public static async Task SeedAsync() {
            List<AnnouncementModel> announcements = new() {
                new AnnouncementModel {
                    Title = "Robota 1",
                    Description = "We are hiring a skilled software engineer for our development team.",
                    CategoryID = 1,
                    CompanyID = 2,
                    TypeOfWorkID = 3,
                    Position = "Software Engineer",
                    MinWage = 50000,
                    MaxWage = 70000,
                },
                new AnnouncementModel {
                    Title = "Robota 2",
                    Description = "We are hiring a skilled software engineer for our development team.",
                    CategoryID = 3,
                    CompanyID = 2,
                    TypeOfWorkID = 1,
                    Position = "Software Engineer",
                    MinWage = 60000,
                    MaxWage = 70000,
                },
                new AnnouncementModel {
                    Title = "Robota 3",
                    Description = "We are hiring a skilled software engineer for our development team.",
                    CategoryID = 2,
                    CompanyID = 1,
                    TypeOfWorkID = 3,
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