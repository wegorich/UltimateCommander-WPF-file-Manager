namespace JoyOs.BusinessLogic.Pluginable
{
    // Content plugins have several purposes:
    // Searching for specific file properties and contents,
    // displaying of these properties in file lists, and using them
    // in the multi-rename tool to add them to the file name.
    // Examples are:
    // - mp3 id-tags (Artist, Title, Album etc)
    // - digital photo information from JPEG files in EXIF format (aperture, exposure time, was the flash used etc)
    // - file attributes like creation time, program version number etc.

    public interface IContentPlugin : IPluginable
    {

    }
}
