using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ForceDeleteApp
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            if (args.Length > 0)
            {
                if (args[0] == "--install")
                {
                    InstallContextMenu();
                    MessageBox.Show("Context menu installed successfully.", "Force Delete");
                    return;
                }
                if (args[0] == "--uninstall")
                {
                    UninstallContextMenu();
                    MessageBox.Show("Context menu uninstalled successfully.", "Force Delete");
                    return;
                }

                string filePath = args[0];
                if (File.Exists(filePath) || Directory.Exists(filePath))
                {
                    Application.Run(new Form1(filePath));
                }
                else
                {
                    MessageBox.Show("File or directory not found.", "Error");
                }
            }
            else
            {
                // Prompt installation if launched without args
                var res = MessageBox.Show("Do you want to install the 'Force Delete' context menu?\n\nChoose 'Cancel' to exit.", "Force Delete Setup", MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Yes)
                {
                    InstallContextMenu();
                    MessageBox.Show("Context menu installed.", "Success");
                }
                else if (res == DialogResult.No)
                {
                    var r2 = MessageBox.Show("Do you want to UNINSTALL the 'Force Delete' context menu?", "Force Delete Setup", MessageBoxButtons.YesNo);
                    if (r2 == DialogResult.Yes)
                    {
                        UninstallContextMenu();
                        MessageBox.Show("Context menu uninstalled.", "Success");
                    }
                }
            }
        }

        static void InstallContextMenu()
        {
            try
            {
                string exePath = Process.GetCurrentProcess().MainModule.FileName;

                // File context menu
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(@"*\shell\ForceDelete"))
                {
                    key.SetValue("", "Force Delete");
                    key.SetValue("Icon", exePath);
                    using (RegistryKey commandKey = key.CreateSubKey("command"))
                    {
                        commandKey.SetValue("", $"\"{exePath}\" \"%1\"");
                    }
                }

                // Directory context menu
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(@"Directory\shell\ForceDelete"))
                {
                    key.SetValue("", "Force Delete");
                    key.SetValue("Icon", exePath);
                    using (RegistryKey commandKey = key.CreateSubKey("command"))
                    {
                        commandKey.SetValue("", $"\"{exePath}\" \"%1\"");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("You need to run this program as Administrator to install the context menu.", "Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error installing: {ex.Message}", "Error");
            }
        }

        static void UninstallContextMenu()
        {
            try 
            { 
                Registry.ClassesRoot.DeleteSubKeyTree(@"*\shell\ForceDelete", false); 
                Registry.ClassesRoot.DeleteSubKeyTree(@"Directory\shell\ForceDelete", false); 
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("You need to run this program as Administrator to uninstall the context menu.", "Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uninstalling: {ex.Message}", "Error");
            }
        }
    }
}