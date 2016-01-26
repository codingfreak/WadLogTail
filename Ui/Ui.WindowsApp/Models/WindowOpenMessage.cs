namespace codingfreaks.WadLogTail.Ui.WindowsApp.Models
{
    using System.Windows;
    using System.Windows.Controls;

    using codingfreaks.WadLogTail.Ui.WindowsApp.Enumerations;

    /// <summary>
    /// Can be used to transport the state of a window to any receiver so that this receiver can handle the form appropriate.
    /// </summary>
    public class WindowOpenMessage
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WindowOpenMessage()
        {
        }

        /// <summary>
        /// Constructor including the possibility to init all properties.
        /// </summary>
        /// <param name="window">The window which should be opened.</param>
        /// <param name="windowType">The type of the window.</param>
        /// <param name="windowTarget">The target of window.</param>
        /// <param name="title">The title to of the window because it is not available before it is displayed.</param>
        /// <param name="height">The height of window.</param>
        /// <param name="width">The width of window.</param>
        public WindowOpenMessage(ContentControl window, WindowType windowType, WindowTarget windowTarget, string title, double height, double width)
        {
            WindowType = windowType;
            WindowTarget = windowTarget;
            Content = window.Content;
            DataContext = window.DataContext;
            Title = title;
            Height = height;
            Width = width;
            Window = window;
        }

        #endregion

        #region properties

        public ContentControl Window { get; set; }

        /// <summary>
        /// The content of the original window.
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// The data context of the original window.
        /// </summary>
        public object DataContext { get; set; }

        /// <summary>
        /// The height of window.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// The text of the window title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The width of window.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// The target of window.
        /// </summary>
        public WindowTarget WindowTarget { get; set; }

        /// <summary>
        /// The internal type of the window.
        /// </summary>
        public WindowType WindowType { get; set; }

        #endregion

        #region methods

        /// <summary>
        /// Factory method for convenient initialization.
        /// </summary>
        /// <param name="window">The window which should be opened.</param>
        /// <param name="windowType">The type of the window.</param>
        /// <param name="windowTarget">The target of window.</param>
        /// <param name="title">The title to of the window because it is not available before it is displayed.</param>
        /// <returns>The instance of the message.</returns>
        public static WindowOpenMessage GetInstance(Window window, WindowType windowType, WindowTarget windowTarget, string title)
        {
            return new WindowOpenMessage(window, windowType, windowTarget, title, window.Height, window.Width);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <filterpriority>2</filterpriority>
        public new bool Equals(object obj)
        {
            var p = obj as WindowOpenMessage;
            if (p == null)
            {
                return false;
            }

            return ((WindowOpenMessage)obj).WindowType == WindowType;
        }

        #endregion
    }
}