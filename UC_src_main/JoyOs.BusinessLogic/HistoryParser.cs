using System;
using System.IO;
using JoyOs.BusinessLogic.Configuration;

namespace JoyOs.BusinessLogic
{
    // TODO: Add multilanguage support

    /// <summary>
    ///  ласс дл€ работы с файлами History
    /// </summary>
    public sealed class HistoryParser : IHistoryParser
    {
        private enum HistoryTokens
        {
            NoToken = 0,

            SearchIn,
            SearchText,
            SearchName,
            RightHistory,
            LeftHistory,
            MkDirHistory
        }

        private FileStream _fileStream;

        ~HistoryParser()
        {
            if (_fileStream == null) return;
            _fileStream.Close();

            _fileStream = null;
        }

        #region „лены ITextParser

        public bool Init()
        {
            throw new NotImplementedException();
        }

        public bool Close()
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginParseRead()
        {
            throw new NotImplementedException();
        }

        public IAsyncResult EndParseRead()
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginParseWrite(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult EndParseWrite(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public bool ParseCommand(int iIndex)
        {
            throw new NotImplementedException();
        }

        public string CheckCommand(string command, out string value)
        {
            throw new NotImplementedException();
        }

        public void RemoveParm(string parm)
        {
            throw new NotImplementedException();
        }

        public void AppendParm(string parm, string values)
        {
            throw new NotImplementedException();
        }

        public long ParmValue(string sz, long nDefaultVal)
        {
            throw new NotImplementedException();
        }

        public float ParmValue(string sz, float flDefaultVal)
        {
            throw new NotImplementedException();
        }

        public int ParmCount()
        {
            throw new NotImplementedException();
        }

        public int FindParm(string parm)
        {
            throw new NotImplementedException();
        }

        public string GetParm(int nIndex)
        {
            throw new NotImplementedException();
        }

        public void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }

        public float WriteTimeout
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public float ReadTimeout
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region „лены IDisposable

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}