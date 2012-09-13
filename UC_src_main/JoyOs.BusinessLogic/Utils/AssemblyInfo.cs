using System.Reflection;

namespace JoyOs.BusinessLogic.Utils
{
    public static class AssemblyInfo
    {
        #region Assembly Attribute Accessors

        public static string AssemblyTitle
        {
            get
            {
                var attributes = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                var assemblyVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();

                return assemblyVersion.Remove(assemblyVersion.LastIndexOf('.'));
            }
        }

        public static string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);

                return (attributes.Length != 0)
                            ? ((AssemblyDescriptionAttribute)attributes[0]).Description
                            : "";
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);

                return (attributes.Length != 0)
                            ? ((AssemblyProductAttribute)attributes[0]).Product
                            : "";
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

                return (attributes.Length != 0)
                            ? ((AssemblyCopyrightAttribute)attributes[0]).Copyright
                            : "";
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

                return (attributes.Length != 0)
                            ? ((AssemblyCompanyAttribute)attributes[0]).Company
                            : "";
            }
        }

        public static string AssemblyInformationVersion
        {
            get
            {
                object[] attributes = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);

                return (attributes.Length != 0)
                            ? ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion
                            : "";
            }
        }

        public static string ProcessorArchitecture
        {
            get
            {
                return Assembly.GetCallingAssembly().GetName().ProcessorArchitecture.ToString().Replace('X', 'x');
            }
        }

        #endregion
    }
}
