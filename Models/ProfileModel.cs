using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFOgloszenia.Models {
    public class ProfileModel {
        public int? ID { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Validate() {
            if (string.IsNullOrEmpty(Name) || Name.Length < 3 || Name.Length > 20) {
                return ("Imię musi mieć od 3 do 20 znaków.");
            }
            if (string.IsNullOrEmpty(Surname) || Surname.Length < 4 || Surname.Length > 30) {
                return ("Nazwisko musi mieć od 3 do 30 znaków.");
            }
            return null;
        }
    }
}
