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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Security.AccessControl;


namespace lmsda.persistence.document
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///     
    ///     Exposes a System.IO.Stream around a file, supporting both synchronous and
    ///     asynchronous read and write operations.
    ///     When used to read a PDF file, this class filters out date stamps and generated IDs,
    ///     allowing the output to be used for file comparison.
    /// </summary>
    class PDFFilterFileStream : FileStream
    {
        private const String CREATION_DATE_PATTERN = "/CreationDate(D:..............)";
        private const String MOD_DATE_PATTERN      = "/ModDate(D:..............)";
        private const String R_ID_PATTERN          = "R/ID[<................................><................................>]";

        // all of the original constructors; just to be safe.
        [Obsolete("This constructor has been deprecated.  Please use new FileStream(SafeFileHandle handle, FileAccess access) instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public PDFFilterFileStream(IntPtr handle, FileAccess access) : base(handle, access) { }
        public PDFFilterFileStream(SafeFileHandle handle, FileAccess access) : base(handle, access) { }
        public PDFFilterFileStream(string path, FileMode mode) : base(path, mode) { }
        [Obsolete("This constructor has been deprecated.  Please use new FileStream(SafeFileHandle handle, FileAccess access) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public PDFFilterFileStream(IntPtr handle, FileAccess access, bool ownsHandle) : base(handle, access, ownsHandle) { }
        public PDFFilterFileStream(SafeFileHandle handle, FileAccess access, int bufferSize) : base(handle, access, bufferSize) { }
        public PDFFilterFileStream(string path, FileMode mode, FileAccess access):base(path, mode, access) { }
        [Obsolete("This constructor has been deprecated.  Please use new FileStream(SafeFileHandle handle, FileAccess access, int bufferSize) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public PDFFilterFileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize):base(handle, access, ownsHandle, bufferSize) { }
        public PDFFilterFileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync):base(handle, access, bufferSize, isAsync) { }
        public PDFFilterFileStream(string path, FileMode mode, FileAccess access, FileShare share):base(path, mode, access, share) { }
        [Obsolete("This constructor has been deprecated.  Please use new FileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public PDFFilterFileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync):base(handle, access, ownsHandle, bufferSize, isAsync) { }
        public PDFFilterFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize):base(path, mode, access, share, bufferSize) { }
        public PDFFilterFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync):base(path, mode, access, share, bufferSize, useAsync) { }
        public PDFFilterFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options):base(path, mode, access, share, bufferSize, options) { }
        public PDFFilterFileStream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options):base(path, mode, rights, share, bufferSize, options) { }
        public PDFFilterFileStream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity):base(path, mode, rights, share, bufferSize, options, fileSecurity) { }

        public override int ReadByte()
        {
            byte[] buffer = new byte[1];
            Boolean byteRead=this.Read(buffer, 0, 1) == 1;
            return (byteRead ? buffer[0] : -1);
        }

        /// <summary>
        ///     Reads a block of bytes from the stream and writes the data in a given buffer, filtering out
        ///     time stamps and randomly generated ID numbers in PDF data to allow PDF comparison
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The byte offset in array at which the read bytes will be placed.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns>The total number of bytes read into the buffer. This might be less than the number of bytes requested if that number of bytes are not currently available, or zero if the end of the stream is reached.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            Int32 extraLengthBefore;
            Int32 extraLengthAfter;
            
            // Length of the longest string -1
            // R_ID_PATTERN is the longest string
            Int32 addedspace = R_ID_PATTERN.Length - 1;

            Int64 currentpos = this.Position;
            if (currentpos < addedspace)
                extraLengthBefore = Convert.ToInt32(currentpos);
            else
                extraLengthBefore = addedspace;

            if (currentpos+count >= this.Length)
            {
                extraLengthAfter = 0;
                count=Convert.ToInt32(this.Length - currentpos);
            }
            else if (currentpos + count + addedspace >= this.Length)
                extraLengthAfter = Convert.ToInt32(this.Length - (currentpos + count));
            else
                extraLengthAfter = addedspace;

            int bigLength = extraLengthBefore + count + extraLengthAfter;
            byte[] bigbuffer = new byte[bigLength];

            this.Position = currentpos-extraLengthBefore;
            int result = base.Read(bigbuffer,0,bigLength);
            
            // the actual search & replace function calls
            this.searchAndReplace(bigbuffer, CREATION_DATE_PATTERN, '.', '0', false);
            this.searchAndReplace(bigbuffer, MOD_DATE_PATTERN, '.', '0', false);
            this.searchAndReplace(bigbuffer, R_ID_PATTERN, '.', '0', true);

            Array.Copy(bigbuffer,extraLengthBefore,buffer,offset,count);
            this.Position = currentpos + result - extraLengthBefore - extraLengthAfter;

            return count;
        }

        private void searchAndReplace(byte[] buffer, String searchString, char wildcard, char replacechar, Boolean hexadecimalMatch)
        {
            for (int index = 0; index < buffer.Length - searchString.Length; index++)
            {
                Boolean matchFound = buffer.Length > 0;
                for (int z = 0; z < searchString.Length; z++)
                {
                    // aborts reading the buffer from the moment the last read character
                    // no longer matches the one in the search string
                    int currentval = buffer[z + index];
                    if (    !(searchString[z] != wildcard && currentval == Convert.ToInt32(searchString[z]))
                         && !(searchString[z] == wildcard && ((currentval >= 0x30 && currentval <= 0x39)
                                // hexadecimal: adds possibilities 'A' to 'F';
                                || (hexadecimalMatch && (currentval >= 0x41 && currentval <= 0x46)))))
                    {
                        matchFound = false;
                        break;
                    }
                }
                if (matchFound)
                    replaceMatch(buffer, index, searchString, wildcard, replacechar);
            }
        }

        private void replaceMatch(byte[] buffer, int index, String searchString, char wildcard, char replacechar)
        {
            byte replacevalue = Convert.ToByte(replacechar);

            for (int pos = 0; pos < searchString.Length; pos++)
            {
                if (searchString[pos] == wildcard)
                {
                    buffer[index+pos] = replacevalue;
                }
            }
        }
    }
}
