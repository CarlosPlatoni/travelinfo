using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WCFPhotoServerConsole
{
    public static class PhotoDirectoryCache
    {
        private static string PhotoDirectory { get; set; }
        private static List<string> DirectoryList { get; set; }
        private static List<string> PhotoList { get; set; }
        private static int CurrentDirectoryIndex { get; set; }
        private static int CurrentPhotoIndex { get; set; }
        private static string PhotoDirectoryChooser { get; set; }
        private static bool Initialised { get; set; }

        public static string GetNextPhoto()
        {
            string photofilename = Path.Combine(DirectoryList[CurrentDirectoryIndex], PhotoList[CurrentPhotoIndex]);
            CurrentPhotoIndex++;
            if (CurrentPhotoIndex > PhotoList.Count - 1)
            {
                GetNextDirectory();
            }

            return photofilename;
        }

        public static void Initialise(string photodirectory, string photodirectorychooser)
        {
            if (Initialised)
            {
                return;
            }

            Initialised = true;
            PhotoDirectory = photodirectory;
            PhotoDirectoryChooser = photodirectorychooser;
            BuildPhotoCache();
        }

        public static bool BuildPhotoCache()
        {
            try
            {
                DirectoryList = new List<string>();
                string[] patterns = PhotoDirectoryChooser.Split(',');
                foreach (string pattern in patterns)
                {
                    List<string> list = new List<string>(Directory.EnumerateDirectories(PhotoDirectory, pattern, SearchOption.AllDirectories));
                    DirectoryList.AddRange(list);
                }

                GetNextDirectory();
            }
            catch (Exception)
            {
                throw;
            }
            
            return true;
        }

        private static void GetNextDirectory()
        {
            PhotoList = new List<string>();
            while (PhotoList.Count == 0)
            {
                Random random = new Random();
                CurrentDirectoryIndex = random.Next(DirectoryList.Count);
                PhotoList = Directory.GetFiles(DirectoryList[CurrentDirectoryIndex], "*.jpg").OrderBy(x => x).ToList();
                CurrentPhotoIndex = 0;
            }
        }
    }
}
