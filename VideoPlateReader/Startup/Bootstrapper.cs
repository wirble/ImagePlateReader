using Autofac;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlateReader.Data.Repositories;
using VideoPlateReader.ViewModel;

namespace VideoPlateReader.Startup
{
    //responsible for creating the autofac container
    public class Bootstrapper
    {

        public IContainer Bootstrap() {
            var builder = new ContainerBuilder();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();
            builder.RegisterType<DetailViewModel>().As<IDetailViewModel>();

            //register the mainviewmodel
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            //using DataService whenenver an IDataService is required
            //whenever the IDataService is required, it will create the DataService class
            builder.RegisterType<DataService>().As<IDataService>();
            return builder.Build();
        } 
        
    }
}
