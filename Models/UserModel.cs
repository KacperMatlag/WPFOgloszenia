using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFOgloszenia.Models {
    public class UserModel {
        public int? ID { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public int? Permission { get; set; }
        public int? ProfileID { get; set; }
        //Join Model
        public ProfileModel? Profile { get; set; }
    }
}
