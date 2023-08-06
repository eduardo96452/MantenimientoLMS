/*
*  
*  Copyright (C) 2010-2011 University College Ghent.
*  
*  For a full list of contributors, see "credits.txt".
*  
*  This file is part of LMS Desktop Assistant.
*  
*  LMS Desktop Assistant is free software: you can redistribute it and/or modify
*  it under the terms of the GNU General Public License as published by
*  the Free Software Foundation, either version 3 of the License, or
*  (at your option) any later version.
*  
*  LMS Desktop Assistant is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU General Public License for more details.
*  
*  You should have received a copy of the GNU General Public License
*  along with LMS Desktop Assistant. If not, see <http://www.gnu.org/licenses/>.
*  
*/

using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace lmsda.gui.treeview
{
    /// <summary>
    ///     Source: http://www.codingforums.com/showthread.php?t=98374
    /// </summary>
    class ShellIcon
    {
        [StructLayout(LayoutKind.Sequential)]
		public struct SHFILEINFO 
		{
			public IntPtr hIcon;
			public IntPtr iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public String szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public String szTypeName;
		};

		class Win32
		{
			public const uint SHGFI_ICON = 0x100;
			public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
			public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

			[DllImport("shell32.dll")]
			public static extern IntPtr SHGetFileInfo(String pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
		}

		public ShellIcon()
		{

		}

		public static Icon GetSmallIcon(String fileName)
		{
            Icon icn = null;
			IntPtr hImgSmall; //the handle to the system image list
			SHFILEINFO shinfo = new SHFILEINFO();

			//Use this to get the small Icon
			hImgSmall = Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);

			//The icon is returned in the hIcon member of the shinfo struct
            try
            {
                icn=System.Drawing.Icon.FromHandle(shinfo.hIcon);
            }
            catch
            {
                Bitmap iconbmp = new Bitmap( 16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                IntPtr Hicon = iconbmp.GetHicon();
                // Create a new icon from the handle. 
                icn = Icon.FromHandle(Hicon);
            }
            return icn;

		}

		public static Icon GetLargeIcon(String fileName)
		{
			IntPtr hImgLarge; //the handle to the system image list
			SHFILEINFO shinfo = new SHFILEINFO();

			//Use this to get the large Icon
			hImgLarge = Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);

			//The icon is returned in the hIcon member of the shinfo struct
			return System.Drawing.Icon.FromHandle(shinfo.hIcon);                
		}
    }
}
