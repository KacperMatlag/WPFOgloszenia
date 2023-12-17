using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
            Setup();
        }
        private async void Setup() {
            CompanyList.ItemsSource = await CompanyRepository.GetAllAsync();
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
                Description = CompanyDescription.Text,
                NIP = int.Parse(CompanyNip.Text),
                ImageLink = CompanyImageUrl.Text,
            };
            int companyID = await CompanyRepository.CreateAsync(company);
            await UserRepository.SetUserCompany(companyID, App.User?.ID);
            App.User = await UserRepository.GetOneAsync(App.User?.ID);
            this.Close();
            MessageBox.Show($"FUstawiono firme na:  {App.User?.Company?.Name}");
        }

        private async void CompanySearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            CompanyList.ItemsSource = await CompanyRepository.GetAllByNameAsync(CompanySearch.Text);
        }

        private async void CompanyList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (CompanyList.SelectedItem is Company company) {
                await UserRepository.SetUserCompany(company.ID, App.User?.ID);
                App.User = await UserRepository.GetOneAsync(App.User?.ID);
                this.Close();
                MessageBox.Show($"FUstawiono firme na:  {App.User?.Company?.Name}");
            }
        }
    }
}
