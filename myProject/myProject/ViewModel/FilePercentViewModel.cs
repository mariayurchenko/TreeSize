﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TreeSize;

namespace myProject.ViewModel
{
    public class FilePercentViewModel : Observable
    {
        private List<File> files;
        public FilePercentViewModel(List<File> list)
        {
            if (list != null)
            {
                files = new List<File> { };
                foreach (File file in list)
                {
                    files.Add(new File { Name = file.Path, Size = file.Size, Allocated = file.Allocated, Files = file.Files, Folders = file.Folders, Percent = file.Percent, Path = file.Path, Modified = file.Modified, AllFiles = CopyList(file.AllFiles) });
                }
                foreach (var file in files)
                {
                    file.Name = file.Percent + "% " + file.Path;
                    PercentName(file.AllFiles);
                }
            }
        }
        private void PercentName(List<File> file)
        {
            foreach (var f in file)
            {
                f.Name = f.Percent + "% " + f.Name;
                if (f.AllFiles != null)
                {
                    PercentName(f.AllFiles);
                }
            }
        }

        private List<File> CopyList(List<File> list)
        {
            List<File> newFiles = new List<File>();
            if (list != null)
            {
                foreach (File file in list)
                {
                    newFiles.Add(new File
                    {
                        Name = file.Name,
                        Size = file.Size,
                        Allocated = file.Allocated,
                        Files = file.Files,
                        Folders = file.Folders,
                        Percent = file.Percent,
                        Path = file.Path,
                        Modified = file.Modified,
                        AllFiles = CopyList(file.AllFiles)
                    });
                }
            }
            return newFiles;
        }


        public List<File> Files
        {
            get { return files; }
            set
            {
                files = value;
                OnPropertyChanged("Files");
            }
        }
    }
}
