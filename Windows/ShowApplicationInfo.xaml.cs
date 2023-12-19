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
using System.Windows.Shapes;
using WPFOgloszenia.Models;
using WPFOgloszenia.Repositories;
using WPFOgloszenia.Views;

namespace WPFOgloszenia.Windows {
    /// <summary>
    /// Interaction logic for ShowApplicationInfo.xaml
    /// </summary>
    public partial class ShowApplicationInfo : Window {
        private ApplicationForAdvertisement _application = new();
        public ShowApplicationInfo(ApplicationForAdvertisement application) {
            InitializeComponent();
            _application = application;
            NameSurname.Content = $"{application.User?.Profile?.Name} {application.User?.Profile?.Surname}";
            Email.Content = application.User?.Profile?.Email;
            Login.Content = application.User?.Login;
        }

        private async void RefreshList() {
            if(Application.Current.MainWindow is MainWindow window) {
                if (window.NavigationFrame.Content is ProfileView profile)
                    profile.Applications.ItemsSource = await ApplicationForAdvertisementRepository.GetAllApplications(App.User.ID);
            }
        }

        private async void Denied_Click(object sender, RoutedEventArgs e) {
            await ApplicationForAdvertisementRepository.RemoveAsync(_application.User, _application.AnnouncementID);
            Close();
            MessageBox.Show("Pomyślnie odrzucono aplikacje");
            RefreshList();
        }

        private async void Accepted_Click(object sender, RoutedEventArgs e) {
            await ApplicationForAdvertisementRepository.RemoveAsync(_application.User, _application.AnnouncementID);
            Close();
            MessageBox.Show("Pomyślnie przyjęto aplikacje");
            RefreshList();
        }

        private void MinimalizeWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            WindowState = WindowState.Minimized;

        }

        private void CloseWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void DragMoveWindow(object sender, MouseButtonEventArgs e) {
            DragMove();
        }
    }
}
