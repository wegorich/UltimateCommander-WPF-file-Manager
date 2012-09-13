namespace JoyOs.BusinessLogic.Pluginable
{
    // These are used to unpack specific file types,
    // usually archive formats. Some packer plugins
    // also support the creation of new archives of
    // the supported type, and to modify existing archives.
    // Examples are:
    // A plugin to
    // - pack/unpack bzip2 archives, a format similar to the built-in gzip.
    // - create a list of files in the selected directories. Useful to create catalogs of whole disks
    // - create a batch file for all selected files, e.g. for repeated batch-copying
    
    public interface IPackerPlugin : IPluginable
    {
    }
}
