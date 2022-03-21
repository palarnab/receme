using System;
using Command.Core;
using CaptureDesktopWithCursor;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace BasicCommands
{
    public class ScreenshotCommand : ICommand
    {
        private string name;

        public ScreenshotCommand()
        {
            name = "screenshot";
        }

        public string Run(string options)
        {
            ScreenCapturer c = new ScreenCapturer();
            Bitmap b = c.CaptureDesktop();

            string path = Path.Combine(Environment.CurrentDirectory + "\\RequestedData", Guid.NewGuid().ToString() + ".png");
            b.Save(path);

            b.Dispose();

            return path;
        }

        public override string ToString()
        {
            return name;
        }
    }
}

namespace CaptureDesktopWithCursor
{
    internal class ScreenCapturer
    {
        internal Bitmap CaptureDesktop()
        {
            IntPtr bitmap;
            Bitmap result = null;
            IntPtr deviceContext = Win32Utilities.GetDC(Win32Utilities.GetDesktopWindow());
            IntPtr memoryDeviceContext = GDIUtilities.CreateCompatibleDC(deviceContext);

            int width = Win32Utilities.GetSystemMetrics(Win32Utilities.SM_CXSCREEN);
            int height = Win32Utilities.GetSystemMetrics(Win32Utilities.SM_CYSCREEN);

            bitmap = GDIUtilities.CreateCompatibleBitmap(deviceContext, width, height);

            if (bitmap != IntPtr.Zero)
            {
                IntPtr oldHandle = (IntPtr)GDIUtilities.SelectObject(memoryDeviceContext, bitmap);

                GDIUtilities.BitBlt(memoryDeviceContext, 0, 0, width, height, deviceContext, 0, 0, GDIUtilities.SRCCOPY);

                GDIUtilities.SelectObject(memoryDeviceContext, oldHandle);
                GDIUtilities.DeleteDC(memoryDeviceContext);
                Win32Utilities.ReleaseDC(Win32Utilities.GetDesktopWindow(), deviceContext);
                result = System.Drawing.Image.FromHbitmap(bitmap);
                GDIUtilities.DeleteObject(bitmap);
                // GC.Collect();
            }

            return result;      
        }
    }

    internal class Win32Utilities
    {
        #region Members

        internal const int SM_CXSCREEN = 0;
        internal const int SM_CYSCREEN = 1;

        internal const Int32 CURSOR_SHOWING = 0x00000001;

        [StructLayout(LayoutKind.Sequential)]
        internal struct ICONINFO
        {
            internal bool fIcon;
            internal Int32 xHotspot;
            internal Int32 yHotspot;
            internal IntPtr hbmMask;
            internal IntPtr hbmColor;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            internal Int32 x;
            internal Int32 y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CURSORINFO
        {
            internal Int32 cbSize;
            internal Int32 flags;
            internal IntPtr hCursor;
            internal POINT ptScreenPos;
        }

        #endregion

        #region Methods

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        internal static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        internal static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        internal static extern int GetSystemMetrics(int abc);

        [DllImport("user32.dll", EntryPoint = "GetWindowDC")]
        internal static extern IntPtr GetWindowDC(Int32 ptr);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        internal static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);


        [DllImport("user32.dll", EntryPoint = "GetCursorInfo")]
        internal static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll", EntryPoint = "CopyIcon")]
        internal static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport("user32.dll", EntryPoint = "GetIconInfo")]
        internal static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);


        #endregion
    }

    internal class GDIUtilities
    {
        #region Members
        internal const int SRCCOPY = 13369376;
        #endregion

        #region Methods
        [DllImport("gdi32.dll", EntryPoint = "CreateDC")]
        internal static extern IntPtr CreateDC(IntPtr lpszDriver, string lpszDevice, IntPtr lpszOutput, IntPtr lpInitData);

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        internal static extern IntPtr DeleteDC(IntPtr hDc);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        internal static extern IntPtr DeleteObject(IntPtr hDc);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        internal static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest,
                                         int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        internal static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        #endregion
    }
}
