namespace codingfreaks.WadLogTail.Ui.WindowsApp.Enumerations
{
    /// <summary>
    /// Defines the target of a new windows in dock manager
    /// </summary>
    public enum WindowTarget
    {
        /// <summary>
        /// Main area.
        /// </summary>
        ContentHost = 0,

        /// <summary>
        /// Detail area.
        /// </summary>
        DetailPane = 1,

        /// <summary>
        /// Init contentPane while loading Layout.
        /// </summary>
        PaneLoading = 2,

        /// <summary>
        /// Open a dialog.
        /// </summary>
        Dialog = 3,

        /// <summary>
        /// Open a modal dialog.
        /// </summary>
        ModalDialog = 4,

        /// <summary>
        /// Image area.
        /// </summary>
        ImagePane = 5,

        /// <summary>
        /// Marketplace area.
        /// </summary>
        MarketplacePane = 6,

        /// <summary>
        /// Mapping area to link product variations.
        /// </summary>
        MappingPane = 7
    }
}