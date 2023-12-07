using System.Windows;
using System.Windows.Controls;
using WPFOgloszenia.Models;
using WPFOgloszenia.Repositories;
using WPFOgloszenia.Supports;

namespace WPFOgloszenia.Views {
    /// <summary>
    /// Interaction logic for LoginRegisterPage.xaml
    /// </summary>
    public partial class LoginRegisterPage : Page {
        public LoginRegisterPage() {
            InitializeComponent();
        }

        private async void Register_Click(object sender, RoutedEventArgs e) {

            if (await UserRepository.IsLoginExists(Login.Text)) {
                MessageBox.Show("Login jest już zajęty");
                return;
            }

            ProfileModel profile = new() {
                Name = Name.Text,
                Surname = Name.Text,
                Email = Name.Text,
            };
            int inserttedProfileID = await ProfileRepository.CreateAsync(profile);
            UserModel user = new() {
                Login = Login.Text,
                Password = PasswordHandling.HashPassword(Password.Password),
                Permission = 1,
                ProfileID = inserttedProfileID,
            };
            await UserRepository.CreateAsync(user);
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e) {
            string Login = UserLogin.Text;
            string Password = UserPassword.Password;
            UserModel? user = await UserRepository.GetOneAsync(Login, Password);
            if (user is not null) {
                App.User = user;
                LoginAndLogoutHandling.LoginChanges();
            }
            else
                MessageBox.Show("Niepoprawy Login lub Hasło");
        }
    }
}
