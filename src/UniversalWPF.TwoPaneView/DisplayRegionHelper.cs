/*
Based on a port of TwoPaneView control in the WinUI Library: https://github.com/microsoft/microsoft-ui-xaml/
    MIT License

    Copyright (c) Microsoft Corporation. All rights reserved.

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace UniversalWPF
{
    internal class DisplayRegionHelper
    {
        public static Rect WindowRect(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero || !GetWindowRect(hwnd, out RECT rect))
                return Rect.Empty;
            return rect.AsRect();
        }

        internal static DisplayRegionHelperInfo GetRegionInfo(IntPtr hwnd)
        {
            DisplayRegionHelperInfo info = new DisplayRegionHelperInfo(TwoPaneViewMode.SinglePane);
            
            if (hwnd != IntPtr.Zero && s_isGetContentRectsSupported)
            {
                List<Rect> rects = GetRegions(hwnd);
                if (rects != null && rects.Count == 2)
                {
                    info.Regions[0] = rects[0];
                    info.Regions[1] = rects[1];

                    // Determine orientation. If neither of these are true, default to doing nothing.
                    if (info.Regions[0].X < info.Regions[1].X && info.Regions[0].Y == info.Regions[1].Y)
                    {
                        // Double portrait
                        info.Mode = TwoPaneViewMode.Wide;
                    }
                    else if (info.Regions[0].X == info.Regions[1].X && info.Regions[0].Y < info.Regions[1].Y)
                    {
                        // Double landscape
                        info.Mode = TwoPaneViewMode.Tall;
                    }
                }
            }
            return info;
        }

        static bool s_isGetContentRectsSupported = true;

        private static List<Rect> GetRegions(IntPtr hwnd)
        {
            try
            {
                uint count = 2;
                RECT[] regions = new RECT[2];
                bool result = GetContentRects(hwnd, ref count, regions);
                if (result)
                {
                    List<Rect> rects = new List<Rect>((int)count);
                    for (int i = 0; i < (int)count; i++)
                    {
                        rects.Add(regions[i].AsRect());
                    }
                    return rects;
                }
            }
            catch (EntryPointNotFoundException) // Expected to throw on older OS
            {
                s_isGetContentRectsSupported = false;
            }
            return null;
        }


        [DllImport("user32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        private static extern bool GetContentRects(IntPtr hwnd, ref UInt32 count, [MarshalAs(UnmanagedType.LPArray)] RECT[] rects);

        /// <summary>
        /// Retrieves the dimensions of the bounding rectangle of the specified window.
        /// The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lpRect"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
            public Rect AsRect() => new Rect(Left, Top, Right - Left, Top - Bottom);
        }
    }
}
