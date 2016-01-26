namespace codingfreaks.WadLogTail.Ui.WindowsApp.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;

    using codingfreaks.cfUtils.Logic.Portable.Extensions;

    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Threading;

    using codingfreaks.cfUtils.Logic.Azure;
    using codingfreaks.cfUtils.Logic.Wpf.Components;
    using codingfreaks.cfUtils.Logic.Wpf.MvvmLight;

    using GalaSoft.MvvmLight.Command;

    /// <summary>
    /// The view model for the <see cref="MainWindow"/>.
    /// </summary>
    public class MainViewModel : BroadcastViewModelBase
    {
        #region member vars

        private readonly TableHelper<WadLogEntity> Helper = new TableHelper<WadLogEntity>();
        private bool _lastSortAsc;

        private string _lastSortMemberPath = "timestamp";
        private readonly List<WadLogItemViewModel> _rawItems = new List<WadLogItemViewModel>();
        private long _receiveCounter;

        #endregion

        #region constants

        private static readonly object _lock = new object();

        #endregion

        #region constructors and destructors

        public MainViewModel()
        {
            BindingOperations.EnableCollectionSynchronization(Entries, _lock);
            if (!IsInDesignMode)
            {
                // handle event when new results are received from monitoring
                Helper.MonitoringReceivedNewEntries += (s, e) =>
                {
                    StatusText = "Receiving entries...";
                    LastResultReceived = DateTime.Now;
                    Dispatcher.CurrentDispatcher.Invoke(
                        () =>
                        {
                            try
                            {
                                _rawItems.ToList().ForEach(ent => ent.IsNew = false);
                                _rawItems.AddRange(
                                    e.Entries.Select(
                                        entry => new WadLogItemViewModel
                                        {
                                            EntityItem = entry,
                                            IsNew = true,
                                            ReceiveCounter = ++_receiveCounter
                                        }));
                                FilterAndSortEntities("timestamp", false);
                                StatusText = e.Entries.Count() + " entries received.";
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceError(ex.Message);
                                StatusText = "Error";
                                throw;
                            }
                        });
                };
                // define the command for starting and stopping
                StartStopMonitoringCommand = new AutoRelayCommand(
                    () =>
                    {
                        if (IsRunning)
                        {
                            Task.Run(
                                () =>
                                {
                                    StatusText = "Stopping monitoring...";
                                    Helper.StopMonitoringTable();
                                    IsRunning = false;
                                    StatusText = "Monitoring stopped...";
                                });
                            return;
                        }
                        Task.Run(
                            () =>
                            {
                                _receiveCounter = 0;
                                var table = StorageHelper.GetTableReference("WADLogsTable", StorageConnectionString);
                                Helper.StartMonitoringTable(table, 5, TimeSpan.FromDays(2).TotalSeconds);
                            });
                        IsRunning = true;
                    },
                    () => IsRunning || !StorageConnectionString.IsNullOrEmpty());
                // ensure that the StartStopMonitoringCommand is re-evaluated when the StorageConnectionString changes
                StartStopMonitoringCommand.DependsOn(() => StorageConnectionString);
                // init the command which will be bound to grid-sorting-event
                GridSortingCommand = new RelayCommand<DataGridSortingEventArgs>(
                    e =>
                    {
                        var newSortAscending = (e.Column.SortDirection ?? 0) == ListSortDirection.Descending;
                        FilterAndSortEntities(e.Column.SortMemberPath, newSortAscending);
                        e.Column.SortDirection = newSortAscending ? ListSortDirection.Ascending : ListSortDirection.Descending;
                        e.Handled = true;
                    });
                // handle the property-changed event to re-execute filtering when someone enters a text in the filter box                
                PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName.Equals(nameof(FilterText), StringComparison.OrdinalIgnoreCase))
                    {
                        FilterAndSortEntities(_lastSortMemberPath, _lastSortAsc);
                    }
                };
            }
            else
            {
                // fill sample values for the design mode
                StorageConnectionString = "StorageConnectionString";
                Entries = new OptimizedObservableCollection<WadLogItemViewModel>
                {
                    new WadLogItemViewModel
                    {
                        EntityItem = new WadLogEntity
                        {
                            Timestamp = DateTimeOffset.Now,
                            Message = "Test"
                        }
                    },
                    new WadLogItemViewModel
                    {
                        EntityItem = new WadLogEntity
                        {
                            Timestamp = DateTimeOffset.Now,
                            Message = "Test2"
                        }
                    },
                    new WadLogItemViewModel
                    {
                        EntityItem = new WadLogEntity
                        {
                            Timestamp = DateTimeOffset.Now,
                            Message = "Test3"
                        }
                    },
                    new WadLogItemViewModel
                    {
                        EntityItem = new WadLogEntity
                        {
                            Timestamp = DateTimeOffset.Now,
                            Message = "Test4"
                        }
                    }
                };
            }
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

        /// <summary>
        /// Is called when the grid control performs the sorting.
        /// </summary>
        /// <param name="sortMemberPath">The member path for the column that should be sorted.</param>
        /// <param name="ascending"><c>true</c> if the new sort direction should be ascending.</param>
        private void FilterAndSortEntities(string sortMemberPath, bool ascending = true)
        {
            var entries = _rawItems.ToList();
            _lastSortMemberPath = sortMemberPath;
            _lastSortAsc = ascending;
            switch (sortMemberPath.ToLower())
            {
                case "timestamp":
                    entries = ascending
                        ? entries.OrderBy(e => e.EntityItem.Timestamp).ThenBy(e => e.ReceiveCounter).ToList()
                        : entries.OrderByDescending(e => e.EntityItem.Timestamp).ThenByDescending(e => e.ReceiveCounter).ToList();
                    break;
            }
            Entries = new OptimizedObservableCollection<WadLogItemViewModel>();
            if (!FilterText.IsNullOrEmpty())
            {
                entries = entries.Where(item => item.EntityItem.MessageCleaned.ToLower().Contains(FilterText.ToLower())).ToList();
            }
            Entries.AddRange(entries);
        }

        #endregion

        #region properties

        /// <summary>
        /// Entries already read from Azure.
        /// </summary>
        public OptimizedObservableCollection<WadLogItemViewModel> Entries { get; set; } = new OptimizedObservableCollection<WadLogItemViewModel>();

        /// <summary>
        /// The text to filter for.
        /// </summary>
        public string FilterText { get; set; }

        public string FormattedLastResultReceived => LastResultReceived?.ToString("G", CultureInfo.CurrentUICulture);
       
        /// <summary>
        /// Is called when the grids Sorting event is raised.
        /// </summary>
        public RelayCommand<DataGridSortingEventArgs> GridSortingCommand { get; private set; }

        /// <summary>
        /// Indicates whether a background operation is taking place currently.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// The time when the last result was received.
        /// </summary>
        public DateTime? LastResultReceived { get; set; }

        /// <summary>
        /// The caption for the start-/stop button.
        /// </summary>
        public string StartStopCaption => IsRunning ? "Stop" : "Start";

        /// <summary>
        /// Starts or stops a monitoring.
        /// </summary>
        public AutoRelayCommand StartStopMonitoringCommand { get; private set; }

        /// <summary>
        /// The text to display in the status bar.
        /// </summary>
        public string StatusText { get; set; }

        /// <summary>
        /// The Azure storage connection string.
        /// </summary>
        public string StorageConnectionString { get; set; }

        /// <summary>
        /// The title for the window.
        /// </summary>
        public string Title => "codingfreaks Azure Log Table Watcher";

        #endregion
    }
}