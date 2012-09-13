using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JoyOs.BusinessLogic.Pluginable;

namespace UcMediaViewer
{
    /// <summary>
    /// Interaction logic for VideoList.xaml
    /// </summary>
    public partial class MediaViewer : IListerPlugin
    {
        public static int IndexOfCurImg = -1;
        private const int DefSize = 160;

        private double SizeImage { get; set; }

        private readonly List<MediaItem> _sourceData = new List<MediaItem>();

        public MediaViewer()
        {
            InitializeComponent();

            SizeImage = DefSize;

            Menu = new List<MenuItem>(mainMenu.Items.Cast<MenuItem>());

            mainMenu.Items.Clear();
            mainDockPanel.Children.Remove(mainMenu);

            LoadImages(UvStartDirectory);
            LoadList();
        }

        private void RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var item = sender as RadioButton;
            if (item.Content == null) return;
            switch (item.Content.ToString())
            {
                case "Список":
                    if (item.IsChecked == true)
                        LoadList();
                    break;
                case "Превью":
                    if (item.IsChecked == true)
                        WrapList();
                    break;
            }
        }

        private void OpenMediaPluginClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Utils", "ump", "umplayer.exe"));
            }
            catch
            {
                Process.Start("mpc-hc.exe", "");
            }
        }

        private static void MediaItemClick(object sender, RoutedEventArgs e)
        {
            var fileName = ((StackPanel)sender).Tag.ToString();
            try
            {
                Process.Start(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Utils", "ump", "umplayer.exe"),
                    "\"" + fileName + "\" -play");
            }
            catch
            {
                Process.Start("mpc-hc.exe", " /play " + fileName);
            }
        }
    }
}