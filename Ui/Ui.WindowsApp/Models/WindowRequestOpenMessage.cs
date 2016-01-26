namespace codingfreaks.WadLogTail.Ui.WindowsApp.Models
{
    using codingfreaks.WadLogTail.Ui.WindowsApp.Enumerations;

    /// <summary>
    /// Can be used to transport a request for content of a window of a specific type from the UI to the logic
    /// </summary>
    public class WindowRequestOpenMessage
    {
        #region properties

        /// <summary>
        /// The target of window
        /// </summary>
        public WindowTarget WindowTarget { get; set; }

        /// <summary>
        /// The type of the window the sender wants to be opened.
        /// </summary>
        public WindowType WindowType { get; set; }

        #endregion
    }
}