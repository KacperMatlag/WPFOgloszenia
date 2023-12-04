using System.ComponentModel.DataAnnotations;

namespace WPFOgloszenia.Models {
    public class AnnouncementModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public int CompanyID { get; set; }
        public string Position { get; set; }
        public decimal MinWage { get; set; }
        public decimal MaxWage { get; set; }
        //Join Models
        public CategoryModel Category { get; set; }
        public Company Company { get; set; }
    }
}
