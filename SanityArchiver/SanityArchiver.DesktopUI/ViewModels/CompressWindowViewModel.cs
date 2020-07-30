using System.Collections.Generic;
using System.IO.Compression;
using SanityArchiver.ViewModels;
using static System.IO.Directory;

namespace SanityArchiver.DesktopUI.ViewModels
{
    public class CompressWindowViewModel : BaseViewModel
    {
        private string _fileName;

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                SaveBtnEnabled = value.Length >= 3;
            }
        }

        public bool SaveBtnEnabled { get; set; }

        public List<DirectoryItemViewModel> Files;

        public void CompressTheFiles()
        {
            using (var zip = ZipFile.Open(FileName + ".zip", ZipArchiveMode.Create))
            {
                foreach (var file in Files)
                {
                    zip.CreateEntryFromFile(file.FullPath, file.Name);
                }
            }

            var sourceLocation = $"{GetCurrentDirectory()}\\{FileName}.zip";
            var targetLocation = Files[0].FullPath.Substring(0, Files[0].FullPath.Length - Files[0].Name.Length) +
                                 "\\" + FileName + ".zip";
            System.IO.File.Move(sourceLocation, targetLocation);
        }
    }
}