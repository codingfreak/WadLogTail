using System;
using System.Linq;

namespace codingfreaks.WadLogTail.Ui.WindowsApp.ViewModel
{
    using codingfreaks.cfUtils.Logic.Wpf.Components;
    using codingfreaks.cfUtils.Logic.Wpf.MvvmLight;
    using codingfreaks.WadLogTail.Ui.WindowsApp.Attributes;
    using codingfreaks.WadLogTail.Ui.WindowsApp.Enumerations;
    using codingfreaks.WadLogTail.Ui.WindowsApp.Helper;
    using codingfreaks.WadLogTail.Ui.WindowsApp.Models;

    using GalaSoft.MvvmLight.Command;

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
                OkCommand = new AutoRelayCommand(
                    () =>
                    {
                        Variables.Settings.Accounts = StoredAccounts.ToList();
                        Variables.Settings.Save();
                    });
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// The command for the OK button.
        /// </summary>
        public RelayCommand OkCommand { get; private set; }

        /// <summary>
        /// The list of storage accounts known so far.
        /// </summary>
        public OptimizedObservableCollection<StorageAccountSetting> StoredAccounts { get; private set; } = new OptimizedObservableCollection<StorageAccountSetting>();

        /// <summary>
        /// A part of the window title.
        /// </summary>
        public override string Title => "Select Storage Account";

        #endregion
    }
}