namespace codingfreaks.WadLogTail.Ui.WindowsApp.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Threading;

    using codingfreaks.cfUtils.Logic.Azure;
    using codingfreaks.cfUtils.Logic.Portable.Extensions;
    using codingfreaks.cfUtils.Logic.Wpf.Components;
    using codingfreaks.cfUtils.Logic.Wpf.MvvmLight;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    /// <summary>
    /// The view model for the <see cref="MainWindow"/>.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region member vars

        private readonly TableHelper<WadLogEntity> Helper = new TableHelper<WadLogEntity>();

        private static object _lock = new object();

        public bool IsRunning { get; private set; }

        #endregion

        #region constructors and destructors

        public MainViewModel()
        {
            BindingOperations.EnableCollectionSynchronization(Entries, _lock);
            if (!IsInDesignMode)
            {
                Helper.EntriesReceived += (s, e) =>
                {
                    Dispatcher.CurrentDispatcher.Invoke(
                        () =>
                        {
                            try
                            {
                                Entries.All(entry => entry.IsNew = false);
                                Entries.AddRange(
                                    e.Entries.Select(
                                        entry => new WadLogItemViewModel()
                                        {
                                            EntityItem = entry,
                                            IsNew = true
                                        }));  
                                SortEntities("timestamp", false);                              
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                        });
                };
                StartStopMonitoringCommand = new AutoRelayCommand(
                    () =>
                    {
                        if (IsRunning)
                        {
                            Helper.StopMonitoringTable();
                            IsRunning = false;
                            return;
                        }
                        Task.Run(
                            () =>
                            {
                                var table = StorageHelper.GetTableReference("WADLogsTable", StorageConnectionString);
                                Helper.StartMonitoringTable(table, 5, TimeSpan.FromDays(2).TotalSeconds);
                            });
                        IsRunning = true;
                    },
                    () => IsRunning || !StorageConnectionString.IsNullOrEmpty());
                GridSortingCommand = new RelayCommand<DataGridSortingEventArgs>(
                    (e) =>
                    {
                        var newSortAscending = (e.Column.SortDirection ?? 0) == ListSortDirection.Descending;
                        SortEntities(e.Column.SortMemberPath, newSortAscending);
                        e.Column.SortDirection = newSortAscending ? ListSortDirection.Ascending : ListSortDirection.Descending;
                        e.Handled = true;
                    });
            }
            else
            {
                StorageConnectionString = "StorageConnectionString";
            }
        }

        private void SortEntities(string sortMemberPath, bool ascending = true)
        {
            var entries = Entries.ToList();
            Entries.Clear();
            switch (sortMemberPath.ToLower())
            {
                case "timestamp":
                    entries = ascending ? entries.OrderBy(e => e.EntityItem.Timestamp).ToList() : entries.OrderByDescending(e => e.EntityItem.Timestamp).ToList();
                    break;                    
            }
            Entries.AddRange(entries);
        }

        #endregion

        #region methods

        public override void Cleanup()
        {
            try
            {
                Helper.StopMonitoringTable();
            }
            catch
            {
            }
            base.Cleanup();
        }

        #endregion

        #region properties

        /// <summary>
        /// The caption for the start-/stop button.
        /// </summary>
        public string StartStopCaption => IsRunning ? "Stop" : "Start";

        /// <summary>
        /// The text to display in the status bar.
        /// </summary>
        public string StatusText { get; set; }

        /// <summary>
        /// Starts or stops a monitoring.
        /// </summary>
        public RelayCommand StartStopMonitoringCommand { get; private set; }

        /// <summary>
        /// Is called when the grids Sorting event is raised.
        /// </summary>
        public RelayCommand<DataGridSortingEventArgs> GridSortingCommand { get; private set; }

        /// <summary>
        /// Entries already read from Azure.
        /// </summary>
        public OptimizedObservableCollection<WadLogItemViewModel> Entries { get; set; } = new OptimizedObservableCollection<WadLogItemViewModel>();

        /// <summary>
        /// The Azure storage connection string.
        /// </summary>
        public string StorageConnectionString { get; set; }

        /// <summary>
        /// The title for the window.
        /// </summary>
        public string Title => "Azure Table Watcher";

        #endregion
    }
}