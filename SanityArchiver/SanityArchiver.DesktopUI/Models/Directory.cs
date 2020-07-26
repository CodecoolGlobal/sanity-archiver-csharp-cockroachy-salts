using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanityArchiver.DesktopUI.Models
{
    public class Directory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Directory"/> class.
        /// Directory constructor
        /// </summary>
        public Directory()
        {
            Files = new List<File>();
            Directories = new List<Directory>();
        }
        /// <summary>
        /// Getter and setter for List of files
        /// </summary>
        public List<File> Files { get; private set; }
        /// <summary>
        /// Getter and setter for list of directories
        /// </summary>
        public List<Directory> Directories { get; private set; }
        /// <summary>
        /// Getter and setter for dir name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// File counter
        /// </summary>
        public int FileCount => Files.Count;

        /// <summary>
        /// Dir counter
        /// </summary>
        public int DirectoryCount => Directories.Count;

        /// <summary>
        /// To string method for dir names
        /// </summary>
        /// <returns> String of name</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
