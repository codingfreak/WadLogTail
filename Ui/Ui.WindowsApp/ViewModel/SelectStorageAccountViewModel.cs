using System;
using System.Linq;

namespace codingfreaks.WadLogTail.Ui.WindowsApp.ViewModel
{
    using codingfreaks.WadLogTail.Ui.WindowsApp.Attributes;
    using codingfreaks.WadLogTail.Ui.WindowsApp.Enumerations;

    /// <summary>
    /// The view model for the <see cref="SelectStorageAccountWindow"/>.
    /// </summary>
    [ViewModelDescription("selectStorage", WindowType.SelectStorageAccountView, DataSourceType.All)]
    public class SelectStorageAccountViewModel : BaseViewModel
    {
        #region properties

        /// <summary>
        /// A part of the window title.
        /// </summary>
        public override string Title => "Select Storage Account";

        #endregion
    }
}