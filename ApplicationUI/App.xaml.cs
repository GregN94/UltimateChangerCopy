using System;
using System.Windows;
using ApplicationUI.ViewModels;

namespace ApplicationUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (FindResource("Locator") is ViewModelLocator locator)
            {
                locator.Initialize();
            }
        }
    }
}
