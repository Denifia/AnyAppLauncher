using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32;

namespace AnyApp
{
    class Program
    {
        const string FormatError = "Link incorrectly formatted. Correct format: anyapp://{appName}:{uriScheme}@{uri}";
        static string regKeyString = @"HKEY_LOCAL_MACHINE\Software\Classes";

        static void Main(string[] args)
        {
            var configSection = (ApplicationDetailsConfigurationSection)ConfigurationManager.GetSection("applicationSection");
            var applications = configSection.Applications;

            // Bail if not only 1 argument
            switch (args.Length)
            {
                case 0:
                    Install();
                    return;
                case 1:
                    if (args[0].Equals("uninstall", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Uninstall();
                        return;
                    }
                    // Continue with the application
                    break;
                default:
                    Bail("No link provided as the first and only parameter.");
                    return;
            }

            Uri uri;
            Uri.TryCreate(args[0], UriKind.Absolute, out uri);

            // Bail if argument couldn't formate into a URI
            if (uri == null)
            {
                Bail(FormatError);
                return;
            }

            // User info not formatted correctly
            if (uri.UserInfo.Split(':').Length != 2)
            {
                Bail(FormatError);
                return;
            }

            // Find out what browser they want to use
            var applicationName = uri.UserInfo.Split(':')[0].ToLower();
            String applicationPath = null;
            var application =
                applications.FirstOrDefault(
                    x => x.Name.Equals(applicationName, StringComparison.InvariantCultureIgnoreCase));

            // Bail if the application couldn't be identified
            if (application == null)
            {
                Bail(String.Format("Application {0} not defined.", applicationName));
                return;
            }

            // Fetch the path to the application exe
            switch ((ApplicationDetails.ApplicationPathType)application.PathType)
            {
                case ApplicationDetails.ApplicationPathType.Registry:
                    applicationPath = (string) Registry.GetValue(application.Path, String.Empty, null);
                    break;
                case ApplicationDetails.ApplicationPathType.File:
                    applicationPath = application.Path;
                    break;
                default:
                    Bail("Application PathType unknown. Use 1 for registry or 2 for file path.");
                    return; // Bail
            }

            // Bail if we dont know what browser to use or the path wasnt found
            if (applicationPath == null)
            {
                Bail("Could not find the path to the Application.");
                return;
            }

            var newUri = new UriBuilder(uri.AbsoluteUri)
            {
                // Replace the AnyBrowser scheme with Http or Https
                Scheme = uri.UserInfo.Split(':')[1].ToLower(),
                Port = -1, // default port for scheme
                UserName = String.Empty, // Strip out user info
                Password = String.Empty // Strip out user info
            };

            // Open Broswer to the URI
            Process.Start(applicationPath, String.Format(application.Format, newUri));
        }

        static void Bail(string message = "An unknown error occured")
        {
            MessageBox.Show(message, "Any Application Launcher");
        }

        static void Install()
        {
            var rootRegKey = Registry.LocalMachine.OpenSubKey(@"software\classes", true);
            if (rootRegKey == null)
            {
                Bail(String.Format("{0} not found.", regKeyString));
                return;
            }

            var anyAppRegKey = rootRegKey.CreateSubKey("anyapp", RegistryKeyPermissionCheck.ReadWriteSubTree);
            if (anyAppRegKey == null)
            {
                regKeyString += @"\AnyApp";
                Bail(String.Format("{0} not found.", regKeyString));
                return;
            }
            anyAppRegKey.SetValue(String.Empty, "URL:AnyApp Protocol");
            anyAppRegKey.SetValue("URL Protocol", String.Empty);

            anyAppRegKey = anyAppRegKey.CreateSubKey("shell");
            if (anyAppRegKey == null)
            {
                regKeyString += @"\shell";
                Bail(String.Format("{0} not found.", regKeyString));
                return;
            }

            anyAppRegKey = anyAppRegKey.CreateSubKey("open");
            if (anyAppRegKey == null)
            {
                regKeyString += @"\open";
                Bail(String.Format("{0} not found.", regKeyString));
                return;
            }

            anyAppRegKey = anyAppRegKey.CreateSubKey("command");
            if (anyAppRegKey == null)
            {
                regKeyString += @"\command";
                Bail(String.Format("{0} not found.", regKeyString));
                return;
            }

            var uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var pathToExe = uri.LocalPath;
            anyAppRegKey.SetValue(String.Empty, String.Format("\"{0}\" \"%1\"", pathToExe));

            anyAppRegKey.Close();
            rootRegKey.Close();
        }

        static void Uninstall()
        {
            var rootRegKey = Registry.LocalMachine.OpenSubKey(@"software\classes", true);
            if (rootRegKey == null)
            {
                Bail(String.Format("{0} not found.", regKeyString));
                return;
            }
            try
            {
                rootRegKey.DeleteSubKeyTree("AnyApp");
            }
            catch
            {
                // Don't care.
            }
            
            rootRegKey.Close();
        }
    }
}
