using System.Collections.Generic;

namespace JoyOs.FileSystem
{
    public interface IPathDataProvider
    {
        IEnumerable<string> GetItems(string textPattern);
        bool LegalPath(string value);
        bool Exist(string value,bool checkLegal);
    }
}
