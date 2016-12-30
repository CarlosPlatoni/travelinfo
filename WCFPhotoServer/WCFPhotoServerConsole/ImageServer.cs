using System;
using System.IO;

namespace WCFPhotoServerConsole
{
    // implement the service contract
    public class Service : IImageServer
    {
        public Stream GetImage()
        {
            Console.WriteLine("Incoming Call");

            PhotoDirectoryCache.Initialise(@"\\sarge\media\pictures", "20*,19*");
            string filename = PhotoDirectoryCache.GetNextPhoto();
            Console.WriteLine("Filename {0}", filename);
            using (FileStream fileStream = File.OpenRead(filename))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                Console.WriteLine("Call exit");
                return memStream;
            }
        }

        public string GetImageDetails()
        {
            return string.Empty;
        }
    }
}
