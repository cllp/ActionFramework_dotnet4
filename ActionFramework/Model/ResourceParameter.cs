using ActionFramework.Enum;
using ActionFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Model
{
    public class ResourceParameter : IParameter
    {
        private DateTime loadDate = DateTime.Now;
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public ResourceFileType FileType { get; set; }
        public string FileExtension { get; set; }
        public ResourceOrigin Origin { get; set; }
        public string Description { get; set; }
        public string CompressedFile { get; set; }

        public DateTime LoadDate
        {
            get { return loadDate; }
            set { loadDate = value; }
        }

        public byte[] ResourceByte
        {
            get
            {
                if (!string.IsNullOrEmpty(this.CompressedFile))
                    return ActionFactory.Compression.DecompressFile(this.CompressedFile);
                else
                    throw new Exception("ResourceParameter has no compressed filestring. Can not get ResourceFile from empty string.");
            }
        }

        public Stream ResourceStream
        {
            get
            {
                Stream stream = new MemoryStream(this.ResourceByte);
                return stream;
            }
        }

    }
}
