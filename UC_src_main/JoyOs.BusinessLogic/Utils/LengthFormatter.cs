using System;
using System.Text;

namespace JoyOs.BusinessLogic.Utils
{
    public class LengthFormatter : IFormattable
    {
        #region Члены IFormattable

        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        #endregion

        public static string LengthFormat(long bytes)
        {
            var format = new StringBuilder("0 ");

            if (bytes >= 1024L)
            {
                var tp = bytes;

                do
                {
                    format.Append("000 ");
                    tp /= 1000L;
                } while (tp >= 1000L);
            }

            return bytes.ToString(format.ToString());
        }
    }
}
