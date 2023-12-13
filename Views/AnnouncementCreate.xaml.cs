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

namespace WPFOgloszenia.Views {
    /// <summary>
    /// Interaction logic for AnnouncementCreate.xaml
    /// </summary>
    public partial class AnnouncementCreate : Page {
        public AnnouncementCreate() {
            InitializeComponent();
            Setup();
        }
        private async void Setup() {
            List<CategoryModel> categories = await CategoryRepository.GetAllAsync();
            List<TypeOfWork> typeOfWorks = await TypeOfWorkRepository.GetAllAsync();

            CategoryComboBox.ItemsSource = categories;
            TypeOfWorkComboBox.ItemsSource = typeOfWorks;
        }

        private async void ActionButton_Click(object sender, RoutedEventArgs e) {
            AnnouncementModel announcement = new() {
                Title = TitleEntry.Text,
                Description = DescriptionEntry.Text,
                Position = PositionEntry.Text,
                CategoryID = (CategoryComboBox.SelectedItem as CategoryModel).ID,
                CompanyID = App.User?.Company?.ID,
                TypeOfWorkID = (TypeOfWorkComboBox?.SelectedItem as TypeOfWork).ID,
                MinWage = decimal.Parse(MinWageEntry.Text),
                MaxWage = decimal.Parse(MaxWageEntry.Text),
            };
            int id = await AnnouncementRepository.CreateAsync(announcement);
            if (Application.Current.MainWindow is MainWindow window)
                window.NavigationFrame.Navigate(new AnnouncementsView(id));
        }
    }
}
