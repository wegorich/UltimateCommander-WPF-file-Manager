namespace JoyOs.BusinessLogic.Pluginable
{
    // File system plugins are used via the Network Neighborhood.
    // They usually access a certain part of your PC which
    // cannot be accessed via drive letters, or some remote
    // system. Examples are:
    // A plugin to access
    // - a Windows CE or PocketPC device attached to your PC
    // - remote Web servers via HTTP, to download a whole list of files
    // - a mail server

    public interface IFileSystemPlugin : IPluginable
    {

    }
}
