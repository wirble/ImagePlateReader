using Autofac;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VideoPlateReader.Data.Repositories;
using VideoPlateReader.Startup;
using VideoPlateReader.ViewModel;

namespace VideoPlateReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            var bootStrapper = new Bootstrapper();
            var container = bootStrapper.Bootstrap();
            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.Show();
            //dont' need this with bootstrapper
            //var mainWindow = new MainWindow(
            //    new MainViewModel(new DataService()));
            //mainWindow.Show();
        }
    }
}
