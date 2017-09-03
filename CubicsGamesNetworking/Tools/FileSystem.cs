using System;
using System.IO;

namespace CubicsGamesNetworking.Tools
{
    public static class FileSystem
    {
        public static bool CreateFolderIfNotFound(string fold)
        {
            bool ret = false;
            try
            {
                if (!Directory.Exists(fold))
                {
                    Directory.CreateDirectory(fold);
                    ret = true;
                }
                else
                {
                    ret = true;
                }
            }finally{ }
            return ret;
        }
    }
}
