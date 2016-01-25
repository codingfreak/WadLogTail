using System;
using System.Linq;

namespace codingfreaks.WadLogTail.Ui.WindowsApp.ViewModel
{
    using System.Globalization;

    using codingfreaks.cfUtils.Logic.Azure;

    /// <summary>
    /// The model of one row in the grid in <see cref="MainWindow"/>.
    /// </summary>
    public class WadLogItemViewModel
    {
        #region properties

        /// <summary>
        /// The item sent by the monitoring.
        /// </summary>
        public WadLogEntity EntityItem { get; set; }

        /// <summary>
        /// A string representing <see cref="LocalTimestamp"/> in a formatted way.
        /// </summary>
        public string FormattedLocalTimestamp => LocalTimestamp.ToString("G", CultureInfo.CurrentUICulture);

        /// <summary>
        /// <c>true</c> if this item was added in the last event received from the monitoring.
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// The local timestamp.
        /// </summary>
        public DateTime LocalTimestamp => EntityItem.EventDateTime.ToLocalTime();

        #endregion
    }
}