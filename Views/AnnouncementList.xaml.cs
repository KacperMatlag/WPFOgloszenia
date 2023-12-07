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
        private void Setup() {
            RenderAnnoucementList("");
        }

        private void SearchBarSearch_Click(object sender, RoutedEventArgs e) {
            RenderAnnoucementList(SearchInput.Text);
        }

        private async void RenderAnnoucementList(string filter) {
            List.Children.Clear();

            foreach (var item in await AnnouncementRepository.GetAllAsync(filter)) {
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
                    if (Application.Current.MainWindow is MainWindow window) {
                        window.NavigationFrame.Navigate(new AnnouncementsView(item._ID));
                    }
                };
            }
        }
    }
}
