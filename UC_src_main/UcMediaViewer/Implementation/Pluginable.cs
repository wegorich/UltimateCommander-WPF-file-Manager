using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace UcMediaViewer
{
    /// <summary>
    /// Класс файлового мэнеджера
    /// </summary>
    public partial class MediaViewer
    {
        #region Члены IPluginable

        public IEnumerable<MenuItem> Menu { get; set; }

        public string SupportedExtensions
        {
            get
            {
                return "*.mp4;*.m4v;*.mp4v;*.3gp;*.3gpp;*.3g2;*.3gp2;*.m2ts;*.m2t;*.mts;*.ts;*.tts;*.dvr-ms;*.wtv;*.m4a;*.asx;*.wax;*.m3u;*.wpl;*.wvx;*.wmx;*.search-ms;*.mid;*.rmi;*.midi;*.asf;*.wm;*.wma;*.wmv;*.wmd;*.wav;*.snd;*.au;*.aif;*.aifc;*.aiff;*.mp2;*.mp3;*.adts;*.adt;*.aac;*.avi;*.mov;*.jpg;*.jpeg;*.mpeg;*.mpg;*.m1v;*.m2v;*.mod;*.mpa;*.mpe;*.ifo;*.vob";
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