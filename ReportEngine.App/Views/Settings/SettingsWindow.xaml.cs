﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using ReportEngine.App.Views.Settings;


namespace ReportEngine.App.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }



        private void SettingsCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //переключалка выбранного окна
            switch (true)
            {
                case true:
        
                default:
                    break;
            }
        }
    }
}
