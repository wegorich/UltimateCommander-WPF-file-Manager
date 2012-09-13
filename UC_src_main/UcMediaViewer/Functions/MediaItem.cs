using System;
using System.IO;
using System.Windows.Media;

using LengthFormatter = JoyOs.BusinessLogic.Utils.LengthFormatter;

namespace UcMediaViewer
{
    public partial class MediaViewer
    {
        #region Media Item

        /// <summary>
        /// Have got Information about one of dataGridItems
        /// </summary>
        public class MediaItem
        {
            /// <summary>
            /// Fileinfo VideoItem Constuctor
            /// </summary>
            /// <param name="file"></param>
            public MediaItem(FileInfo file)
                : this(System.IO.Path.GetFileNameWithoutExtension(file.Name),
                       file.Extension,
                       file.Length,
                       file.LastAccessTime,
                       file.FullName
                    )
            {
            }

            /// <summary>
            /// Конструктор класа DataGridItem
            /// </summary>
            ///<remarks>1: Имя - 2: Тип - 3: Длина - 4: Дата - 5: Tag</remarks>
            /// <param name="name">string Имя</param>
            /// <param name="type">string Тип</param>
            /// <param name="length">string Длина</param>
            /// <param name="lastChangeDate">string Дата</param>
            /// <param name="path">string Путь</param>
            public MediaItem(string name, string type, long length, DateTime lastChangeDate, string path)
            {
                Name = name;
                Type = type;
                Length = LengthFormatter.LengthFormat( length );

                Date = lastChangeDate.ToString();
                Path = path;
            }

            /// <summary>
            /// Have got Source of Image - show small file icon in DataGrid
            /// </summary>
            public ImageSource Image { get; set; }

            /// <summary>
            /// Have got only Name of file with out Extension
            ///</summary>
            public string Name { get; set; }

            /// <summary>
            /// Have got Extension of file
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// Have got Size of file in string +KB
            /// </summary>
            public string Length { get; set; }            

            /// <summary>
            /// Have got Last changed time in string
            /// </summary>
            public string Date { get; set; }

            /// <summary>
            /// Have got full path
            /// </summary>
            public string Path { get; set; }
        }

        #endregion
    }
}