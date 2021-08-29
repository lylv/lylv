using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XFileCopyLib
{
    public class FileCopier
    {
        public static void CopyTo(string sourceFile, string destFile, bool overwrite = false, Action<double> progressCallback = null)
        {
            var source = new FileInfo(sourceFile);
            if (!source.Exists) throw new FileNotFoundException($"File {source.FullName} does not exists.", source.FullName);
            var dest = new FileInfo(destFile);
            if (!dest.Directory.Exists) throw new DirectoryNotFoundException($"Directory {dest.Directory.FullName} does not exists.");
            if (!overwrite && dest.Exists)
            {
                throw new Exception($"Can not copy because {dest.FullName} existed.");
            }
            var copier = new XCopyFile();
            copier.CopyTo(source, dest, overwrite, progressCallback);
        }

        public static void MoveTo(string sourceFile, string destFile, bool overwrite = false, Action<double> progressCallback = null)
        {
            var source = new FileInfo(sourceFile);
            if (!source.Exists) throw new FileNotFoundException($"File {source.FullName} does not exists.", source.FullName);
            var dest = new FileInfo(destFile);
            if (!dest.Directory.Exists) throw new DirectoryNotFoundException($"Directory {dest.Directory.FullName} does not exists.");
            if (!overwrite && dest.Exists)
            {
                throw new Exception($"Can not move because {dest.FullName} existed.");
            }
            var copier = new XCopyFile();
            copier.MoveTo(source, dest, overwrite, progressCallback);
        }

        public static void CopyTo(FileInfo source, FileInfo dest, bool overwrite = false, Action<double> progressCallback = null)
        {
            if (!source.Exists) throw new FileNotFoundException($"File {source.FullName} does not exists.", source.FullName);
            if (!dest.Directory.Exists) throw new DirectoryNotFoundException($"Directory {dest.Directory.FullName} does not exists.");
            if (!overwrite && dest.Exists)
            {
                throw new Exception($"Can not copy because {dest.FullName} existed.");
            }
            var copier = new XCopyFile();
            copier.CopyTo(source, dest, overwrite, progressCallback);
        }

        public static void MoveTo(FileInfo source, FileInfo dest, bool overwrite = false, Action<double> progressCallback = null)
        {
            if (!source.Exists) throw new FileNotFoundException($"File {source.FullName} does not exists.", source.FullName);
            if (!dest.Directory.Exists) throw new DirectoryNotFoundException($"Directory {dest.Directory.FullName} does not exists.");
            if (!overwrite && dest.Exists)
            {
                throw new Exception($"Can not move because {dest.FullName} existed.");
            }
            var copier = new XCopyFile();
            copier.MoveTo(source, dest, overwrite, progressCallback);
        }

    }

    public class XCopyFile
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName,
           CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref Int32 pbCancel,
           CopyFileFlags dwCopyFlags);

        delegate CopyProgressResult CopyProgressRoutine(
        long TotalFileSize,
        long TotalBytesTransferred,
        long StreamSize,
        long StreamBytesTransferred,
        uint dwStreamNumber,
        CopyProgressCallbackReason dwCallbackReason,
        IntPtr hSourceFile,
        IntPtr hDestinationFile,
        IntPtr lpData);

        int pbCancel;

        enum CopyProgressResult : uint
        {
            PROGRESS_CONTINUE = 0,
            PROGRESS_CANCEL = 1,
            PROGRESS_STOP = 2,
            PROGRESS_QUIET = 3
        }

        enum CopyProgressCallbackReason : uint
        {
            CALLBACK_CHUNK_FINISHED = 0x00000000,
            CALLBACK_STREAM_SWITCH = 0x00000001
        }

        [Flags]
        enum CopyFileFlags : uint
        {
            COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
            COPY_FILE_RESTARTABLE = 0x00000002,
            COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
            COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008
        }

        private Action<double> _progressCallback;
        private bool _completed;
        public XCopyFile()
        {

        }

        public void CopyTo(FileInfo source, FileInfo destination, bool overwrite = false, Action<double> progressCallback = null)
        {
            this._progressCallback = progressCallback;
            if (destination.Exists)
            {
                if (overwrite)
                {
                    destination.Delete();
                }
                else
                {
                    throw new Exception($"{destination.FullName} existed.");
                }
            }

            var callback = new CopyProgressRoutine(this.CopyProgressHandler);
            CopyFileEx(source.FullName, destination.FullName, callback, IntPtr.Zero, ref pbCancel, CopyFileFlags.COPY_FILE_RESTARTABLE);

        }

        public void MoveTo(FileInfo source, FileInfo destination, bool overwrite = false, Action<double> progressCallback = null)
        {
            this._progressCallback = progressCallback;            
            if (destination.Exists)
            {
                if (overwrite)
                {
                    destination.Delete();
                }
                else
                {
                    throw new Exception($"{destination.FullName} existed.");
                }
            }            

            var callback = new CopyProgressRoutine(this.CopyProgressHandler);
            CopyFileEx(source.FullName, destination.FullName, callback, IntPtr.Zero, ref pbCancel, CopyFileFlags.COPY_FILE_RESTARTABLE);

            destination.Refresh();
            if (this._completed && destination.Exists)
            {
                source.Delete();
            }

        }

        private CopyProgressResult CopyProgressHandler(long total, long transferred, long streamSize, long StreamByteTrans, uint dwStreamNumber,
            CopyProgressCallbackReason reason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData)
        {
            double dProgress = (transferred / (double)total) * 100.0;
            if (this._progressCallback != null) this._progressCallback(dProgress);
            if (dProgress >= 100) _completed = true;
            return CopyProgressResult.PROGRESS_CONTINUE;
        }

    }

}
