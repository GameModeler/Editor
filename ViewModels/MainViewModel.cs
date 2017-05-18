using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Editor.ViewModels.Base;
using Map.Models;
using Map.Utilities;
using Prism.Commands;

namespace Editor.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Attributes

        private World _world;

        #endregion

        #region Properties

        public ObservableCollection<Asset> Assets { get; set; }

        public World World
        {
            get => _world;
            set
            {
                if (value == null) return;
                _world = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand NewCommand { get; }
        public DelegateCommand OpenCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand SaveAsCommand { get; }
        public DelegateCommand ExitCommand { get; }
        public DelegateCommand AddAssetsCommand { get; }
        public DelegateCommand RemoveAssetsCommand { get; }
        public DelegateCommand SettingsCommand { get; }
        public DelegateCommand AboutCommand { get; }                    

        #endregion

        #region Constructors

        public MainViewModel()
        {
            Assets = new ObservableCollection<Asset>();

            NewCommand = new DelegateCommand(NewCommand_OnExecuted, NewCommand_OnCanExecute);
            OpenCommand = new DelegateCommand(OpenCommand_OnExecuted, OpenCommand_OnCanExecute);
            SaveCommand = new DelegateCommand(SaveCommand_OnExecuted, SaveCommand_OnCanExecute);
            SaveAsCommand = new DelegateCommand(SaveAsCommand_OnExecuted, SaveAsCommand_OnCanExecute);
            ExitCommand = new DelegateCommand(ExitCommand_OnExecuted, ExitCommand_OnCanExecute);
            AddAssetsCommand = new DelegateCommand(AddAssetsCommand_OnExecuted, AddAssetsCommand_OnCanExecute);
            RemoveAssetsCommand = new DelegateCommand(RemoveAssetsCommand_OnExecuted, RemoveAssetsCommand_OnCanExecute);
            SettingsCommand = new DelegateCommand(SettingsCommand_OnExecuted, SettingsCommand_OnCanExecute);
            AboutCommand = new DelegateCommand(AboutCommand_OnExecuted, AboutCommand_OnCanExecute);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if the user is allowed to create a new map.
        /// </summary>
        private bool NewCommand_OnCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Logic when the new command gets invoked.
        /// </summary>
        private void NewCommand_OnExecuted()
        {
            World = new World();

            AddAssetsCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Checks if the user is allowed to open a map.
        /// </summary>
        public bool OpenCommand_OnCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Logic when the open command gets invoked.
        /// </summary>
        public void OpenCommand_OnExecuted()
        {
            // Todo: Open map
        }

        /// <summary>
        /// Checks if the user is allowed to save the current map.
        /// </summary>
        public bool SaveCommand_OnCanExecute()
        {
            return false;
        }

        /// <summary>
        /// Logic when the save command gets invoked.
        /// </summary>
        public void SaveCommand_OnExecuted()
        {
            // Todo: Save the map
        }

        /// <summary>
        /// Checks if the user is allowed to save the current map as...
        /// </summary>
        public bool SaveAsCommand_OnCanExecute()
        {
            return false;
        }

        /// <summary>
        /// Logic when the save as command gets invoked.
        /// </summary>
        public void SaveAsCommand_OnExecuted()
        {
            // Todo: Save the map as...
        }

        /// <summary>
        /// Checks if the user is allowed to exit the application.
        /// </summary>
        public bool ExitCommand_OnCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Logic when the close command gets invoked.
        /// </summary>
        public void ExitCommand_OnExecuted()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Checks if the user is allowed to add assets.
        /// </summary>
        public bool AddAssetsCommand_OnCanExecute()
        {
            return World != null;
        }

        /// <summary>
        /// Logic when the add assets command gets invoked.
        /// </summary>
        public void AddAssetsCommand_OnExecuted()
        {
            var assetFiles = WindowsInteractions.BrowseFiles("Browse Assets", "Assets", new[] { "jpg", "png" }, true);

            if (assetFiles != null)
            {
                var newFiles = assetFiles.FileNames.Where(af => Assets.All(a => new Asset(af).Location != a.Location));
                foreach (var file in newFiles)
                {
                    Assets.Add(new Asset(file));
                }
            }
        }

        /// <summary>
        /// Checks if the user is allowed to remove assets.
        /// </summary>
        public bool RemoveAssetsCommand_OnCanExecute()
        {
            return Assets.Any(a => a.IsSelected);
        }

        /// <summary>
        /// Logic when the remove assets command gets invoked.
        /// </summary>
        public void RemoveAssetsCommand_OnExecuted()
        {
            foreach (var asset in Assets.ToList())
            {
                if (asset.IsSelected)
                {
                    Assets.Remove(asset);
                }
            }

            RemoveAssetsCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Checks if the user is allowed to change the current map's settings.
        /// </summary>
        public bool SettingsCommand_OnCanExecute()
        {
            return false;
        }

        /// <summary>
        /// Logic when the map settings command gets invoked.
        /// </summary>
        public void SettingsCommand_OnExecuted()
        {
            // Todo: Change settings
        }

        /// <summary>
        /// Checks if the user is allowed to see the credits.
        /// </summary>
        public bool AboutCommand_OnCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Logic when the about command gets invoked.
        /// </summary>
        public void AboutCommand_OnExecuted()
        {
            WindowsInteractions.ShowMessage("About", "Copyright 2017 GameModeler Inc.", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Sets the IsSelected property of the assets accordingly to their UI selection state.
        /// </summary>
        /// <param name="selectedAssets">The list of selected assets.</param>
        /// <param name="deselectedAssets">The list of deselected assets.</param>
        public void HandleAssetListSelections(IList selectedAssets, IList deselectedAssets)
        {
            foreach (var item in selectedAssets)
            {
                var asset = item as Asset;
                if (asset != null)
                {
                    asset.IsSelected = true;
                }
            }

            foreach (var item in deselectedAssets)
            {
                var asset = item as Asset;
                if (asset != null)
                {
                    asset.IsSelected = false;
                }
            }

            RemoveAssetsCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }
}
