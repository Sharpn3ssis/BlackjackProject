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

namespace BlackjackProject
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public MainMenu()
        {
            InitializeComponent(); //do toho slideru nemohu dát desetinná čísla takže to settuju zde
            WinSlider.Value = 1.5;
            WinValue.Content = ($"{1.5}X (Def: 1.5X)");
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(); 
            mainWindow.Show();
            this.Close();
        }
        //nastavení které se ukládá do třídy Settings
        private void BalanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = BalanceSlider.Value;
            BalanceValue.Content = ($"{value.ToString()} CZK (Def: 1000 CZK)"); 
            Settings.StartingBalance = value;
        }
        private void WinSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = WinSlider.Value;
            WinValue.Content = ($"{value.ToString()}X (Def: 1.5X)");
            Settings.WinMultiply = value;
        }
        private void Win21Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = Win21Slider.Value;
            Win21Value.Content = ($"{value.ToString()}X (Def: 2X)");
            Settings.WinMultiply21 = value;
        }
        private void SetButton(object sender, RoutedEventArgs e)
        {
            Settings.PlayerName = PlayerName.Text;
            PlayerSet.Visibility = Visibility.Collapsed;
        
        }

        private void PlayerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlayerSet.Visibility = Visibility.Visible;
        }

        
    }
    
}
