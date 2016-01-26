namespace codingfreaks.WadLogTail.Ui.WindowsApp.Attributes
{
    using System;

    using codingfreaks.WadLogTail.Ui.WindowsApp.Enumerations;

    /// <summary>
    /// If applied to a ViewModel this type will supply descriptive informations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewModelDescriptionAttribute : Attribute
    {
        #region constructors and destructors

        /// <summary>
        /// Constructor to define all properties.
        /// </summary>
        /// <param name="serviceIocKey">Unique key of service to subscribe the view model.</param>
        /// <param name="windowType">Enumeration for a single window.</param>
        /// <param name="defaultDataSourceType">Enumeration for data source type of a view.</param>
        public ViewModelDescriptionAttribute(string serviceIocKey, WindowType windowType, DataSourceType defaultDataSourceType)
        {
            ServiceIocKey = serviceIocKey;
            WindowType = windowType;
            DefaultDataSourceType = defaultDataSourceType;
        }

        #endregion

        #region properties

        /// <summary>
        /// The default data source type which shows on which messenger events to subscribe.
        /// </summary>
        public DataSourceType DefaultDataSourceType { get; private set; }

        /// <summary>
        /// The unique key of the service to subscribe the view model to.
        /// </summary>
        public string ServiceIocKey { get; private set; }

        /// <summary>
        /// Indicator to show which window this view model is responsive for.
        /// </summary>
        public WindowType WindowType { get; private set; }

        #endregion
    }
}