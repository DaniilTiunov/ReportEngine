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
using System.Windows.Shapes;
using ReportEngine.App.Enums;

namespace ReportEngine.App.Views.Windows.Dialog
{
    /// <summary>
    /// Логика взаимодействия для TechCardElecrticDialog.xaml
    /// </summary>
    ///



    public partial class TechCardElecrticDialog : Window
    {

        public TechCardElecticDialogResult SelectedOption { get; private set; }

        public TechCardElecrticDialog()
        {
            InitializeComponent();
            SelectedOption = TechCardElecticDialogResult.Cancel;
        }


        private void WithButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedOption = TechCardElecticDialogResult.WithElectric;
            DialogResult = true;

            Close();
        }

        private void WithoutButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedOption = TechCardElecticDialogResult.WithoutElectric;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedOption = TechCardElecticDialogResult.Cancel;
            DialogResult = false;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedOption = TechCardElecticDialogResult.Cancel;
            DialogResult = false;
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
