using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VideoPlateReader.ViewModel;

namespace VideoPlateReader
{


    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += MainWindow_Loaded;
            
        }
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
        }
    }

    ///// <summary>
    ///// Interaction logic for MainWindow.xaml
    ///// </summary>
    //public partial class MainWindow : Window
    //{
    //    private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    //    private IList<MatchedImageLog> curLog = new List<MatchedImageLog>();
    //    private ProcessMediaFolder media;
    //    public MainWindow()
    //    {
    //        InitializeComponent();

    //    }

    //    private async void btnSelectedFolder_Click(object sender, RoutedEventArgs e)
    //    {
    //        WPFFolderBrowser.WPFFolderBrowserDialog open = new WPFFolderBrowser.WPFFolderBrowserDialog("Select Folder");
    //        open.InitialDirectory = ProcessMedia.AssemblyDirectory;
    //        open.ShowDialog();

    //        try {
    //            logger.Trace($" btnSelectedFolder_Click(object sender, RoutedEventArgs e){DateTime.Now.ToString()} BEFORE");
    //            lblselectedFolder.Content = open.FileName;
    //            this.media = new ProcessMediaFolder(open.FileName, (usRegion.IsChecked ?? true) ? "us" : "eu");

    //            //WaitAll - execute the first one in the list and doesn't execute the second one until the first is done
    //            //var log = this.ReadLogAsync(media.InitialMatchedImageLog);
    //            var images = media.ProcessAndMonitorFolderAsync();
    //            IList<Task> tasks = new List<Task>();
    //            //tasks.Add(log);
    //            tasks.Add(images);
    //            await Task.WhenAll(tasks);
                
    //            logger.Trace($" btnSelectedFolder_Click(object sender, RoutedEventArgs e){DateTime.Now.ToString()} AFTER");

    //        }
    //        catch (InvalidOperationException ex) {//not sure how to check to make sure user actually selected a folder, if user cancel, once use FileName get exception 
    //            logger.Error(ex, "btnSelectedFolder_Click: InvalidOperationException");
    //        }
    //        catch (OperationCanceledException ex)
    //        {
    //            logger.Error(ex, "btnSelectedFolder_Click: OperationCanceledException");
    //        }
    //        catch(Exception ex)
    //        {
    //            logger.Error(ex,"btnSelectedFolder_Click: all exceptions");
    //        }

    //        //foreach (var file in processedImages)
    //        //{
    //        //    count = 0;//reset
    //        //    fileList.Add(file.FilePath);
    //        //    string matchedPlates = string.Empty;
    //        //    foreach (var plate in file.MatchedPlates)
    //        //    {
    //        //        matchedPlates += plate;

    //        //    }
    //        //    PlateList.Items.Add(matchedPlates);
    //        //    //PlateList.Items.Add(file.MatchedPlates.Take(1).ToList());
    //        //}
    //    }

    //    private async void btnRefreshMatchedLog_Click(object sender, RoutedEventArgs e)
    //    {

    //        try
    //        {
    //            logger.Trace($" btnRefreshMatchedLog_Click(object sender, RoutedEventArgs e){DateTime.Now.ToString()} BEFORE");
    //            if (this.media != null)
    //            {
    //                var log = this.ReadLogRefreshAsync(this.media.InitialMatchedImageLog);
    //                //var images = media.ProcessAndMonitorFolderAsync();
    //                IList<Task> tasks = new List<Task>();
    //                tasks.Add(log);
    //                //tasks.Add(images);
    //                await Task.WhenAll(tasks);
    //            }

    //            logger.Trace($" btnRefreshMatchedLog_Click(object sender, RoutedEventArgs e){DateTime.Now.ToString()} AFTER");

    //        }
    //        catch (InvalidOperationException ex)
    //        {//not sure how to check to make sure user actually selected a folder, if user cancel, once use FileName get exception 
    //            logger.Error(ex, "btnRefreshMatchedLog_Click: InvalidOperationException");
    //        }
    //        catch (OperationCanceledException ex)
    //        {
    //            logger.Error(ex, "btnRefreshMatchedLog_Click: OperationCanceledException");
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.Error(ex, "btnRefreshMatchedLog_Click: all exceptions");
    //        }

    //    }
    //    public async Task ReadLogLoopAsync(string path)
    //    {
    //        while (true)
    //        {
    //            logger.Trace($" ReadLogAsync(string path): {path}");
    //            var diff = MatchedImageLogReader.LogDiff(path, curLog);
    //            if (diff.Count != 0)
    //            { //when 0 no differences in log files
    //                foreach(var d in diff)
    //                {
    //                    this.curLog.Add(d);//need to add in the diff with the curlog
    //                }
                    
    //            }
    //            else
    //            {
    //                await Task.Delay(5000);
    //            }
    //            //foreach (var l in diff) //add only the differences so we don't have to clear and readd the entire log
    //            //{
    //                //PlateList.Items.Add(l.Plate);
    //                //fileList.Items.Add(l.MatchedImage);
                    
    //            //Application.Current.Dispatcher.Invoke((Action)(() =>
    //            //{
    //                //MatchedPlates.Items.Clear();
    //            MatchedPlates.ItemsSource = this.curLog;
    //            MatchedPlates.Items.Refresh();
    //            //}));
    //            //}
    //        }
            
    //    }
    //    public async Task ReadLogRefreshAsync(string path)
    //    {

    //        logger.Trace($" ReadLogAsync(string path): {path}");
    //        var diff = MatchedImageLogReader.LogDiff(path, curLog);
    //        if (diff.Count != 0)
    //        { //when 0 no differences in log files
    //            foreach (var d in diff)
    //            {
    //                this.curLog.Add(d);//need to add in the diff with the curlog
    //            }

    //        }
    //        else
    //        {
    //            await Task.Delay(5000);
    //        }

    //        MatchedPlates.ItemsSource = this.curLog;
    //        MatchedPlates.Items.Refresh();

    //    }
    //    private void resetControls()
    //    {
    //        picOriginal.Source = null;
    //        picLicensePlate.Source = null;
    //        //PlateList.Items.Clear();
    //        //fileList.Items.Clear();
    //    }

    //    private void Button_PreviewClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    //    {
    //        var log = MatchedPlates.SelectedItem as MatchedImageLog;
           
            
    //        picOriginal.Source = new BitmapImage(new Uri(log.OriginalImage));
    //        picLicensePlate.Source = new BitmapImage(new Uri(log.MatchedImage));
    //    }
    //}
}
