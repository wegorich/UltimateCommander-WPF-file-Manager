using System.Collections.Generic;
using System.IO;

namespace JoyOs.BusinessLogic.Common
{
    public static class DataReader
    {
        #region Read String[] from File
        /// <summary>
        /// Считывет все строки из файла
        /// </summary>
        /// <param name="fileFullName">Имя файла string</param>
        /// <returns>string[] </returns>
        public static IEnumerable<string> ReadDataFromFile(string fileFullName)
        {
            if (!File.Exists(fileFullName)) return null;

            var list = new List<string>();

            using (var reader = new StreamReader(fileFullName, System.Text.Encoding.Unicode))
            {
                while (!reader.EndOfStream)
                {
                    list.Add(reader.ReadLine());
                }
            }

            return list.ToArray();
        }
        #endregion

        #region Write String[] to file
        /// <summary>
        /// Записываем строки в файл
        /// </summary>
        /// <param name="fileName">string содержаший имя файла</param>
        /// <param name="inf">string[]</param>
        public static void SaveDataToFile(string fileName, IEnumerable<string> inf)
        {
            using (var writer = new StreamWriter(fileName, false, System.Text.Encoding.Unicode))
            {
                foreach (var str in inf)
                {
                    writer.Write(str + "\r\n");
                }
            }
        }
        #endregion
    }
}
