using System.Collections;
using System.ComponentModel;
using System.Windows.Controls;
using JoyOs.FileSystem.Model;

namespace JoyOs.FileSystem.Sorting
{
    /// <summary>
    /// Sort Logic for ViewDataItems
    /// </summary>
    public class SortLogic : IComparer
    {
        private delegate int SortDelegate(ILogicItem arg1, ILogicItem arg2);

        readonly SortDelegate _compare;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortLogic"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="column">The column.</param>
        public SortLogic(ListSortDirection direction, DataGridColumn column)
        {
            var dir = (direction == ListSortDirection.Ascending) ? 1 : -1;
            switch ((string)column.Header)
            {//TODO: сделать нормальную сортировку
                case "Имя":
                    _compare = (x, y) => CompareToName(x,y,dir);
                    break;
                case "Тип":
                    _compare = (x, y) => CompareToType(x, y, dir);
                    break;
                case "Размер":
                    _compare = (x, y) => CompareToLength(x, y, dir);                                        
                    break;
                case "Дата":
                    _compare = (x, y) => CompareToDate(x, y, dir);
                    break;
                default:_compare = (x, y) => x.CompareTo(y);
                    break;
            }
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less 
        /// than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of 
        /// <paramref name="x"/> and <paramref name="y"/>, as 
        /// shown in the following table.Value Meaning Less than zero 
        /// <paramref name="x"/> is less than <paramref name="y"/>. Zero <
        /// paramref name="x"/> equals <paramref name="y"/>. 
        /// Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// Neither <paramref name="x"/> nor <paramref name="y"/> implements the 
        /// <see cref="T:System.IComparable"/> interface.-or- 
        /// <paramref name="x"/> and <paramref name="y"/> are of different types 
        /// and neither one can handle comparisons with the other. </exception>
        int IComparer.Compare(object x, object y)
        {
            return _compare((ILogicItem)x, (ILogicItem)y);
        }

        /// <summary>
        /// Compares to name.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="dir">The dir.</param>
        private static int CompareToName(ILogicItem x,ILogicItem y,int dir)
        {
            return  x.Name==y.Name&&y.IsFolder&&x.IsFolder? 0 :x.IsFolder&&y.IsFolder&&x.IsFolder
                                                                                                            ?-1:y.IsFolder
                                                                                                            ?1: x.Info.Name.CompareTo(y.Info.Name) * dir;
        }

        /// <summary>
        /// Compares to type.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="dir">The dir.</param>
        private static int CompareToType(ILogicItem x, ILogicItem y, int dir)
        {
            return x.IsFolder && y.IsFolder ? 0 : x.IsFolder ? -1 : y.IsFolder ? 1 : x.Type.CompareTo(y.Type) * dir;
        }

        /// <summary>
        /// Compares to date.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="dir">The dir.</param>
        private static int CompareToDate(ILogicItem x, ILogicItem y, int dir)
        {
            return x.IsFolder && y.IsFolder ? 0 : x.IsFolder ? -1 : y.IsFolder ? 1 : x.Info.LastAccessTime.CompareTo(y.Info.LastAccessTime) * dir;
        }

        /// <summary>
        /// Compares to length.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="dir">The dir.</param>
        private static int CompareToLength(ILogicItem x, ILogicItem y, int dir)
        {
            return x.IsFolder&&y.IsFolder ? 0 :x.IsFolder?-1:y.IsFolder?1: x.Length.CompareTo(y.Length) * dir;
        }
    }
}