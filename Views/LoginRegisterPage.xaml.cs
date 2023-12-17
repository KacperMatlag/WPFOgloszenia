using System;
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
            UserLogin.Text = "Login";
            UserPassword.Password = "Password";
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

            if (profile.Validate() is not null) {
                MessageBox.Show(profile.Validate());
                return;
            }

            UserModel user = new() {
                Login = Login.Text,
                Password = Password.Password,
                Permission = 1,
                ProfileID = -1,
                CompanyID=null,
                Company=null,
            };

            if (user.Validate() is not null) {
                MessageBox.Show(profile.Validate());
                return;
            }

            user.Password=PasswordHandling.HashPassword(user.Password);
            int inserttedProfileID = await ProfileRepository.CreateAsync(profile);
            user.ProfileID = inserttedProfileID;
            int userID = await UserRepository.CreateAsync(user);
            App.User = await UserRepository.GetOneAsync(userID);
            if (Application.Current.MainWindow is MainWindow window) {
                window.NavigationFrame.Navigate(new AnnouncementList());
                window.MenuSelectionChange(window.MainList);
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e) {
            string Login = UserLogin.Text;
            string Password = UserPassword.Password;
            UserModel? user = await UserRepository.GetOneAsync(Login, Password);
            if (user is not null && Application.Current.MainWindow is MainWindow window) {
                App.User = user;
                window.LoginLogOut.Content = "Wyloguj się";
                window.LoginLogOut.MouseLeftButtonDown -= window.MenuLogin_MouseLeftButtonDown;
                window.LoginLogOut.MouseLeftButtonDown += window.MenuLogOut_MouseLeftButtonDown;
                window.UserName.Content = $"Zalogowano jako: {App.User.Login}";
                window.Addannoucement.Visibility = Visibility.Visible;
                window.MenuSelectionChange(window.MainList);
                window.NavigationFrame.Navigate(new AnnouncementList());
            } else
                MessageBox.Show("Niepoprawy Login lub Hasło");
        }
    }
}
