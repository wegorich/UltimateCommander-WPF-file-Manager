using System.Collections;

namespace JoyOs.FileSystem.Functionality
{
    internal static class CommonAPI
    {
        /// <summary>
        /// COMs the is data item selected.
        /// </summary>
        /// <param name="itemsCollection">The items collection.</param>
       public static bool ComIsDataItemSelected(ICollection itemsCollection)
        {
           return (itemsCollection != null);
        }
    }
}