using System;
using System.Linq;

namespace codingfreaks.WadLogTail.Ui.WindowsApp.Models
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    using codingfreaks.cfUtils.Logic.Portable.Extensions;

    using Newtonsoft.Json;

    public class ProgramSetting
    {
        #region methods

        /// <summary>
        /// Loads settings from the default location in user profile.
        /// </summary>
        /// <returns></returns>
        public static ProgramSetting Load()
        {
            var folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AzureLogWatcher");
            var fileName = Path.Combine(folderName, "settings.json");
            if (!Directory.Exists(folderName) || !File.Exists(fileName))
            {
                return new ProgramSetting();
            }
            try
            {
                var text = File.ReadAllText(fileName);
                return text.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject<ProgramSetting>(text);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
            return new ProgramSetting();
        }

        /// <summary>
        /// Loads settings from the default location in user profile.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AzureLogWatcher");
            var fileName = Path.Combine(folderName, "settings.json");
            if (!Directory.Exists(folderName))
            {
                try
                {
                    Directory.CreateDirectory(folderName);
                }
                catch
                {
                    return false;
                }
            }
            try
            {
                File.WriteAllText(fileName, JsonConvert.SerializeObject(this));
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
            return false;
        }

        #endregion

        #region properties

        /// <summary>
        /// A list of all storage account settings already used by the user.
        /// </summary>
        public IEnumerable<StorageAccountSetting> Accounts { get; set; } = new List<StorageAccountSetting>();

        /// <summary>
        /// The name of the storage account which was used last.
        /// </summary>
        public string LastStorageAccount { get; set; }

        #endregion
    }
}