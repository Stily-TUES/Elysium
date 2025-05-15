using System;
using System.IO;

namespace Editor.Utils
{
    public static class PathHelper
    {
        public static readonly string BasePath = GetEditorBasePath();

        private static string GetEditorBasePath()
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo directory = new DirectoryInfo(currentPath);

            while (directory != null && !directory.Name.Equals("Elysium", StringComparison.OrdinalIgnoreCase))
            {
                directory = directory.Parent;
            }

            return directory.FullName;
        }
    }
}
