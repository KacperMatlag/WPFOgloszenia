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
        AnnouncementModel Announcement = new();
        public AnnouncementCreate() {
            InitializeComponent();
            Setup();
        }
        public AnnouncementCreate(AnnouncementModel model) {
            InitializeComponent();
            Setup();
            Setup_(model.ID);
            ActionButton.Click -= ActionButton_AddClick;
            ActionButton.Click+=ActionButton_EditClick;
        }
        private async void Setup_(int id) {
            Announcement = await AnnouncementRepository.GetByIdAsync(id);
            TitleEntry.Text = Announcement.Title;
            DescriptionEntry.Text = Announcement.Description;
            PositionEntry.Text = Announcement.Position;
            MinWageEntry.Text = Announcement.MinWage.ToString();
            MaxWageEntry.Text = Announcement.MaxWage.ToString();

            ActionButton.Content = "Edytuj Ogłoszenie";
            ActionButton.Click -= ActionButton_AddClick;
        }
        private async void Setup() {
            List<CategoryModel> categories = await CategoryRepository.GetAllAsync();
            List<TypeOfWork> typeOfWorks = await TypeOfWorkRepository.GetAllAsync();

            CategoryComboBox.ItemsSource = categories;
            TypeOfWorkComboBox.ItemsSource = typeOfWorks;
            CompanyName.Content = App.User?.Company?.Name;
            NIP.Content = $"NIP: {App.User?.Company?.NIP}";
        }
        private async void ActionButton_EditClick(object sender, RoutedEventArgs e) {
            Announcement.Title = TitleEntry.Text;
            Announcement.Description= DescriptionEntry.Text;
            Announcement.MinWage = int.Parse(MinWageEntry.Text);
            Announcement.MaxWage = int.Parse(MaxWageEntry.Text);
            Announcement.Position= PositionEntry.Text;
            Announcement.CategoryID = (CategoryComboBox.SelectedItem as CategoryModel).ID;
            Announcement.TypeOfWorkID = (TypeOfWorkComboBox.SelectedItem as TypeOfWork).ID;
            await AnnouncementRepository.UpdateAsync(Announcement);
            if (Application.Current.MainWindow is MainWindow window)
                window.NavigationFrame.Navigate(new ProfileView());
        }
        private async void ActionButton_AddClick(object sender, RoutedEventArgs e) {
            AnnouncementModel announcement = new() {
                Title = TitleEntry.Text,
                Description = DescriptionEntry.Text,
                Position = PositionEntry.Text,
                CategoryID = (CategoryComboBox.SelectedItem as CategoryModel)?.ID,
                CompanyID = App.User?.Company?.ID,
                TypeOfWorkID = (TypeOfWorkComboBox?.SelectedItem as TypeOfWork)?.ID,
                MinWage = decimal.Parse(MinWageEntry.Text),
                MaxWage = decimal.Parse(MaxWageEntry.Text),
                UserID = App.User?.ID,
            };
            int id = await AnnouncementRepository.CreateAsync(announcement);
            if (Application.Current.MainWindow is MainWindow window)
                window.NavigationFrame.Navigate(new AnnouncementsView(id));
        }
    }
}
