using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFOgloszenia.Models;

namespace WPFOgloszenia.Controls {
    /// <summary>
    /// Interaction logic for Announcement.xaml
    /// </summary>
    public partial class AnnouncementControl : UserControl {
        public int _ID;
        public AnnouncementControl() {
            InitializeComponent();

        }
        public AnnouncementControl(AnnouncementModel model) {
            InitializeComponent();
            _ID = model.ID;
            Photo.Source = new BitmapImage(new Uri(model.Company.ImageLink));
            ATitle.Content = model.Title;
            Company.Content = model.Company.Name;
            Position.Content = model.Position;
            Position.Content= model.Position;
            Wage.Content = model.MinWage + " - " + model.MaxWage + " zł";
        }
    }
}
