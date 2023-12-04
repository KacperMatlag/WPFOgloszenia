using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPFOgloszenia.Controls;
using WPFOgloszenia.Repositories;
using WPFOgloszenia.Models;
namespace WPFOgloszenia.Views {
    /// <summary>
    /// Interaction logic for announcementList.xaml
    /// </summary>
    public partial class AnnouncementList : Page {
        public AnnouncementList() {
            InitializeComponent();
            Setup();
        }
        private async void Setup() {
            foreach (var item in await AnnouncementRepository.GetAllAsync()) {
                List.Children.Add(new AnnouncementControl(item));
                List.Children.Add(new StackPanel() {
                    Width = 800, Height = 3,
                    Background = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(30),
                });
            }
            foreach (var item in List.Children.OfType<AnnouncementControl>().ToList()) {
                item.MouseLeftButtonDown += (sender, e) => {
                    MessageBox.Show(item._ID.ToString());
                };
            }
        }
        private void NavigateToItemPage(object sender, MouseButtonEventArgs e) {
            MessageBox.Show("123");
        }
    }
}
