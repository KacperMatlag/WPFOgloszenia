using System.Windows.Controls;
using WPFOgloszenia.Repositories;
using WPFOgloszenia.Models;
using System.Threading.Tasks;

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
            GetAnnouncementModel(model.ID);
        }
        private async void GetAnnouncementModel(int id) {
           DataContext= await AnnouncementRepository.GetByIdAsync(id);
        }
    }
}
