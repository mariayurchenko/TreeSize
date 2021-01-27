using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using myProject;
using myProject.ViewModel;

namespace myProject.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainViewModel main = new MainViewModel();
            DataContext = main;
        }
        private void ViewSize(object sender, RoutedEventArgs e)
        {
            if (viewSize.Background != Brushes.SteelBlue)
            {
                viewSize.Background = Brushes.SteelBlue;
                viewAllocatedSpace.Background = Brushes.LightSteelBlue;
                viewFileCount.Background = Brushes.LightSteelBlue;
                viewPercent.Background = Brushes.LightSteelBlue;
            }
        }
        private void ViewAllocatedSpace(object sender, RoutedEventArgs e)
        {
            if(viewAllocatedSpace.Background != Brushes.SteelBlue)
            {
                viewSize.Background = Brushes.LightSteelBlue;
                viewAllocatedSpace.Background = Brushes.SteelBlue;
                viewFileCount.Background = Brushes.LightSteelBlue;
                viewPercent.Background = Brushes.LightSteelBlue;
            }
        }
        private void ViewFileCount(object sender, RoutedEventArgs e)
        {
            if(viewFileCount.Background != Brushes.SteelBlue)
            {
                viewSize.Background = Brushes.LightSteelBlue;
                viewAllocatedSpace.Background = Brushes.LightSteelBlue;
                viewFileCount.Background = Brushes.SteelBlue;
                viewPercent.Background = Brushes.LightSteelBlue;
            }
        }
        private void ViewPercent(object sender, RoutedEventArgs e)
        {
            if(viewPercent.Background != Brushes.SteelBlue)
            {
                viewSize.Background = Brushes.LightSteelBlue;
                viewAllocatedSpace.Background = Brushes.LightSteelBlue;
                viewFileCount.Background = Brushes.LightSteelBlue;
                viewPercent.Background = Brushes.SteelBlue;
            }
        }
    }
}
