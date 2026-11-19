using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI
{
    public static class FilePaths
    {
        public const string StorageFileName = "Tradeckr_Storage.db";
        public const string LogFileName = "Tradeckr_Log.txt";

        public readonly static string StoragePath = Path.Combine(FileSystem.AppDataDirectory, FilePaths.StorageFileName);
        public readonly static string LogFilePath = Path.Combine(FileSystem.AppDataDirectory, FilePaths.LogFileName);
    }
}
