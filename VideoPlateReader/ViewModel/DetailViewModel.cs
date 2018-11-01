using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlateReader.Data.Repositories;
using VideoPlateReader.Event;

namespace VideoPlateReader.ViewModel
{
    public class DetailViewModel : ViewModelBase, IDetailViewModel
    {
        private IDataService dataService;
        private MatchedImageLog matchedLog;
        private IEventAggregator _eventAggregator;
        private string originalPic;
        private string matchedPic;
        public DetailViewModel(IDataService dataService,IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<MatchedPlateSelectedEvent>().Subscribe(OnSelectedMatchedPlate);
            this.DataService = dataService;
        }

        public async Task LoadAsync(string id,string path)
        {
            MatchedLog = await DataService.GetByIdAsync(id,path);
        }

   
        public MatchedImageLog MatchedLog { get => matchedLog; set => matchedLog = value; }
        public IDataService DataService { get => dataService; set => dataService = value; }
        public string OriginalPic {
            get => originalPic;
            set
            {
                originalPic = value;
                OnPropertyChanged();

            }
        }
        public string MatchedPic
        {
            get => matchedPic;
            set
            {
                matchedPic = value;
                OnPropertyChanged();
            }
        }

        public void OnSelectedMatchedPlate(MatchedImageLog imageLog)
        {
            if (imageLog != null)
            {
                OriginalPic = imageLog.OriginalImage;
                MatchedPic = imageLog.MatchedImage;
            }
            
        }
    }
}
