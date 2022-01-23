using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Bo4kaBass.Model
{
    public class MetaDataMP3File
    {
       
        public string Title { get; set; }
        public byte[] Image { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Year { get; set; }
        public string Comment { get; set; }
        public string Genre { get; set; }
        public string Bitrate { get; set; }
        public string Duration { get; set; }
        public string Path { get; set; }
    }
}
