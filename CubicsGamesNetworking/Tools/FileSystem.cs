using ProtoBuf;
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

        public static T DeserializeFromFile<T>(string filepath)
        {
            using (var file = File.OpenRead(filepath)) {
               return Serializer.Deserialize<T>(file);
            }
        }

        public static bool SerializeToFile(object obj,string destination)
        {
            try
            {
                using (var file = File.Create(destination))
                {
                    Serializer.Serialize(file, obj);
                }
                return true;
            }catch (Exception e) { };
            return false;
        }

        public static string SerializeToString<T>(this T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<T>(ms, obj);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static T DeserializeFromString<T>(this string txt)
        {
            byte[] arr = Convert.FromBase64String(txt);
            using (MemoryStream ms = new MemoryStream(arr))
            {
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }
    }
}
