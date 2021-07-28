using System;
using System.IO;

namespace Bot
{
    [Serializable]
    public class MediaResourceException : Exception
    {
        public MediaResourceException() : base()
        { }
        public MediaResourceException(string message) : base(message)
        { }
        public MediaResourceException(string message, Exception inner) : base(message, inner)
        { }
    }
    
    public static class MediaResources
    {
        // do not access directly.
        private static string _workingDir;
        public static string WorkingDirectory
        {
            get
            {
                if (_workingDir == null || _workingDir.Length <= 0)
                {
                    // Do not handle exceptions
                    RecalculateWorkingDir();
                }

                return _workingDir;
            }
            private set => _workingDir = value;
        }

        public static FileStream GetImage(string fileName, FileMode mode = FileMode.Open)
        {
            try
            {
                var filePath = Path.Combine(WorkingDirectory, fileName);
                var file = File.Open(filePath, mode);
                return file;
            }
            catch (Exception e)
            {
                throw new MediaResourceException("Could not access the file from the working directory.", e);
            }
        }

        public static void RecalculateWorkingDir()
        {
            try
            {
                var baseDir = Directory.GetCurrentDirectory();
                var workingDir = Path.GetFullPath("./Resources", baseDir);
                WorkingDirectory = workingDir;
            }
            catch (Exception e)
            {
                throw new MediaResourceException("Could not recalculate the working directory.", e);
            }
        }
    }
}