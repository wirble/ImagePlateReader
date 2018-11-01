using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VideoPlateReader.Data.Repositories;
using VideoPlateReader.Event;

namespace VideoPlateReader.ViewModel
{
    public class MainViewModel:ViewModelBase
    {
        public INavigationViewModel NavigationViewModel { get; }
        public IDetailViewModel DetailViewModel { get; }
        public MainViewModel(INavigationViewModel navigationViewModel,IDetailViewModel detailViewModel)

        {
            NavigationViewModel = navigationViewModel;
            DetailViewModel = detailViewModel;
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

    }


}
