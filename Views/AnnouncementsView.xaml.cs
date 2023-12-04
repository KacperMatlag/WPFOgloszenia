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
        public AnnouncementsView(int id) {
            InitializeComponent();
            GetCertainModel(id);
        }
        async void GetCertainModel(int id) {
            DataContext=await AnnouncementRepository.GetByIdAsync(id);
        }
    }
}
