using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace P4G_Save_Tool
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            this.Title = "P4G Save Tool " + getRunningVersion();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        private Version getRunningVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
