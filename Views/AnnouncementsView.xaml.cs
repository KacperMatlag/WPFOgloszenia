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
    /// Interaction logic for AnnouncementsView.xaml
    /// </summary>
    public partial class AnnouncementsView : Page {
        private readonly int _announcementID;
        public AnnouncementsView(int id) {
            InitializeComponent();
            _announcementID = id;
            GetCertainModel(id);
        }
        async void GetCertainModel(int id) {
            ApplicationCounter.Content =  $"{await ApplicationForAdvertisementRepository.CountApplications(_announcementID)} Aplikacji";
            AnnouncementModel? announcementModel = await AnnouncementRepository.GetByIdAsync(id);
            DataContext = announcementModel;
            if (await ApplicationForAdvertisementRepository.IsAlreadyApplicating(App.User, _announcementID)) {
                ApplicationButton.Click -= ApplicationRequest_Click;
                ApplicationButton.Click += ApplicationCancel_Click;
                ApplicationButton.Content = "Anuluj Aplikacje";
            }else if (announcementModel?.UserID==App.User?.ID) {
                ApplicationButton.Visibility= Visibility.Collapsed;
            }
        }

        private async void ApplicationRequest_Click(object sender, RoutedEventArgs e) {
            ApplicationButton.Content = "Anuluj Aplikacje";
            ApplicationForAdvertisement application = new() {
                UserID = App.User?.ID,
                AnnouncementID = _announcementID,
            };
            await ApplicationForAdvertisementRepository.InsertAssync(application);
            ApplicationButton.Click -= ApplicationRequest_Click;
            ApplicationButton.Click += ApplicationCancel_Click;
            ApplicationCounter.Content = $"{await ApplicationForAdvertisementRepository.CountApplications(_announcementID)} Aplikacji";
        }
        private async void ApplicationCancel_Click(object sender, RoutedEventArgs e) {
            ApplicationButton.Content = "Aplikuj";
            await ApplicationForAdvertisementRepository.RemoveAsync(App.User,_announcementID);
            ApplicationButton.Click += ApplicationRequest_Click;
            ApplicationButton.Click -= ApplicationCancel_Click;
            ApplicationCounter.Content = $"{await ApplicationForAdvertisementRepository.CountApplications(_announcementID)} Aplikacji";
        }
    }
}
