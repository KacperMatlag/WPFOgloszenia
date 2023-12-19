using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFOgloszenia.Models {
    public class ApplicationForAdvertisement {
        public int? ID { get; set; }
        public int? UserID { get; set; }
        public int? AnnouncementID { get; set; }
        //Join Models

        public UserModel? User { get; set; }
        public AnnouncementModel? Announcement { get; set; }
    }
}
