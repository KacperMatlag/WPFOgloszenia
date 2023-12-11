using System.Windows;
using System.Windows.Input;
using WPFOgloszenia.Models;
using WPFOgloszenia.Repositories;
namespace WPFOgloszenia.Windows {
    /// <summary>
    /// Interaction logic for CompanyCreate.xaml
    /// </summary>
    public partial class CompanyCreate : Window {
        public CompanyCreate() {
            InitializeComponent();
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

        private async void AddCompanyToUser_Click(object sender, RoutedEventArgs e) {
            Company company = new() {
                Name = CompanyName.Text,
                Description=CompanyDescription.Text,
                NIP=int.Parse(CompanyNip.Text),
                ImageLink=CompanyImageUrl.Text,
            };
            int companyID=await CompanyRepository.CreateAsync(company);
        }
    }
}
