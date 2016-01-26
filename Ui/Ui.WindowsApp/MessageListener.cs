namespace codingfreaks.WadLogTail.Ui.WindowsApp
{
    using System.Diagnostics;

    using codingfreaks.WadLogTail.Ui.WindowsApp.Enumerations;
    using codingfreaks.WadLogTail.Ui.WindowsApp.Models;
    using codingfreaks.WadLogTail.Ui.WindowsApp.ViewModel;

    using GalaSoft.MvvmLight.Messaging;

    /// <summary>
    /// The purpose of this class is to be provided as a static ressource in App.xaml. This way
    /// it is ensured that the messenger is up all the time.
    /// </summary>
    public class MessageListener
    {
        #region constructors and destructors

        public MessageListener()
        {
            Trace.TraceInformation("Message listener running.");
            Messenger.Default.Register<WindowRequestOpenMessage>(
                this,
                m =>
                {
                    switch (m.WindowType)
                    {
                        case WindowType.SelectStorageAccountView:
                        {
                            Messenger.Default.Send(WindowOpenMessage.GetInstance(new SelectStorageAccountWindow(), m.WindowType, m.WindowTarget, "Select Storage Account"));
                            break;
                        }
                        default:
                        {
                            //TODO: defaultWindow? Exception --> unknownWindow?
                            break;
                        }
                    }
                });
        }

        #endregion

        #region properties

        /// <summary>
        /// This property is only important for binding in MainView and holding a variable for
        /// the <see cref="MainViewModel "/>.
        /// </summary>
        /// <remarks>
        /// Bind this to the IsEnabled property of the main view.
        /// </remarks>
        public bool IsVisible => true;
        
        #endregion
    }
}