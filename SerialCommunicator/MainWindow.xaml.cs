using SerialCommunicator.Utilities;
using SerialCommunicator.ViewModels;
using SerialCommunicator.Windows;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SerialCommunicator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public App CurrentApplication { get; set; }
        public MainWindow(App app)
        {
            CurrentApplication = app;
            InitializeComponent();
        }

        private bool SettingsMenuShowing;
        private void AutoShowMenu(object sender, RoutedEventArgs e)
        {
            if (!SettingsMenuShowing)
            {
                AnimateSettingsMenu(0, 200, TimeSpan.FromSeconds(0.2));
                SettingsMenuShowing = true;
            }
            else
            {
                AnimateSettingsMenu(200, 0, TimeSpan.FromSeconds(0.2));
                SettingsMenuShowing = false;
            }
        }

        public void AnimateSettingsMenu(double from, double to, TimeSpan time)
        {
            DoubleAnimation da = new DoubleAnimation(from, to, time);
            da.AccelerationRatio = 0;
            da.DecelerationRatio = 1;

            SettingsMenu.BeginAnimation(WidthProperty, da);
        }

        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            switch (int.Parse(((MenuItem)sender).Uid))
            {
                // light
                case 0: CurrentApplication.SetTheme(App.Theme.Light); break;
                // colourful light
                case 1: CurrentApplication.SetTheme(App.Theme.ColourfulLight); break;
                // dark
                case 2: CurrentApplication.SetTheme(App.Theme.Dark); break;
                // colourful dark
                case 3: CurrentApplication.SetTheme(App.Theme.ColourfulDark); break;
            }
            e.Handled = true;
        }
    }
}
