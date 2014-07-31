﻿using GoToWindow.Api;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows;

namespace GoToWindow.Windows
{
    public partial class SettingsWindow : Window
    {
        private bool _originalStartWithWindowsIsChecked;
        private readonly IGoToWindowContext _context;

        public SettingsWindow(IGoToWindowContext context)
        {
            _context = context;

            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();

            if(_originalStartWithWindowsIsChecked != StartWithWindowsCheckbox.IsChecked)
            {
                UpdateStartWithWindows(StartWithWindowsCheckbox.IsChecked == true);
            }

            _context.EnableAltTabHook(Properties.Settings.Default.HookAltTab);

            Close();
        }

        private void UpdateStartWithWindows(bool active)
        {
            if (active)
            {
                var executablePath = Assembly.GetExecutingAssembly().Location;

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "reg.exe",
                        Arguments = string.Format("add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\" /v \"GoToWindow\" /t REG_SZ /d \"{0}\" /f", executablePath),
                        Verb = "runas",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden

                    }
                };
                process.Start();
                process.WaitForExit();
            }
            else
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "reg.exe",
                        Arguments = "delete \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\" /v \"GoToWindow\" /f",
                        Verb = "runas",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                process.Start();
                process.WaitForExit();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            var runList = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);
            if (runList != null)
            {
                var executablePath = Assembly.GetExecutingAssembly().Location;
                StartWithWindowsCheckbox.IsChecked = _originalStartWithWindowsIsChecked = ((string) runList.GetValue("GoToWindow") == executablePath);
            }

            if (WindowsVersion.IsWindows8())
            {
                var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                NoElevatedPrivilegesWarning.Visibility = (principal.IsInRole(WindowsBuiltInRole.Administrator) ||
                                                          principal.IsInRole(0x200))
                    ? Visibility.Hidden
                    : Visibility.Visible;
            }
            else
            {
                NoElevatedPrivilegesWarning.Visibility = Visibility.Hidden;
            }

            VersionTextBlock.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
