using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace TreeSize
{
    public class FillFile
    {
        public string path;
        public File file;
        public FillFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException();
            this.path = path;
            if (!Directory.Exists(path))
                throw new ArgumentException("The " + path + " does not exist.");
        }
        public void GetFile()
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            long size = GetDirSizeOnDisk(directory);
            double persent = Parent(size, size);
            File file = new File()
            {
                Path = path,
                Name = directory.Name,
                Size = BytesToString(GetDirSize(directory)),
                Allocated = BytesToString(size),
                Files = GetNumberOfAllFiles(path),
                Folders = GetNumberOfFolders(path),
                Percent = Math.Round(persent, 2),
            Modified = directory.LastWriteTime
        };
            this.file = file;
            file.AllFiles = AllFilesList(path, size);
            this.file = file;
        }

        public  List<File> AllFilesList(string path, long perentSize)
        {
            List<File> result = new List<File>();
            string[] allfiles = Directory.GetFiles(path);
            string[] folders = Directory.GetDirectories(path);

            try
            {
                foreach (string file in folders)
                {
                    FillFile newFile = new FillFile(file);
                    newFile.GetFile();
                    var files = newFile.file;
                    DirectoryInfo directory = new DirectoryInfo(file);
                    long size = GetDirSizeOnDisk(directory);
                    double persent = Parent(perentSize, size);
                    var f = new File
                    {
                        Path = file,
                        Name = directory.Name,
                        Size = BytesToString(GetDirSize(directory)),
                        Allocated = BytesToString(size),
                        Files = GetNumberOfAllFiles(file),
                        Folders = GetNumberOfFolders(file),
                        Percent = Math.Round(persent, 2),
                        Modified = directory.LastWriteTime,
                        AllFiles = files.AllFiles
                    };

                    result.Add(f);
                }
                if (allfiles.Length==1)
                {
                    var list = GetFiles(allfiles, perentSize);
                    result.Add(list[0]);
                }
                else if (allfiles.Length > 1)
                {
                    DirectoryInfo directory = new DirectoryInfo(path);
                
                    long allocated = 0;
                    foreach (var file in allfiles)
                    {
                        allocated += GetFileSizeOnDisk(file);
                    }
                    double persent = Parent(perentSize, allocated);
                    result.Add(new File
                    {
                        Path = path,
                        Name = "[" + GetNumberOfFiles(path) + " Files]",
                        Size = BytesToString(GetFilesSize(directory)),
                        Allocated = BytesToString(allocated),
                        Files = GetNumberOfFiles(path),
                        Folders = 0,
                        Percent = Math.Round(persent, 2),
                        Modified = LastTime(allfiles),
                        AllFiles = GetFiles(allfiles, allocated)
                    });
                }
            }
            catch
            {

            }

            return result;
        }

        private static double Parent(long parent, long children)
        {
            if (parent == children)
                return 100;

            double percent = ((double)children / parent) * 100;

       

            return  percent;
        }
        private static List<File> GetFiles(string[] allfiles, long perentSize)
        {
            List<File> result = new List<File>();
            foreach (string file in allfiles)
            {
                var fileInfo = new System.IO.FileInfo(file);
                long size = GetFileSizeOnDisk(file);
                double persent = Parent(perentSize, size);
                result.Add(new File
                {
                    Path = file,
                    Name = fileInfo.Name,
                    Size = BytesToString(fileInfo.Length),
                    Allocated = BytesToString(size),
                    Files = 0,
                    Folders = 0,
                    Percent = Math.Round(persent, 2),
                    Modified = fileInfo.LastWriteTime
                });
            }
            return result;
        }
        private static DateTime LastTime(string[] allfiles)
        {
            DateTime? date = null;
            foreach (string file in allfiles)
            {
                var fileInfo = new System.IO.FileInfo(file);
                if (date == null || date < fileInfo.LastWriteTime)
                {
                    date = fileInfo.LastWriteTime;
                }
            }

            return (DateTime)date;
        }
        private static bool IsAccessible(FileSystemInfo fsInfo)
        {
            return (
                fsInfo.Exists &&
                ((fsInfo.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
            );
        }
        private static long GetFilesSize(DirectoryInfo dirInfo)
        {
            long size = 0;
            FileInfo[] fis = dirInfo.GetFiles();
         
            foreach (FileInfo fi in fis)
            {
                if (IsAccessible(fi))
                {
                    size += fi.Length;
                }
            }

            return size;
        }
        private static long GetDirSizeOnDisk(DirectoryInfo dirInfo)
        {

           /* if (dirInfo.Parent == null)
            {
                DriveInfo d = new DriveInfo(dirInfo.Name);
                long s = d.TotalSize;
                return s;
            }
            */
            long size =0;
            FileInfo[] fis = dirInfo.GetFiles();

            foreach (FileInfo fi in fis)
            {
                if (IsAccessible(fi))
                {
                    size += GetFileSizeOnDisk(fi.FullName);
                }
            }
        
              

            DirectoryInfo[] dis = dirInfo.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                if (IsAccessible(di))
                {
                    size += GetDirSizeOnDisk(di);
                }
            }

            return size;
        }
        public static long GetFileSizeOnDisk(string file)
        {
            FileInfo info = new FileInfo(file);
            uint dummy, sectorsPerCluster, bytesPerSector;
            int result = GetDiskFreeSpaceW(info.Directory.Root.FullName, out sectorsPerCluster, out bytesPerSector, out dummy, out dummy);
            if (result == 0) throw new Win32Exception();
            uint clusterSize = sectorsPerCluster * bytesPerSector;
            uint hosize;
            uint losize = GetCompressedFileSizeW(file, out hosize);
            long size;
            size = (long)hosize << 32 | losize;
            return ((size + clusterSize - 1) / clusterSize) * clusterSize;

        }

        [DllImport("kernel32.dll")]
        static extern uint GetCompressedFileSizeW([In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
            [Out, MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);

        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
        static extern int GetDiskFreeSpaceW([In, MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName,
            out uint lpSectorsPerCluster, out uint lpBytesPerSector, out uint lpNumberOfFreeClusters,
            out uint lpTotalNumberOfClusters);

        private static long GetDirSize(DirectoryInfo dirInfo)
        {
            long size = GetFilesSize(dirInfo);
     
           /* if (dirInfo.Parent == null)
            {
                DriveInfo d = new DriveInfo(dirInfo.Name);
                long s = d.TotalSize;
                return s;
            }
  */
            DirectoryInfo[] dis = dirInfo.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                if (IsAccessible(di))
                {
                    size += GetDirSize(di);
                }
            }

            return size;
        }
        private static String BytesToString(long byteCount)
        {
            string[] suf = { "Byt", "KB", "MB", "GB", "TB", "PB", "EB" }; if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
        private static int GetNumberOfFolders(string start_path)
        {
            int sum = 0;
            try
            {
                string[] folders = Directory.GetDirectories(start_path);
                foreach (string folder in folders)
                {
                    sum++;
                    sum = sum + GetNumberOfFolders(folder);
                }
            }
            catch
            {

            }
            return sum;
        }
        private static int GetNumberOfFiles(string start_path)
        {
            int sum = (Directory.GetFiles(start_path)).Length;
            return sum;
        }
        private static int GetNumberOfAllFiles(string start_path)
        {
            int sum = 0;
            try
            {
                string[] folders = Directory.GetDirectories(start_path);
                foreach (string folder in folders)
                {
                    sum += GetNumberOfAllFiles(folder);
                }

                sum += GetNumberOfFiles(start_path);
            }
            catch
            {

            }

            return sum;
        }
    }
}
