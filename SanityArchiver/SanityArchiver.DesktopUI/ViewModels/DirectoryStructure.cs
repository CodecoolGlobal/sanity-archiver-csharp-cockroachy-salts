using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SanityArchiver.DesktopUI.ViewModels;

namespace SanityArchiver
{
    public class DirectoryStructure
    {
        public static List<DirectoryItem> GetLogicalDrives()
        {
            return System.IO.Directory.GetLogicalDrives()
                .Select(drive => new DirectoryItem { FullPath = drive, Type = DirectoryItemType.Drive }).ToList();
        }

        public static List<DirectoryItem> GetDirectoryContents(string fullPath)
        {
            var items = new List<DirectoryItem>();

            #region Get Folders

            try
            {
                var dirs = System.IO.Directory.GetDirectories(fullPath);

                if (dirs.Length > 0)
                {
                    items.AddRange(dirs.Select(dir => new DirectoryItem
                    { FullPath = dir, Type = DirectoryItemType.Folder }));
                }
            }
            catch
            {
                // ignored
            }

            #endregion


            return items;
        }

        public static List<DirectoryItemViewModel> GetDirectoryFiles(string fullPath)
        {
            var items = new List<DirectoryItemViewModel>();

            #region Get Files

            try
            {
                var fs = System.IO.Directory.GetFiles(fullPath);

                if (fs.Length > 0)
                {
                    foreach (var path in fs)
                    {
                        FileInfo fileInfo = new FileInfo(path);
                        items.Add(new DirectoryItemViewModel
                        {
                            FullPath = path,
                            Type = DirectoryItemType.File,
                            Created = fileInfo.CreationTime,
                            IsChecked = false,
                            IsHidden = fileInfo.Attributes.HasFlag(FileAttributes.Hidden),
                            Extension = Path.GetExtension(path),
                            Size = FileSizeFormatter1.FormatSize(fileInfo.Length),
                        });
                    }
                }
            }
            catch
            {
                // ignored
            }

            return items;

            #endregion
        }

        public static List<DirectoryItemViewModel> GetCheckedFiles(List<DirectoryItemViewModel> files)
        {
            if (files == null)
            {
                return null;
            }

            return (from file in files
                    where file.IsChecked
                    select file).ToList();
        }

        #region Helpers

        public static string GetFileFolderName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            var normalizedPath = path.Replace('/', '\\');

            var lastIndex = normalizedPath.LastIndexOf('\\');

            if (lastIndex <= 0)
            {
                return path;
            }

            return path.Substring(lastIndex + 1);
        }

        #endregion
    }

    /// <summary>
    /// Formats to the greates file suffix.
    /// </summary>
    public static class FileSizeFormatter1
    {
        // Load all suffixes in an array  
        static readonly string[] Suffixes =
            {"Bytes", "KB", "MB", "GB", "TB", "PB"};

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            return string.Format("{0:n1}{1}", number, Suffixes[counter]);
        }
    }
}