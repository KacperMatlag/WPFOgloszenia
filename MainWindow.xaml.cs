﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPFOgloszenia.Views;
using WPFOgloszenia.Repositories;
using System.Threading.Tasks;
using System;

namespace WPFOgloszenia {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Setup();
        }
        private async void Setup() {
            CheckTables();
            await Task.Delay(1000);
            NavigationFrame.Navigate(new AnnouncementList());

        }
        private async static void CheckTables() {
            try {
                await CategoryRepository.CreateIfNotExistsAsync();
                await CompanyRepository.CreateIfNotExistsAsync();
                await TypeOfWorkRepository.CreateIfNotExistsAsync();
                await AnnouncementRepository.CreateIfNotExistsAsync();


            } catch (Exception e) {
                MessageBox.Show(e.Message);
                throw;
            }
        }
        private void MinimalizeWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void CloseWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Application.Current.Shutdown();
        }

        private void DragMoveWindow(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void MenuLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (NavigationFrame?.Content is Page page && page.Title != "LoginRegisterPage") {
                NavigationFrame?.Navigate(new LoginRegisterPage());
            }
        }
        private void SelectClickedOption(object sender, MouseButtonEventArgs e) {
            foreach (var item in Menu.Children.OfType<StackPanel>().ToList()) {
                if (item.Children[0] is Label label1) {
                    label1.BorderThickness = new Thickness(0);
                    label1.BorderBrush = Brushes.Transparent;
                }
            }
            if (sender is Label label) {
                label.BorderThickness = new Thickness(0, 0, 0, 2);
                label.BorderBrush = Brushes.Red;
            }
        }

        private void MenuMainPage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (NavigationFrame?.Content is Page p && p.Title != "announcementList") {
                NavigationFrame?.Navigate(new AnnouncementList());
            }
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e) {
            if (sender is Label label)
                label.Foreground = Brushes.Red;
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e) {
            if (sender is Label label)
                label.Foreground = Brushes.Black;
        }
    }
}