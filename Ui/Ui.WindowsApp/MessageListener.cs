namespace codingfreaks.WadLogTail.Ui.WindowsApp
{
    using System.Diagnostics;

    using codingfreaks.WadLogTail.Ui.WindowsApp.Enumerations;
    using codingfreaks.WadLogTail.Ui.WindowsApp.Models;

    using GalaSoft.MvvmLight.Messaging;

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

        public bool IsVisible => true;
        
        #endregion
    }
}