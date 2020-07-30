using System.Collections.ObjectModel;
using System.Linq;
using SanityArchiver.ViewModels;

namespace SanityArchiver.DesktopUI.ViewModels
{
    public class DirectoryStructureViewModel : BaseViewModel
    {
        #region Public Properties

        public ObservableCollection<DirectoryItemViewModel> Items { get; set; }

        #endregion

        #region Constructor

        public DirectoryStructureViewModel()
        {
            var children = DirectoryStructure.GetLogicalDrives();
            Items = new ObservableCollection<DirectoryItemViewModel>(children.Select(drive =>
                new DirectoryItemViewModel(drive.FullPath, DirectoryItemType.Drive)));
        }

        #endregion
    }
}