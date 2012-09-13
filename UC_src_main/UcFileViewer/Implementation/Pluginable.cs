using System.Collections.Generic;
using System.Windows.Controls;


namespace UcFileViewer
{
    /// <summary>
    /// Класс файлового мэнеджера
    /// </summary>
    public partial class FileManager
    {
        #region Реализация IPluginable

        public IEnumerable<MenuItem> Menu { get; set; }

        public string SupportedExtensions
        {
            get { throw new System.NotImplementedException(); }
        }

        public System.Version CurrentVersion
        {
            get { throw new System.NotImplementedException(); }
        }

        public string FriendlyName
        {
            get { throw new System.NotImplementedException(); }
        }

        #endregion
    }
}