using System;
using System.Linq;

namespace codingfreaks.WadLogTail.Ui.WindowsApp.ViewModel
{
    using System.Diagnostics;

    using codingfreaks.cfUtils.Logic.Portable.Extensions;
    using codingfreaks.cfUtils.Logic.Wpf.Components;
    using codingfreaks.cfUtils.Logic.Wpf.MvvmLight;
    using codingfreaks.WadLogTail.Ui.WindowsApp.Attributes;
    using codingfreaks.WadLogTail.Ui.WindowsApp.Enumerations;
    using codingfreaks.WadLogTail.Ui.WindowsApp.Helper;
    using codingfreaks.WadLogTail.Ui.WindowsApp.Models;

    using GalaSoft.MvvmLight.Command;
    using System.Windows;

    using GalaSoft.MvvmLight.Messaging;

    /// <summary>
    /// The view model for the <see cref="SelectStorageAccountWindow"/>.
    /// </summary>
    [ViewModelDescription("selectStorage", WindowType.SelectStorageAccountView, DataSourceType.All)]
    public class SelectStorageAccountViewModel : BaseViewModel
    {
        #region constructors and destructors

        public SelectStorageAccountViewModel()
        {
            if (!IsInDesignMode)
            {
                // get settings from variables if any
                if (Variables.Settings.Accounts.Any())
                {
                    StoredAccounts.AddRange(Variables.Settings.Accounts);
                }
                // create command for OK-button
                OkCommand = new AutoRelayCommand<Window>(
                    win =>
                    {
                        Variables.Settings.Accounts = StoredAccounts.ToList();
                        Variables.Settings.Save();
                        Messenger.Default.Send(new SettingsChangedMessage());
                        win?.Close();
                    });
                // create command for Save-button
                AddNewCommand = new AutoRelayCommand(
                    () =>
                    {
                        var item = new StorageAccountSetting();
                        StoredAccounts.Add(item);
                        CurrentSelectedItem = item;
                    });
                // create command for Remove-button
                RemoveCommand = new AutoRelayCommand(
                    () =>
                    {
                        StoredAccounts.Remove(CurrentSelectedItem);                        
                    }, 
                    () => CanEdit);
                this.PropertyChanged += (s, e) => Trace.WriteLine(e.PropertyName);
            }
            else
            {
                StoredAccounts = new OptimizedObservableCollection<StorageAccountSetting>
                {
                    new StorageAccountSetting()
                    {
                        Account = "Testaccount",
                        Key = "Hello362736273=="
                    },
                    new StorageAccountSetting()
                    {
                        Account = "Testaccount2",
                        Key = "fsdfdsf33fv33=="
                    }
                };
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// The command for the OK button.
        /// </summary>
        public RelayCommand<Window> OkCommand { get; private set; }

        /// <summary>
        /// The command for the Add button.
        /// </summary>
        public RelayCommand AddNewCommand { get; private set; }

        /// <summary>
        /// The command for the Remove button.
        /// </summary>
        public RelayCommand RemoveCommand { get; private set; }

        /// <summary>
        /// The list of storage accounts known so far.
        /// </summary>
        public OptimizedObservableCollection<StorageAccountSetting> StoredAccounts { get; private set; } = new OptimizedObservableCollection<StorageAccountSetting>();

        /// <summary>
        /// A part of the window title.
        /// </summary>
        public override string Title => "Select Storage Account";

        /// <summary>
        /// The item currently in selection.
        /// </summary>
        public StorageAccountSetting CurrentSelectedItem { get; set; }

        /// <summary>
        /// Indicates whether user can edit something.
        /// </summary>
        public bool CanEdit => CurrentSelectedItem != null;

        #endregion
    }
}