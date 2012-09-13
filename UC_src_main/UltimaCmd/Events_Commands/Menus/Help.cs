using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JoyOs.BusinessLogic.Utils;

namespace UltimaCmd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        void HelpOnExecute(object sender, ExecutedRoutedEventArgs args)
        {
            MessageBox.Show(
                        "Bla-bla-bla - Егор и Юля, Димыч, 2011",
                        "Авторы", MessageBoxButton.OK, MessageBoxImage.Information
                       );
        }

        private void HelpItemClick(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Tag.ToString())
            {
                case "0":
                    break;
                case "1":
                    break;
                case "2":
                    break;
                case "3":
                    break;

                case "4":
                    HelpSystem.ShowAboutBox();
                    break;
            }
        }        
    }
}