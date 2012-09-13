using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace JoyOs.FileSystem.Provider
{
   public class FileSysPathDataProvider:IPathDataProvider
    {
       /// <summary>
       /// Возвращает коллекцию элементов находящихся в папке с таким именем
       /// </summary>
       /// <param name="textPattern">примерный путь к папке</param>
       /// <returns>коллекцию похожих элементов</returns>
        public IEnumerable<string> GetItems(string textPattern)
        {
            if (textPattern.Length < 2 || textPattern[1] != ':')
            {
                return null;
            }
            var lastSlashPos = textPattern.LastIndexOf(Path.DirectorySeparatorChar);
            var baseFolder = textPattern;
            string partialMatch = null;
            if (lastSlashPos != -1)
            {
                baseFolder = textPattern.Substring(0, lastSlashPos);
                partialMatch = textPattern.Substring(lastSlashPos + 1);
            }
            try
            {
                return Directory.GetDirectories(baseFolder + Path.DirectorySeparatorChar, partialMatch + "*");
            }
            catch
            {
                return null;
            }
        }

       /// <summary>
       /// Проверят путь на корректность
       /// </summary>
       /// <param name="value">путь</param>
       /// <returns>true/false</returns>
       public bool LegalPath(string value)
       {
           return value.EndsWith(Path.DirectorySeparatorChar.ToString());
       }

       /// <summary>
       /// Проверяет существует ли папка, если это не папка то запускает файл
       /// </summary>
       /// <param name="value">путь</param>
       /// <param name="checkLegal">проверять LegalPath пути</param>
       /// <returns>true if это папка и путь корректный</returns>
       public bool Exist(string value,bool checkLegal)
       {
           if (File.Exists(value))
           {
               //UNDONE: can be exception если файл неизвестно с помошью чего открыть
               try
               {
                   Process.Start(value);
               }
               catch (Exception ex)
               {
                   //JoyOs.Windows.Dialogs.MessageBox.ShowDialog(ex.Message,
                   //    "Ошибка открытия файла", MessageBoxButton.OK);
               }
               return false;
           }
           checkLegal = !checkLegal || LegalPath(value);
           return checkLegal && Directory.Exists(value);
       }
    }
}
