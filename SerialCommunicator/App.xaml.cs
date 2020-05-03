﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SerialCommunicator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public enum Theme
        {
            Light, ColourfulLight,
            Dark, ColourfulDark
        }
        private ResourceDictionary ThemeDictionary
        {
            // You could probably get it via its name with some query logic as well.
            get { return Resources.MergedDictionaries[0]; }
            set { Resources.MergedDictionaries[0] = value; }
        }

        private void ChangeTheme(Uri uri)
        {
            //ThemeDictionary.MergedDictionaries.Clear();
            ThemeDictionary = new ResourceDictionary() { Source = uri };
            //ThemeDictionary.MergedDictionaries.Insert(0, new ResourceDictionary() { Source = uri });
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mWind = new MainWindow(this);
            mWind.Show();
            this.MainWindow = mWind;
        }

        public void SetTheme(Theme theme)
        {
            string themeName = null;
            switch (theme)
            {
                case Theme.Dark: themeName = "DarkTheme"; break;
                case Theme.Light: themeName = "LightTheme"; break;
                case Theme.ColourfulDark: themeName = "ColourfulDarkTheme"; break;
                case Theme.ColourfulLight: themeName = "ColourfulLightTheme"; break;
            }

            try
            {
                if (!string.IsNullOrEmpty(themeName))
                    ChangeTheme(new Uri($"Themes/{themeName}.xaml", UriKind.Relative));
            }
            catch { }
        }
    }
}
