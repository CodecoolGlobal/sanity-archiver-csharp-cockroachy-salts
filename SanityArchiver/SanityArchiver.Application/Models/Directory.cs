using System.Collections.Generic;

namespace SanityArchiver.Application.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Directory
    {
        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }

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
        public List<File> Files { get; }

        /// <summary>
        /// Getter and setter for list of directories
        /// </summary>
        public List<Directory> Directories { get; }

        /// <summary>
        /// Getter and setter for dir name
        /// </summary>
        public string Name { get; set; }

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