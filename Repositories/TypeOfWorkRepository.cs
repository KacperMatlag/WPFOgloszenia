using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFOgloszenia.Models;

namespace WPFOgloszenia.Repositories {
    public class TypeOfWorkRepository {
        public static async Task CreateIfNotExistsAsync() {
            using SqlConnection connection = new(App.connectionString);
            string tableExistsQuery = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TypeOfWorks'";


            await connection.OpenAsync();

            using SqlCommand tableExistsCommand = new(tableExistsQuery, connection);
            object? tableExistsResult = await tableExistsCommand.ExecuteScalarAsync();

            if (tableExistsResult == null) {
                string createTableQuery = @"
                        CREATE TABLE TypeOfWorks
                        (
                            ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                            Name NVARCHAR(MAX) NOT NULL
                        );";

                using SqlCommand createTableCommand = new(createTableQuery, connection);
                await createTableCommand.ExecuteNonQueryAsync();
                await SeedAsync();
            }

        }

        public static async Task<List<TypeOfWork>> GetAllAsync() {
            using SqlConnection connection = new(App.connectionString);
            await connection.OpenAsync();
            string query = "SELECT * FROM TypeOfWorks";
            using SqlCommand command = new SqlCommand(query, connection);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            List<TypeOfWork> TypeOfWorks = new List<TypeOfWork>();
            while (await reader.ReadAsync()) {
                TypeOfWork typeOfWork = new TypeOfWork {
                    ID = (int)reader["ID"],
                    Name = reader["Name"].ToString()
                };
                TypeOfWorks.Add(typeOfWork);
            }
            return TypeOfWorks;
        }

        public static async Task<TypeOfWork?> GetByIdAsync(int id) {
            using SqlConnection connection = new SqlConnection(App.connectionString);
            await connection.OpenAsync();
            string query = "SELECT * FROM TypeOfWorks WHERE ID = @ID";
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) {
                TypeOfWork category = new TypeOfWork {
                    ID = (int)reader["ID"],
                    Name = reader["Name"].ToString()
                };

                return category;
            } else
                return null;
        }
        public static async Task AddAsync(TypeOfWork category) {
            using SqlConnection connection = new(App.connectionString);
            await connection.OpenAsync();
            string query = "INSERT INTO TypeOfWorks (Name) VALUES (@Name)";
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", category.Name);
            await command.ExecuteNonQueryAsync();
        }

        public static async Task UpdateAsync(TypeOfWork category) {
            using SqlConnection connection = new(App.connectionString);
            await connection.OpenAsync();
            string query = "UPDATE TypeOfWorks SET Name = @Name WHERE ID = @ID";
            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@ID", category.ID);
            command.Parameters.AddWithValue("@Name", category.Name);
            await command.ExecuteNonQueryAsync();
        }

        public static async Task SeedAsync() {
            List<TypeOfWork> models = new() {
                new TypeOfWork() {
                    ID = 1,
                    Name="Zdalnie"
                },
                new TypeOfWork() {
                    ID=2,
                    Name="Hybrydowo"
                },
                new TypeOfWork() {
                    ID=3,
                    Name="Stacjonarnie"
                },
            };
            foreach (var model in models)
                await AddAsync(model);
        }
    }
}
