using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPFOgloszenia.Models;

namespace WPFOgloszenia.Repositories {
    public static class CompanyRepository {
        public static async Task CreateIfNotExistsAsync() {
            using SqlConnection connection = new(App.connectionString);
            await connection.OpenAsync();

            string tableExistsQuery = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Companies'";
            using SqlCommand tableExistsCommand = new(tableExistsQuery, connection);
            object? tableExistsResult = await tableExistsCommand.ExecuteScalarAsync();

            if (tableExistsResult == null) {
                // Tabela nie istnieje, więc ją tworzymy
                string createTableQuery = @"
                    CREATE TABLE Companies
                    (
                        ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                        Name NVARCHAR(MAX) NOT NULL,
                        Description NVARCHAR(MAX),
                        NIP INT,
                        Location NVARCHAR(MAX),
                        ImageLink NVARCHAR(MAX)
                    );";

                using SqlCommand createTableCommand = new(createTableQuery, connection);
                await createTableCommand.ExecuteNonQueryAsync();
                await SeedAsync();
            }

        }
        public static async Task<int> CreateAsync(Company company) {
            using SqlConnection connection = new(App.connectionString);
            await connection.OpenAsync();

            string query = @"
        INSERT INTO Companies (Name, Description, NIP, Location, ImageLink)
        VALUES (@Name, @Description, @NIP, @Location, @ImageLink);
        SELECT SCOPE_IDENTITY();";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Name", company.Name);
            command.Parameters.AddWithValue("@Description", company.Description);
            command.Parameters.AddWithValue("@NIP", company.NIP);
            command.Parameters.AddWithValue("@Location", "123");
            command.Parameters.AddWithValue("@ImageLink", company.ImageLink);

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        public static async Task<List<Company>> GetAllAsync() {
            using SqlConnection connection = new(App.connectionString);
            await connection.OpenAsync();

            string query = "SELECT * FROM Companies";
            using SqlCommand command = new(query, connection);
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            List<Company> companies = new List<Company>();
            while (await reader.ReadAsync()) {
                Company company = new Company {
                    ID = (int)reader["ID"],
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    NIP = (int)reader["NIP"],
                    Location = reader["Location"].ToString(),
                    ImageLink = reader["ImageLink"].ToString()
                };
                companies.Add(company);
            }

            return companies;
        }

        public static async Task<Company?> GetByIdAsync(int id) {
            using SqlConnection connection = new SqlConnection(App.connectionString);
            await connection.OpenAsync();

            string query = "SELECT * FROM Companies WHERE ID = @ID";
            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@ID", id);

            using SqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) {
                Company company = new Company {
                    ID = (int)reader["ID"],
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    NIP = (int)reader["NIP"],
                    Location = reader["Location"].ToString(),
                    ImageLink = reader["ImageLink"].ToString()
                };

                return company;
            } else {
                return null;
            }
        }
        public static async Task<List<Company>> GetAllByNameAsync(string name = "") {
            using SqlConnection connection = new(App.connectionString);
            await connection.OpenAsync();

            string query = "SELECT * FROM Companies WHERE Name Like @Name";
            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Name",$"%{name}%");
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            List<Company> companies = new List<Company>();
            while (await reader.ReadAsync()) {
                Company company = new Company {
                    ID = (int)reader["ID"],
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    NIP = (int)reader["NIP"],
                    Location = reader["Location"].ToString(),
                    ImageLink = reader["ImageLink"].ToString()
                };
                companies.Add(company);
            }

            return companies;
        }

        public static async Task<bool> UpdateAsync(Company company) {
            using SqlConnection connection = new(App.connectionString);
            await connection.OpenAsync();

            string query = @"
                UPDATE Companies
                SET Name = @Name, Description = @Description, NIP = @NIP, Location = @Location, ImageLink = @ImageLink
                WHERE ID = @ID";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@ID", company.ID);
            command.Parameters.AddWithValue("@Name", company.Name);
            command.Parameters.AddWithValue("@Description", company.Description);
            command.Parameters.AddWithValue("@NIP", company.NIP);
            command.Parameters.AddWithValue("@Location", company.Location);
            command.Parameters.AddWithValue("@ImageLink", company.ImageLink);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<bool> DeleteAsync(int id) {
            using SqlConnection connection = new(App.connectionString);
            await connection.OpenAsync();

            string query = "DELETE FROM Companies WHERE ID = @ID";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@ID", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }
        public static async Task SeedAsync() {
            List<Company> companies = new List<Company> {
        new Company { ID=1,Name = "Tech Solutions Inc", Description = "Information Technology", NIP = 123456789, Location = "City A", ImageLink = "https://www.gotowelogo.pl/wp-content/uploads/2019/12/Gotowelogo_579.png" },
        new Company { ID=2,Name = "ABC Corp", Description = "Software Development", NIP = 987654321, Location = "City B", ImageLink = "https://projektowane.pl/site_media/uploads/artykuly/starbucks-logo.png" },
        new Company { ID=3,Name = "XYZ Ltd", Description = "Manufacturing", NIP = 456789123, Location = "City C", ImageLink = "https://e-reklamowe.com/wp-content/uploads/2018/01/logo-ochrona-bezpieczenstwo-1.jpg" },
        new Company {ID=4, Name = "123 Industries", Description = "Consulting", NIP = 789123456, Location = "City D", ImageLink = "https://www.grupapns.pl/wp-content/uploads/2020/04/projektowanie-logo-dla-firm-1.png" },
        new Company {ID = 5,  Name = "Global Services Co", Description = "Financial Services", NIP = 321654987, Location = "City E", ImageLink = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ17HeDZW7y_S35hBu8tDotTIw-TSP5Wp913A&usqp=CAU" }
    };
            foreach (Company company in companies)
                await CreateAsync(company);
        }
    }
}

