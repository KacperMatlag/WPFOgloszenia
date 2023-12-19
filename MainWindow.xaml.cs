using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPFOgloszenia.Views;
using WPFOgloszenia.Repositories;
using System.Threading.Tasks;
using System;
using WPFOgloszenia.Windows;

namespace WPFOgloszenia {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Setup();
        }
        private async void Setup() {
            CheckTables();
            await Task.Delay(1000);
            NavigationFrame.Navigate(new AnnouncementList());
        }
        private async static void CheckTables() {
            try {
                await CategoryRepository.CreateIfNotExistsAsync();
                await CompanyRepository.CreateIfNotExistsAsync();
                await TypeOfWorkRepository.CreateIfNotExistsAsync();
                await ProfileRepository.CreateIfNotExistsAsync();
                await UserRepository.CreateIfNotExistsAsync();
                await AnnouncementRepository.CreateIfNotExistsAsync();
                await ApplicationForAdvertisementRepository.CreateIfNotExistsAsync();
            } catch (Exception e) {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        private void MinimalizeWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void CloseWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Application.Current.Shutdown();
        }

        private void DragMoveWindow(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        public void MenuLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (sender is Label label)
                MenuSelectionChange(label);
            if (NavigationFrame?.Content is Page page && page.Title != "LoginRegisterPage")
                NavigationFrame?.Navigate(new LoginRegisterPage());
        }

        public void MenuLogOut_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            App.User = null;
            LoginLogOut.Content = "Zaloguj się";
            LoginLogOut.MouseLeftButtonDown += MenuLogin_MouseLeftButtonDown;
            LoginLogOut.MouseLeftButtonDown -= MenuLogOut_MouseLeftButtonDown;
            UserName.Content = "Niezalogowano";
            Addannoucement.Visibility = Visibility.Hidden;
        }

        private void MenuMainPage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (sender is Label label)
                MenuSelectionChange(label);
            if (NavigationFrame?.Content is Page p && p.Title != "announcementList")
                NavigationFrame?.Navigate(new AnnouncementList());
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e) {
            if (sender is Label label)
                label.Foreground = Brushes.Red;
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e) {
            if (sender is Label label)
                label.Foreground = Brushes.Black;
        }

        public void MenuSelectionChange(Label selectedLabel) {
            foreach (var item in Menu.Children.OfType<StackPanel>().ToList()) {
                if (item.Children[0] is Label label1) {
                    label1.BorderThickness = new Thickness(0);
                    label1.BorderBrush = Brushes.Transparent;
                }
            }
            selectedLabel.BorderBrush = Brushes.Red;
            selectedLabel.BorderThickness = new Thickness(0, 0, 0, 2);
        }

        private void Addannoucement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (sender is Label label)
                MenuSelectionChange(label);
            if (App.User is not null && App.User.Company is null) {
                CompanyCreate company = new();
                company.ShowDialog();
            } else {
                NavigationFrame.Navigate(new AnnouncementCreate());
            }

        }

        private void UserName_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (App.User is not null) {
                NavigationFrame.Navigate(new ProfileView());
            }
        }
    }
}
