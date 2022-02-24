using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmPP.Web.Services
{
    public class CamBitmap
    {
        public CamBitmap(string path, string ext)
        {
            var directory = new DirectoryInfo(path);
            var files = directory.GetFiles();
            var myFiles = files.Where(f => f.Extension.Equals(ext));
            var myFile =  myFiles.OrderByDescending(f => f.LastWriteTime).ToList()[0];
            this.FilePath = myFile.FullName;
        }

        public string FilePath { get; set; }
        public Bitmap Bitmap { get; set; }
        public BinaryReader Reader {get;set;}
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
