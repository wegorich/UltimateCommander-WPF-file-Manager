using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace UcImageViewer
{
    public partial class ImageList
    {
        #region Реализация IPluginable

        public IEnumerable<MenuItem> Menu { get; set; }

        public string SupportedExtensions
        {
            get
            {
                return "*.jpg,*.gif,*.png,*.bmp,*.jpe,*.jpeg,*.wmf,*.emf,*.xbm,*.ico,*.eps,*.tif,*.tiff";
            }
        }

        public Version CurrentVersion
        {
            get { throw new NotImplementedException(); }
        }

        public string FriendlyName
        {
            get { throw new NotImplementedException(); }
        }

        #endregion        
    }
}
