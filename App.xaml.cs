using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFOgloszenia.Models;

namespace WPFOgloszenia {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public static UserModel? User = null;
        public static readonly string connectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=Ogloszenia;Integrated Security=True";
    }
}
