using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SanityArchiver.ViewModels;

namespace SanityArchiver.DesktopUI.ViewModels
{
    public class DirectoryItemViewModel : BaseViewModel
    {
        #region Public Properties

        public DirectoryItemType Type { get; set; }

        public string FullPath { get; set; }

        public string Name =>
            Type == DirectoryItemType.Drive
                ? FullPath
                : DirectoryStructure.GetFileFolderName(FullPath);

        public ObservableCollection<DirectoryItemViewModel> Children { get; set; }

        public bool CanExpand => Type != DirectoryItemType.File;

        public bool IsExpanded
        {
            get
            {
                return Children?.Count(f => f != null) > 0;
            }
            set
            {
                if (value)
                {
                    Expand();
                }
                else
                {
                    ClearChildren();
                }
            }
        }

        public string Size { get; set; }

        public bool IsHidden { get; set; }

        public DateTime Created { get; set; }

        public bool IsChecked { get; set; }

        public string Extension { get; set; }

        #endregion

        #region Public Commands

        public ICommand ExpandCommand { get; set; }

        #endregion

        #region Constructor

        public DirectoryItemViewModel(string fullPath, DirectoryItemType type)
        {
            ExpandCommand = new RelayCommand(Expand);
            FullPath = fullPath;
            Type = type;
            ClearChildren();
        }

        public DirectoryItemViewModel()
        {
            ClearChildren();
        }

        #endregion

        #region Helper Methods

        private void ClearChildren()
        {
            Children = new ObservableCollection<DirectoryItemViewModel>();

            if (Type != DirectoryItemType.File)
            {
                Children.Add(null);
            }
        }

        #endregion

        public void Expand()
        {
            DirectoryStructure.GetDirectoryContents(FullPath);
            if (Type == DirectoryItemType.File)
            {
                return;
            }
            var children = DirectoryStructure.GetDirectoryContents(FullPath);
            Children = new ObservableCollection<DirectoryItemViewModel>(
                children.Select(content => new DirectoryItemViewModel(content.FullPath, content.Type)));
        }
    }
}