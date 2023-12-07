using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPFOgloszenia.Models;

namespace WPFOgloszenia.Repositories {
    public static class CategoryRepository {
        private static readonly string connectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=Ogloszenia;Integrated Security=True";
        public static async Task CreateIfNotExistsAsync() {
            using SqlConnection connection = new(connectionString);
            string tableExistsQuery = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categories'";
            await connection.OpenAsync();

            using SqlCommand tableExistsCommand = new(tableExistsQuery, connection);
            object? tableExistsResult = await tableExistsCommand.ExecuteScalarAsync();

            if (tableExistsResult == null) {
                string createTableQuery = @"
            CREATE TABLE Categories
            (
                ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(MAX) NOT NULL
            );";
                using SqlCommand createTableCommand = new(createTableQuery, connection);
                await createTableCommand.ExecuteNonQueryAsync();
                await SeedAsync();
            }
        }

        public static async Task<List<CategoryModel>> GetAllAsync() {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();
            string query = "SELECT * FROM Categories";
            using SqlCommand command = new SqlCommand(query, connection);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            List<CategoryModel> categories = new List<CategoryModel>();
            while (await reader.ReadAsync()) {
                CategoryModel category = new CategoryModel {
                    ID = (int)reader["ID"],
                    Name = reader["Name"].ToString()
                };
                categories.Add(category);
            }
            return categories;
        }

        public static async Task<CategoryModel?> GetByIdAsync(int id) {
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            string query = "SELECT * FROM Categories WHERE ID = @ID";
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) {
                CategoryModel category = new CategoryModel {
                    ID = (int)reader["ID"],
                    Name = reader["Name"].ToString()
                };

                return category;
            } else
                return null;
        }
        public static async Task AddAsync(CategoryModel category) {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();
            string query = "INSERT INTO Categories (Name) VALUES (@Name)";
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", category.Name);
            await command.ExecuteNonQueryAsync();
        }

        public static async Task UpdateAsync(CategoryModel category) {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();
            string query = "UPDATE Categories SET Name = @Name WHERE ID = @ID";
            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@ID", category.ID);
            command.Parameters.AddWithValue("@Name", category.Name);
            await command.ExecuteNonQueryAsync();
        }

        public static async Task SeedAsync() {
            List<CategoryModel> models = new() {
                new CategoryModel() {
                    ID = 1,
                    Name="IT"
                },
                new CategoryModel() {
                    ID=2,
                    Name="Hotelarstwo"
                },
                new CategoryModel() {
                    ID=3,
                    Name="Edukacja"
                },
                new CategoryModel() {
                    ID=4,
                    Name="Mechanika pojazów"
                },
            };
            foreach (var model in models)
                await AddAsync(model);
        }
    }
}
