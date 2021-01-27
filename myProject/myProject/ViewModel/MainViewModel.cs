using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Prism.Commands;
using TreeSize;

namespace myProject.ViewModel
{
    public class MainViewModel : Observable
    {
        public ICommand SelectDirectoryCommand => _selectDirectoryCommand;
        public ICommand ViewSizeCommand => _viewSizeCommand;
        public ICommand ViewAllocatedCommand => _viewAllocatedCommand;
        public ICommand ViewCountCommand => _viewCountCommand;
        public ICommand ViewPercentCommand => _viewPercentCommand;
        public ICommand ExitCommand => _exitCommand;
        public ICommand StopScanCommand => _stopScanCommand;
        public ICommand RefreshCommand => _refreshCommand;
        private string Path;
        private bool isFileSize;
        private bool isFileAllocated;
        private bool isFileCout;
        private bool isFilePercent;
        private string _visible;
        private bool _stopScanEnabled;
        private bool _refreshEnabled;
        private bool _selectDirectoryEnabled;
        private readonly DelegateCommand _selectDirectoryCommand;
        private readonly DelegateCommand _viewSizeCommand;
        private readonly DelegateCommand _viewAllocatedCommand;
        private readonly DelegateCommand _viewCountCommand;
        private readonly DelegateCommand _viewPercentCommand;
        private readonly DelegateCommand _exitCommand;
        private readonly DelegateCommand _refreshCommand;
        private readonly DelegateCommand _stopScanCommand;
        private ObservableCollection<List<File>> list;
        private FileSizeViewModel fileSizeViewModel { get; set; }
        private FilePercentViewModel filePercentViewModel { get; set; }
        private FileCountViewModel fileCountViewModel { get; set; }
        private FileAllocatedViewModel fileAllocatedViewModel { get; set; }
        public MainViewModel()  
        {
            list = new ObservableCollection<List<File>>();
            _selectDirectoryCommand = new DelegateCommand(SelectDirectory);
            _viewSizeCommand = new DelegateCommand(FileSizeView);
            _viewAllocatedCommand = new DelegateCommand(FileAllocatedView);
            _viewCountCommand = new DelegateCommand(FileCountView);
            _viewPercentCommand = new DelegateCommand(FilePercentView);
            _exitCommand = new DelegateCommand(Exit);
            _refreshCommand = new DelegateCommand(Refresh);
            _stopScanCommand = new DelegateCommand(StopScan);
            isFileSize = true;
            isFileAllocated = false;
            isFileCout = false;
            isFilePercent = false;
            DisplayProgressBar = "Hidden";
            StopScanEnabled = false;
            RefreshEnabled = false;
            SelectDirectoryEnabled = true;
        }
        public string DisplayProgressBar
        {
            get { return _visible; }
            set { _visible = value; OnPropertyChanged("DisplayProgressBar"); }
        }
        public bool StopScanEnabled
        {
            get { return _stopScanEnabled; }
            set { _stopScanEnabled = value; OnPropertyChanged("StopScanEnabled"); }
        }
        public bool RefreshEnabled
        {
            get { return _refreshEnabled; }
            set { _refreshEnabled = value; OnPropertyChanged("RefreshEnabled"); }
        }
        public bool SelectDirectoryEnabled
        {
            get { return _selectDirectoryEnabled; }
            set { _selectDirectoryEnabled = value; OnPropertyChanged("SelectDirectoryEnabled"); }
        }
        private void Refresh()
        {
            Task treeSize = new Task(() => TreeSize());
            treeSize.Start();
            DisplayProgressBar = "Visible";
            StopScanEnabled = true;
            RefreshEnabled = false;
            SelectDirectoryEnabled = false;
        }
        private void Exit()
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void SelectDirectory()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if(Path!=null)
                    list[0] = null;
                DisplayProgressBar = "Visible";
                StopScanEnabled = true;
                SelectDirectoryEnabled = false;
                StopScanEnabled = true;
                Path = fbd.SelectedPath;
                Task treeSize = new Task(() => TreeSize());
                treeSize.Start();
            }
        }
        private void TreeSize()
        {
            FillFile fillFile = new FillFile(Path);
            fillFile.GetFile();
            List<File> files = new List<File> { };
            files.Add(fillFile.file);

            fileSizeViewModel = new FileSizeViewModel(files);
            fileCountViewModel = new FileCountViewModel(files);
            fileAllocatedViewModel = new FileAllocatedViewModel(files);
            filePercentViewModel = new FilePercentViewModel(files);

            if (isFileSize)
                FileSizeView();
            else if (isFileAllocated)
                FileAllocatedView();
            else if (isFileCout)
                FileCountView();
            else
                FilePercentView();
            DisplayProgressBar = "Hidden";
            StopScanEnabled = false;
            RefreshEnabled = true;
            SelectDirectoryEnabled = true;
        }
        private void FileSizeView()
        {
            isFileSize = true;
            isFileAllocated = false;
            isFileCout = false;
            isFilePercent = false;
            if (Path != null)
            {
                if (list.Count == 0)
                    App.Current.Dispatcher.Invoke(new Action(() => list.Add(fileSizeViewModel.Files)));
                else
                    App.Current.Dispatcher.Invoke(new Action(() => list[0] = (fileSizeViewModel.Files)));
            }
        }
        private void FilePercentView()
        {
            isFileSize = false;
            isFileAllocated = false;
            isFileCout = false;
            isFilePercent = true;
            if (Path != null)
            {
                if (list.Count == 0)
                    App.Current.Dispatcher.Invoke(new Action(() => list.Add(filePercentViewModel.Files)));
                else
                    App.Current.Dispatcher.Invoke(new Action(() => list[0] = (filePercentViewModel.Files)));
            }
        }
        private void FileAllocatedView()
        {
            isFileSize = false;
            isFileAllocated = true;
            isFileCout = false;
            isFilePercent = false;
            if(Path!=null)
            {
                if (list.Count == 0)
                    App.Current.Dispatcher.Invoke(new Action(() => list.Add(fileAllocatedViewModel.Files)));
                else
                    App.Current.Dispatcher.Invoke(new Action(() => list[0] = (fileAllocatedViewModel.Files)));
            }
        }
        private void FileCountView()
        {
            isFileSize = false;
            isFileAllocated = false;
            isFileCout = true;
            isFilePercent = false;
            if (Path != null)
            {
                if (list.Count == 0)
                    App.Current.Dispatcher.Invoke(new Action(() => list.Add(fileCountViewModel.Files)));
                else
                    App.Current.Dispatcher.Invoke(new Action(() => list[0] = (fileCountViewModel.Files)));
            }
        }
        private void StopScan()
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
            cancelTokenSource.Cancel();
            list[0] = null;
            DisplayProgressBar = "Hidden";
            StopScanEnabled = false;
            RefreshEnabled = false;
            SelectDirectoryEnabled = true;
            Path = null;
        }
        public ObservableCollection<List<File>> Files
        {
            get { return list; }
            set
            {
                list = value;
                OnPropertyChanged("Files");
            }
        }
    }
}
