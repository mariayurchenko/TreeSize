using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeSize
{
    public class File
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Allocated { get; set; }
        public int Files { get; set; }
        public int Folders { get; set; }
        public double Percent { get; set; }
        public DateTime Modified { get; set; }
        public List<File> AllFiles { get; set; }
    }
}
