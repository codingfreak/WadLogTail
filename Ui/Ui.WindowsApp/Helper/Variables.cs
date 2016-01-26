using System;
using System.Linq;

namespace codingfreaks.WadLogTail.Ui.WindowsApp.Helper
{
    using codingfreaks.WadLogTail.Ui.WindowsApp.Models;

    /// <summary>
    /// Provides access to internally variables needed by many classes.
    /// </summary>
    public static class Variables
    {
        #region constructors and destructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static Variables()
        {
            Settings = ProgramSetting.Load();
        }

        #endregion

        #region properties

        /// <summary>
        /// The currently loaded or created program settings.
        /// </summary>
        public static ProgramSetting Settings { get; private set; }

        #endregion
    }
}