namespace codingfreaks.WadLogTail.Ui.WindowsApp.ViewModel
{
    using GalaSoft.MvvmLight.Ioc;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        #region constructors and destructors

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SelectStorageAccountViewModel>();
        }

        #endregion

        #region methods

        public static void Cleanup()
        {            
        }

        #endregion

        #region properties

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public SelectStorageAccountViewModel SelectStorageAccount => ServiceLocator.Current.GetInstance<SelectStorageAccountViewModel>();

        #endregion
    }
}