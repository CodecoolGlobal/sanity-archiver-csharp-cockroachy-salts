using System;

namespace SanityArchiver
{
    public class DirectoryItem
    {
        public DirectoryItemType Type { get; set; }

        public string FullPath { get; set; }

        public string Name =>
            Type == DirectoryItemType.Drive ? FullPath : DirectoryStructure.GetFileFolderName(FullPath);

        public string Size { get; set; }

        public bool IsHidden { get; set; }

        public DateTime Created { get; set; }

        public bool IsChecked { get; set; }

        public string Extension { get; set; }
    }
}