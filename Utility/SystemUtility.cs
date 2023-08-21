using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Utility
{
    public class SystemUtility
    {
        public static void CallKeyboard()
        {
            try
            {
                Process.Start("osk.exe");
            }
            catch (Exception ex)
            {
                if (ex.Message == "The system cannot find the file specified")
                {
                    try
                    {
                        //Process.Start(GetSystemDirectory() + "/osk.exe");
                        Process.Start("c:/windows/system32/osk.exe");
                    }
                    catch (Exception ex2)
                    {
                        try
                        {
                            Process.Start(@"C:\Windows\WinSxS\amd64_microsoft-windows-osk_31bf3856ad364e35_10.0.18362.449_none_0098d787eb84df09\osk.exe");
                        }
                        catch (Exception ex3)
                        {
                            try
                            {
                                ProcessStartInfo inf = new ProcessStartInfo(Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System)).FullName, "Sysnative", "cmd.exe"), "/c osk.exe");
                                inf.WindowStyle = ProcessWindowStyle.Hidden;
                                Process.Start(inf);
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
        }
        public static void SetStartup()
        {
            try
            {
                var installedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ParsaTeb.exe";
                string keys =
                @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run";
                string appName = "Blender";

                //accessing the CurrentUser root element  
                //and adding "OurSettings" subkey to the "SOFTWARE" subkey  
                RegistryKey key = Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");

                if (Registry.GetValue(keys, appName, null) !=null && Registry.GetValue(keys, appName, null).ToString() != installedPath)
                {
                    key.DeleteValue("Blender");
                }
                //storing the values  
                key.SetValue(appName, installedPath);
                key.Close();

                //if (Registry.GetValue(keys, appName, null) == null)
                //{
                //    // if key doesn't exist
                //    using (RegistryKey key2 =
                //    Registry.CurrentUser.OpenSubKey
                //    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                //    {
                //        key2.DeleteValue("Blender");
                //        key2.SetValue("Blender", Path.GetDirectoryName
                //        (installedPath));
                //        key2.Dispose();
                //        key2.Flush();
                //    }
                //}
                //else
                //{
                //    //if key Exist
                //}
            }
            catch (Exception ex)
            {
                //Error_Logging(ex);
            }

        }
        [DllImport("User32.dll")]
        public static extern int SendMessage
(IntPtr hWnd,
uint Msg,
uint wParam,
uint lParam);
        public const uint WM_SYSCOMMAND = 0x112;
        public const uint SC_SCREENSAVE = 0xF140;
        public enum SpecialHandles
        {
            HWND_DESKTOP = 0x0,
            HWND_BROADCAST = 0xFFFF
        }
        public static void TurnOnScreenSaver()
        {
            SendMessage(
            new IntPtr((int)SpecialHandles.HWND_BROADCAST),
            WM_SYSCOMMAND,
            SC_SCREENSAVE,
            0);
        }
    }

    public static class ScreenSaver
    {
        // Signatures for unmanaged calls
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool SystemParametersInfo(
           int uAction, int uParam, ref int lpvParam,
           int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool SystemParametersInfo(
           int uAction, int uParam, ref bool lpvParam,
           int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int PostMessage(IntPtr hWnd,
           int wMsg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr OpenDesktop(
           string hDesktop, int Flags, bool Inherit,
           uint DesiredAccess);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseDesktop(
           IntPtr hDesktop);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnumDesktopWindows(
           IntPtr hDesktop, EnumDesktopWindowsProc callback,
           IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool IsWindowVisible(
           IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetForegroundWindow();

        // Callbacks
        private delegate bool EnumDesktopWindowsProc(
           IntPtr hDesktop, IntPtr lParam);

        // Constants
        private const int SPI_GETSCREENSAVERACTIVE = 16;
        private const int SPI_SETSCREENSAVERACTIVE = 17;
        private const int SPI_GETSCREENSAVERTIMEOUT = 14;
        private const int SPI_SETSCREENSAVERTIMEOUT = 15;
        private const int SPI_GETSCREENSAVERRUNNING = 114;
        private const int SPIF_SENDWININICHANGE = 2;

        private const uint DESKTOP_WRITEOBJECTS = 0x0080;
        private const uint DESKTOP_READOBJECTS = 0x0001;
        private const int WM_CLOSE = 16;


        // Returns TRUE if the screen saver is active 
        // (enabled, but not necessarily running).
        public static bool GetScreenSaverActive()
        {
            bool isActive = false;

            SystemParametersInfo(SPI_GETSCREENSAVERACTIVE, 0,
               ref isActive, 0);
            return isActive;
        }

        // Pass in TRUE(1) to activate or FALSE(0) to deactivate
        // the screen saver.
        public static void SetScreenSaverActive(int Active)
        {
            int nullVar = 0;

            SystemParametersInfo(SPI_SETSCREENSAVERACTIVE,
               Active, ref nullVar, SPIF_SENDWININICHANGE);
        }

        // Returns the screen saver timeout setting, in seconds
        public static Int32 GetScreenSaverTimeout()
        {
            Int32 value = 0;

            SystemParametersInfo(SPI_GETSCREENSAVERTIMEOUT, 0,
               ref value, 0);
            return value;
        }

        // Pass in the number of seconds to set the screen saver
        // timeout value.
        public static void SetScreenSaverTimeout(Int32 Value)
        {
            int nullVar = 0;

            SystemParametersInfo(SPI_SETSCREENSAVERTIMEOUT,
               Value, ref nullVar, SPIF_SENDWININICHANGE);
        }

        // Returns TRUE if the screen saver is actually running
        public static bool GetScreenSaverRunning()
        {
            bool isRunning = false;

            SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0,
               ref isRunning, 0);
            return isRunning;
        }

        // From Microsoft's Knowledge Base article #140723: 
        // http://support.microsoft.com/kb/140723
        // "How to force a screen saver to close once started 
        // in Windows NT, Windows 2000, and Windows Server 2003"

        public static void KillScreenSaver()
        {
            IntPtr hDesktop = OpenDesktop("Screen-saver", 0,
               false, DESKTOP_READOBJECTS | DESKTOP_WRITEOBJECTS);
            if (hDesktop != IntPtr.Zero)
            {
                EnumDesktopWindows(hDesktop, new
                   EnumDesktopWindowsProc(KillScreenSaverFunc),
                   IntPtr.Zero);
                CloseDesktop(hDesktop);
            }
            else
            {
                PostMessage(GetForegroundWindow(), WM_CLOSE,
                   0, 0);
            }
        }

        private static bool KillScreenSaverFunc(IntPtr hWnd,
           IntPtr lParam)
        {
            if (IsWindowVisible(hWnd))
                PostMessage(hWnd, WM_CLOSE, 0, 0);
            return true;
        }
    }
}
