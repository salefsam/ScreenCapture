using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;

namespace ScreenCapture
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Starting Timer " + DateTime.Now.ToString("g") + ". Timer Set To Every 120 Seconds.");
            Console.WriteLine(@"Saving Screenshots to 'C:\ScreenCapure\' ");
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 120000;
            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
            Console.Read();
        }

        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimeSpan start = new TimeSpan(9, 0, 0);
            TimeSpan end = new TimeSpan(17, 0, 0);
            TimeSpan now = DateTime.Now.TimeOfDay;
            if((now>start) && (now <end))
            {
                ScreenCapture sc = new ScreenCapture();
                Image img = sc.CaptureScreen();
                sc.CaptureScreenToFile(FolderPath() + ".png", ImageFormat.Png);
                Console.WriteLine("Screen Captured. " + now + ".png");
            }
        }

        static string FolderPath()
        {
            var currentDate = DateTime.Now.Date.ToString("MM_dd_yyyy");
            if(!Directory.Exists("C:\\ScreenCapture\\"+currentDate))
            {
                Directory.CreateDirectory("C:\\ScreenCapture\\"+currentDate);
            }

            var currentTime = DateTime.Now.TimeOfDay.ToString("hhmmss");
            currentTime = currentTime.Insert(2, "_");
            currentTime = currentTime.Insert(5, "_");

            return "C:\\ScreenCapture\\" + currentDate + "\\"+currentTime;
        }
    }

    public class ScreenCapture
    {
        public Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }

        public Image CaptureWindow(IntPtr handle)
        {
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            IntPtr hold = GDI32.SelectObject(hdcDest, hBitmap);
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
            GDI32.SelectObject(hdcDest, hold);
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            Image img = Image.FromHbitmap(hBitmap);
            GDI32.DeleteObject(hBitmap);
            return img;
        }

        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(handle);
            img.Save(filename, format);
        }

        public void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image img = CaptureScreen();
            img.Save(filename, format);
        }

        private class GDI32
        {
            public const int SRCCOPY = 0x00CC0020;
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nxDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int NYSrc, int dwRop);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);

            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hwnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hwnd, ref RECT rect);
        }
    }
}