// DIBSection
// Copyright (c) 2012 Felioguine Serguei - All rights reserved.


using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ForexStrategyBuilder.Utils
{
    #region Работа с Bitmap
    public class DIBSection
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        public const int SRCCOPY = 0x00CC0020;
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdcDest, int destX, int destY, int destWidth, int destHeight, IntPtr hdcSource, int sourceX, int sourceY, uint rasterOp);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        private const int DIB_RGB_COLORS = 0;
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
        private class ЗаголовокBITMAP
        {
            [FieldOffset(0)]
            public UInt32 biSize;
            [FieldOffset(4)]
            public Int32 biWidth;
            [FieldOffset(8)]
            public Int32 biHeight;
            [FieldOffset(12)]
            public Int16 biPlanes;
            [FieldOffset(14)]
            public Int16 biBitCount;
            [FieldOffset(16)]
            public UInt32 biCompression;
            [FieldOffset(20)]
            public UInt32 biSizeImage;
            [FieldOffset(24)]
            public Int32 biXPelsPerMeter;
            [FieldOffset(28)]
            public Int32 biYPelsPerMeter;
            [FieldOffset(32)]
            public UInt32 biClrUsed;
            [FieldOffset(36)]
            public UInt32 biClrImportant;
            public ЗаголовокBITMAP(int nWidth, int nHeight, int nBPP)
            {
                biSize = 40;
                biWidth = nWidth;
                biHeight = nHeight;
                biPlanes = 1;
                biBitCount = (Int16)nBPP;
                biCompression = biSizeImage = biClrUsed = biClrImportant = 0;
                biXPelsPerMeter = biYPelsPerMeter = 0;
            }
        }
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
        private class BITMAPINFO
        {
            [FieldOffset(0)]
            public ЗаголовокBITMAP bmih;
            [FieldOffset(40)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt32[] dwColors;
            public BITMAPINFO(int nWidth, int nHeight, int nBPP)
            {
                bmih = new ЗаголовокBITMAP(nWidth, nHeight, nBPP);
                dwColors = new UInt32[256];
            }
        }
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDIBSection(IntPtr hdc, [In,
        MarshalAs(UnmanagedType.LPStruct)] BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);
        public static IntPtr Create24bppDIBSection(int nWidth, int nHeight)
        {
            BITMAPINFO bmi = new BITMAPINFO(nWidth, nHeight, 24);
            IntPtr pBits;
            return CreateDIBSection(IntPtr.Zero, bmi, DIB_RGB_COLORS, out pBits,
            IntPtr.Zero, 0);
        }
        public static void DrawOnPaint(Graphics graphics, Bitmap bitmap, int nWidth, int nHeight)
        {
            IntPtr hBitmap = IntPtr.Zero;
            hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
            IntPtr hdcForm = graphics.GetHdc();
            IntPtr hdcScreen = GetDC(IntPtr.Zero);
            IntPtr hdcMemory = CreateCompatibleDC(hdcScreen);
            IntPtr hOldBitmap = SelectObject(hdcMemory, hBitmap);
            IntPtr hMemDC = CreateCompatibleDC(hdcScreen);
            IntPtr hDIBSection = Create24bppDIBSection(bitmap.Width, bitmap.Height);
            IntPtr hOld = SelectObject(hMemDC, hDIBSection);
            BitBlt(hMemDC, 0, 0, bitmap.Width, bitmap.Height, hdcMemory, 0, 0, SRCCOPY);
            BitBlt(hdcForm, 0, 0, bitmap.Width, bitmap.Height, hMemDC, 0, 0, SRCCOPY);
            SelectObject(hMemDC, hOld);
            DeleteDC(hMemDC);
            DeleteObject(hDIBSection);
            ReleaseDC(IntPtr.Zero, hdcScreen);
            DeleteObject(SelectObject(hdcMemory, hOldBitmap));
            DeleteDC(hdcMemory);
            graphics.ReleaseHdc(hdcForm);
            bitmap.Dispose();
        }
    }
    #endregion
}