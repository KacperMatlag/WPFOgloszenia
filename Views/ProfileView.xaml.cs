using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFOgloszenia.Models;
using WPFOgloszenia.Repositories;
using WPFOgloszenia.Windows;

namespace WPFOgloszenia.Views {
    /// <summary>
    /// Interaction logic for ProfileView.xaml
    /// </summary>
    public partial class ProfileView : Page {
        public ProfileView() {
            InitializeComponent();
            EditName.Text = App.User?.Profile?.Name;
            EditSurname.Text = App.User?.Profile?.Surname;
            EditEmail.Text = App.User?.Profile?.Email;
            Setup();
        }
        private async void Setup() {
            UsersAnnouncements.ItemsSource= await AnnouncementRepository.GetByUser(App.User?.ID);
            Applications.ItemsSource = await ApplicationForAdvertisementRepository.GetAllApplications(App.User?.ID??0);
        }
        private async void PasswordChange_Click(object sender, RoutedEventArgs e) {
            if (await UserRepository.PasswordChange(App.User, NewPassword.Text, OldPassword.Text))
                MessageBox.Show("Hasło pomyślnie zmienione");
            else
                MessageBox.Show("Podano niepoprawne stare haslo");
        }

        private void CompanyChange_Click(object sender, RoutedEventArgs e) {
            CompanyCreate companyCreate = new();
            companyCreate.ShowDialog();
        }

        private async void EditProfile_Click(object sender, RoutedEventArgs e) {
            ProfileModel? toUpdate = App.User?.Profile;
            if (toUpdate != null) {
                toUpdate.Name = EditName.Text;
                toUpdate.Surname = EditSurname.Text;
                toUpdate.Email = EditEmail.Text;

                if(await ProfileRepository.UpdateAsync(toUpdate))
                    MessageBox.Show("Pomyślnie uaktualniono profil");
                else
                    MessageBox.Show("Wystapił błąd");
            }
        }

        private void UsersAnnouncements_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (UsersAnnouncements.SelectedItem is AnnouncementModel a)
                if (Application.Current.MainWindow is MainWindow window)
                    window.NavigationFrame.Navigate(new AnnouncementCreate(a));
        }

        private void Applications_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if(Applications.SelectedItem is ApplicationForAdvertisement a) {
                ShowApplicationInfo info = new(a);
                info.ShowDialog();
            }

        }
    }
}
