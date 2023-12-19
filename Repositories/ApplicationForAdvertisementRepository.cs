using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using WPFOgloszenia.Models;

namespace WPFOgloszenia.Repositories {
    public class ApplicationForAdvertisementRepository {
        public static async Task CreateIfNotExistsAsync() {
            using SqlConnection connection = new(App.connectionString);
            string tableExistsQuery = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ApplicationsForAdvertisement'";
            await connection.OpenAsync();

            using SqlCommand tableExistsCommand = new(tableExistsQuery, connection);
            object? tableExistsResult = await tableExistsCommand.ExecuteScalarAsync();

            if (tableExistsResult == null) {
                string createTableQuery = @"
                    CREATE TABLE ApplicationsForAdvertisement
                    (
                        ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                        UserID INT NOT NULL,
                        AnnouncementID INT NOT NULL,
                    );";
                using SqlCommand createTableCommand = new(createTableQuery, connection);
                await createTableCommand.ExecuteNonQueryAsync();

                string relations =
                    "ALTER TABLE ApplicationsForAdvertisement " +
                    "ADD CONSTRAINT FK_NazwaRelacji222 " +
                    "FOREIGN KEY (AnnouncementID) " +
                    "REFERENCES Announcements(ID);" +
                    "ALTER TABLE ApplicationsForAdvertisement " +
                    "ADD CONSTRAINT FK_NazwaRelacji123 " +
                    "FOREIGN KEY (UserID) " +
                    "REFERENCES Users(ID);";
                SqlCommand command = new(relations, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        public static async Task<int> InsertAssync(ApplicationForAdvertisement application) {
            using SqlConnection connection = new(App.connectionString);
            string query = "INSERT INTO ApplicationsForAdvertisement (UserID,AnnouncementID) VALUES (@UserID,@AnnouncementID);SELECT SCOPE_IDENTITY();";
            await connection.OpenAsync();
            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@UserID", application.UserID);
            command.Parameters.AddWithValue("@AnnouncementID", application.AnnouncementID);
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
        public static async Task<bool> RemoveAsync(UserModel? user,int? ID) {
            using SqlConnection connection = new(App.connectionString);
            string query = "DELETE FROM ApplicationsForAdvertisement WHERE UserID=@UserID AND AnnouncementID=@AID;SELECT SCOPE_IDENTITY();";
            await connection.OpenAsync();
            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@USerID", user?.ID);
            command.Parameters.AddWithValue("@AID", ID);
            return await command.ExecuteNonQueryAsync() > 0;
        }
        public static async Task<bool> IsAlreadyApplicating(UserModel? user,int AnnouncementID) {
            using SqlConnection connection = new(App.connectionString);
            string query = "SELECT COUNT(*) FROM ApplicationsForAdvertisement WHERE UserID=@UserID AND AnnouncementID=@AID";
            await connection.OpenAsync();
            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@UserID", user?.ID);
            command.Parameters.AddWithValue("@AID", AnnouncementID);
            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        public static async Task<int> CountApplications(int ID) {
            using SqlConnection connection = new(App.connectionString);
            string query = "SELECT COUNT(*) FROM ApplicationsForAdvertisement WHERE AnnouncementID=@AID";
            await connection.OpenAsync();
            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@AID", ID);
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
        public static async Task<List<ApplicationForAdvertisement?>> GetAllApplications(int? UserID) {
            using SqlConnection connection = new(App.connectionString);
            string query = "SELECT * FROM ApplicationsForAdvertisement " +
                "WHERE ApplicationsForAdvertisement.AnnouncementID " +
                "IN(" +
                "SELECT ID FROM Announcements WHERE UserID = @UserID" +
                ")";
            await connection.OpenAsync();
            SqlCommand command=new(query, connection);
            command.Parameters.AddWithValue("@UserId",UserID);
            SqlDataReader reader = command.ExecuteReader();
            List<ApplicationForAdvertisement?> list = new();
            while (await reader.ReadAsync()) {
                ApplicationForAdvertisement application = new() {
                    ID = (int)reader["ID"],
                    User=await UserRepository.GetOneAsync((int)reader["UserID"]),
                    Announcement=await AnnouncementRepository.GetByIdAsync((int)reader["AnnouncementID"]),
                    UserID= (int)reader["UserID"],
                    AnnouncementID= (int)reader["AnnouncementID"],
                };
                list.Add(application);
            }
            return list;
        }
    }
}
