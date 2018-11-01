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
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private ProcessMediaFolder media = new ProcessMediaFolder(null, null, new EventAggregator());
        private string selectedFolder;
        private ProcessMedia selectedMedia;
        private IDataService _dataService;
        private IEventAggregator _eventAggregator;
        private MatchedImageLog selectedLogReader;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private IList<MatchedImageLog> curLog = new List<MatchedImageLog>();
        public NavigationViewModel(IDataService dataService, IEventAggregator eventAggregator)
        {
            Media = new ObservableCollection<ProcessMedia>();
            LogReader = new ObservableCollection<MatchedImageLog>();
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<MatchedPlateEvent>().Subscribe(OnMatchedPlateEvent);
            
            MonitorFolder = new DelegateCommand(OnMonitorFolder, OnCanMonitorFolder);
        }

        private async void OnMatchedPlateEvent(Task obj)
        {

            await this.LoadAsync();
        }

        private void OnMonitorFolder()
        {
            WPFFolderBrowser.WPFFolderBrowserDialog open = new WPFFolderBrowser.WPFFolderBrowserDialog("Select Folder");
            open.InitialDirectory = ProcessMedia.AssemblyDirectory;
            open.ShowDialog();

            try
            {
                logger.Trace($" btnSelectedFolder_Click(object sender, RoutedEventArgs e){DateTime.Now.ToString()} BEFORE");
                //lblselectedFolder.Content = open.FileName;
                this.media = new ProcessMediaFolder(open.FileName, "us", this._eventAggregator);
                this.SelectedFolder = open.FileName;
                //WaitAll - execute the first one in the list and doesn't execute the second one until the first is done
                //var log = this.ReadLogAsync(media.InitialMatchedImageLog);
                var images = media.ProcessAndMonitorFolderAsync();
                IList<Task> tasks = new List<Task>();
                //tasks.Add(log);
                tasks.Add(images);
                Task.WhenAll(tasks);

                logger.Trace($" btnSelectedFolder_Click(object sender, RoutedEventArgs e){DateTime.Now.ToString()} AFTER");

            }
            catch (InvalidOperationException ex)
            {//not sure how to check to make sure user actually selected a folder, if user cancel, once use FileName get exception 
                logger.Error(ex, "btnSelectedFolder_Click: InvalidOperationException");
            }
            catch (OperationCanceledException ex)
            {
                logger.Error(ex, "btnSelectedFolder_Click: OperationCanceledException");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "btnSelectedFolder_Click: all exceptions");
            }

            //foreach (var file in processedImages)
            //{
            //    //count = 0;//reset
            //    //fileList.Add(file.FilePath);
            //    string matchedPlates = string.Empty;
            //    foreach (var plate in file.MatchedPlates)
            //    {
            //        matchedPlates += plate;

            //    }
            //    //PlateList.Items.Add(matchedPlates);
            //    //PlateList.Items.Add(file.MatchedPlates.Take(1).ToList());
            //}
        }

        private bool OnCanMonitorFolder()
        {
            //TODO: check if oncanmonitorfolder is valid
            return true;
        }

        public async Task LoadAsync()
        {
            var data = await _dataService.GetAllLogReader(media.InitialMatchedImageLog);
            LogReader.Clear();
            foreach (var lr in data)
            {
                LogReader.Add(lr);

            }

        }

        public ObservableCollection<MatchedImageLog> LogReader { get; set; }
        public ObservableCollection<ProcessMedia> Media { get; set; }




        public MatchedImageLog SelectedLogReader
        {
            get
            {
                
                return selectedLogReader;


            }
            set
            {
                selectedLogReader = value;
                OnPropertyChanged(); //notify when it has changed
                _eventAggregator.GetEvent<MatchedPlateSelectedEvent>().Publish(selectedLogReader);
            }
        }

        public ProcessMedia SelectedMedia
        {
            get { return selectedMedia; }
            set
            {
                selectedMedia = value;
                OnPropertyChanged(); // OnPropertyChanged(nameof(SelectedMedia));

            }
        }

        public ICommand MonitorFolder { get; set; }
        public string SelectedFolder {
            get => selectedFolder;
            set
            {
                selectedFolder = value;
                OnPropertyChanged();
            }
        }
    }


}
