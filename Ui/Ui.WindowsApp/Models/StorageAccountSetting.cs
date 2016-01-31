using System;
using System.Linq;

namespace codingfreaks.WadLogTail.Ui.WindowsApp.Models
{
    using System.Threading.Tasks;

    using codingfreaks.cfUtils.Logic.Portable.Extensions;

    /// <summary>
    /// Encapsulates data and logic for a single Azure storage account.
    /// </summary>
    public class StorageAccountSetting
    {
        #region methods

        /// <summary>
        /// Tests if the current settings are valid Azure Storage Account settings.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Test()
        {
            if (Account.IsNullOrEmpty() || Key.IsNullOrEmpty())
            {
                return false;
            }
            return true;
        }

        #endregion

        #region properties

        /// <summary>
        /// The worldwide unique name of the account.
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// The URL built from <see cref="Account"/> and <see cref="Key"/>.
        /// </summary>
        public string AccountUrl => $"DefaultEndpointsProtocol=https;AccountName={Account};AccountKey={Key}";

        /// <summary>
        /// A unique id generated at save.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The secret key for the account from the Azure Portal.
        /// </summary>
        public string Key { get; set; }

        #endregion
    }
}