using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTool4.Model
{
    public class BitmapHeader
    {
        public ushort FileType { get; set; }
        public uint FileSize { get; set; }
        public ushort Reserved1 { get; set; }
        public ushort Reserved2 { get; set; }
        public uint DataOffset { get; set; }

        public uint HeaderSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ushort Planes { get; set; }
        public ushort BitsPerPixel { get; set; }
        public uint Compression { get; set; }
        public uint ImageSize { get; set; }
        public int XPixelsPerMeter { get; set; }
        public int YPixelsPerMeter { get; set; }
        public uint ColorsUsed { get; set; }
        public uint ImportantColors { get; set; }

        public static BitmapHeader LoadBitmapHeader(string filePath)
        {
            BitmapHeader header = new BitmapHeader();

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                header.FileType = reader.ReadUInt16();
                header.FileSize = reader.ReadUInt32();
                header.DataOffset = reader.ReadUInt32();

                header.HeaderSize = reader.ReadUInt32();
                header.Width = reader.ReadInt32();
                header.Height = reader.ReadInt32();
                header.Planes = reader.ReadUInt16();
                header.BitsPerPixel = reader.ReadUInt16();
                header.Compression = reader.ReadUInt32();
                header.ImageSize = reader.ReadUInt32();
            }

            return header;
        }
    }
}