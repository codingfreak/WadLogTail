using System;
using System.Linq;

namespace codingfreaks.WadLogTail.Ui.WindowsApp.ViewModel
{
    using System.Diagnostics;

    using codingfreaks.cfUtils.Logic.Wpf.MvvmLight;
    using codingfreaks.WadLogTail.Ui.WindowsApp.Attributes;

    using GalaSoft.MvvmLight.Command;

    /// <summary>
    /// Base class for all view models expect the <see cref="MainViewModel"/>.
    /// </summary>
    public abstract class BaseViewModel : BroadcastViewModelBase
    {
        #region member vars

        private ViewModelDescriptionAttribute _descriptionAttribute;

        #endregion

        #region constructors and destructors

        public BaseViewModel()
        {
            WindowId = Guid.NewGuid();
            CleanupCommand = new RelayCommand(Cleanup);
            // Check the required attributes
            var t = GetType();
            if (!t.IsAbstract && t.IsDefined(typeof(ViewModelDescriptionAttribute), false) == false)
            {
                Trace.TraceError("BaseViewModel required the ViewModelDescriptionAttribute");
                throw new InvalidOperationException();
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// Is used to trigger the internal cleanup.
        /// </summary>
        public RelayCommand CleanupCommand { get; private set; }

        /// <summary>
        /// A part of the window title.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// A unique identifier for this window.
        /// </summary>
        public Guid WindowId { get; private set; }

        /// <summary>
        /// Retrieves the currently assigned description attribute for this instance.
        /// </summary>
        private ViewModelDescriptionAttribute DescriptionAttribute
        {
            get
            {
                if (_descriptionAttribute != null)
                {
                    return _descriptionAttribute;
                }
                var attributes = GetType().GetCustomAttributes(typeof(ViewModelDescriptionAttribute), true);
                _descriptionAttribute = attributes.FirstOrDefault() as ViewModelDescriptionAttribute;
                return _descriptionAttribute;
            }
        }

        #endregion
    }
}