using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFOgloszenia.Models {
    public class Company {
        public int? ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? NIP { get; set; }
        public string? Location { get; set; }
        public string? ImageLink { get; set; }
    }
}
