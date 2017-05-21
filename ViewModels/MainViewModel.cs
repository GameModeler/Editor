using System.Collections;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Editor.ViewModels.Base;
using Editor.Views;
using Map.Models;
using Map.Utilities;
using Prism.Commands;

namespace Editor.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Attributes

        private World _world;
        private World _settingsWorld;

        #endregion

        #region Properties

        public ObservableCollection<Asset> Assets { get; set; }

        public World World
        {
            get => _world;
            set
            {
                _world = value;
                OnPropertyChanged();
            }
        }

        public World SettingsWorld
        {
            get => _settingsWorld;
            set
            {
                _settingsWorld = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand NewCommand { get; }
        public DelegateCommand OpenCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand SaveAsCommand { get; }
        public DelegateCommand CloseCommand { get; }
        public DelegateCommand ExitCommand { get; }
        public DelegateCommand AddAssetsCommand { get; }
        public DelegateCommand RemoveAssetsCommand { get; }
        public DelegateCommand AddLayerCommand { get; }
        public DelegateCommand RemoveLayerCommand { get; }
        public DelegateCommand SettingsCommand { get; }
        public DelegateCommand ValidateSettingsCommand { get; }
        public DelegateCommand AboutCommand { get; }                    

        #endregion

        #region Constructors

        public MainViewModel()
        {
            Assets = new ObservableCollection<Asset>();

            NewCommand = new DelegateCommand(NewCommand_OnExecuted, () => true);
            OpenCommand = new DelegateCommand(OpenCommand_OnExecuted, () => true);
            SaveCommand = new DelegateCommand(SaveCommand_OnExecuted, SaveCommand_OnCanExecute);
            SaveAsCommand = new DelegateCommand(SaveAsCommand_OnExecuted, () => World != null);
            CloseCommand = new DelegateCommand(CloseCommand_OnExecuted, () => World != null);
            ExitCommand = new DelegateCommand(ExitCommand_OnExecuted, () => true);
            AddAssetsCommand = new DelegateCommand(AddAssetsCommand_OnExecuted, () => World != null);
            RemoveAssetsCommand = new DelegateCommand(RemoveAssetsCommand_OnExecuted, () => Assets.Any(a => a.IsSelected));
            AddLayerCommand = new DelegateCommand(AddLayerCommand_OnExecuted, () => World != null);
            RemoveLayerCommand = new DelegateCommand(RemoveLayerCommand_OnExecuted, RemoveLayerCommand_OnCanExecute);
            SettingsCommand = new DelegateCommand(SettingsCommand_OnExecuted, () => World != null);
            ValidateSettingsCommand = new DelegateCommand(ValidateSettingsCommand_OnExecuted, () => true);
            AboutCommand = new DelegateCommand(AboutCommand_OnExecuted, () => true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Logic when the new command gets invoked.
        /// </summary>
        private void NewCommand_OnExecuted()
        {
            if (World != null)
            {
                if (!World.IsSaved && AskToSave() == MessageBoxResult.Yes)
                {
                    MessageBox.Show("Need to save here");
                }

                World = null;
                AddAssetsCommand.RaiseCanExecuteChanged();
            }
            
            SettingsWorld = new World();
            new SettingsWindow(Application.Current.MainWindow).ShowDialog();
            World.Layers.Add(new Layer("Layer 1"));
            World.Layers.Add(new Layer("Layer 2"));

            SettingsCommand.RaiseCanExecuteChanged();
            AddLayerCommand.RaiseCanExecuteChanged();
            AddAssetsCommand.RaiseCanExecuteChanged();
            CloseCommand.RaiseCanExecuteChanged();
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
            return World != null && !World.IsSaved;
        }

        /// <summary>
        /// Logic when the save command gets invoked.
        /// </summary>
        public void SaveCommand_OnExecuted()
        {
            // Todo: Save the map
        }

        /// <summary>
        /// Logic when the save as command gets invoked.
        /// </summary>
        public void SaveAsCommand_OnExecuted()
        {
            // Todo: Save the map as...
        }

        /// <summary>
        /// Logic when the close command gets invoked.
        /// </summary>
        public void CloseCommand_OnExecuted()
        {
            if (!World.IsSaved && AskToSave() == MessageBoxResult.Yes)
            {
                MessageBox.Show("Save here!");
            }

            World = null;
            Assets.Clear();
            AddAssetsCommand.RaiseCanExecuteChanged();
            RemoveAssetsCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            SaveAsCommand.RaiseCanExecuteChanged();
            CloseCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Logic when the close command gets invoked.
        /// </summary>
        public void ExitCommand_OnExecuted()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Logic when the add assets command gets invoked.
        /// </summary>
        public void AddAssetsCommand_OnExecuted()
        {
            var assetFiles = WindowsUtilities.BrowseFiles("Browse Assets", "Assets", new[] { "jpg", "png" }, true);

            if (assetFiles == null) return;
            var newFiles = assetFiles.FileNames.Where(af => Assets.All(a => new Asset(af).Location != a.Location));
            foreach (var file in newFiles)
            {
                Assets.Add(new Asset(file));
            }
        }

        /// <summary>
        /// Logic when the remove assets command gets invoked.
        /// </summary>
        public void RemoveAssetsCommand_OnExecuted()
        {
            if (Confirm() == MessageBoxResult.No) return;

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
        /// Logic when the add layer command gets invoked.
        /// </summary>
        public void AddLayerCommand_OnExecuted()
        {
            // Todo: Add layer
        }

        /// <summary>
        /// Checks if the user is allowed to remove layers.
        /// </summary>
        public bool RemoveLayerCommand_OnCanExecute()
        {
            return World != null && World.Layers.Any(l => l.IsSelected);
        }

        /// <summary>
        /// Logic when the remove layers command gets invoked.
        /// </summary>
        public void RemoveLayerCommand_OnExecuted()
        {
            // Todo: Remove selected layers
        }

        /// <summary>
        /// Logic when the map settings command gets invoked.
        /// </summary>
        public void SettingsCommand_OnExecuted()
        {
            SettingsWorld = MapUtilities.Clone(World);
            new SettingsWindow(Application.Current.MainWindow).ShowDialog();
        }

        public void ValidateSettingsCommand_OnExecuted()
        {
            World = SettingsWorld;
        }

        /// <summary>
        /// Logic when the about command gets invoked.
        /// </summary>
        public void AboutCommand_OnExecuted()
        {
            WindowsUtilities.ShowMessage("About", "Copyright 2017 GameModeler Inc.", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Sets the IsSelected property of the layers accordingly to their UI selected state.
        /// </summary>
        /// <param name="selectedItem">The selected layer.</param>
        public void HandleLayerListSelection(object selectedItem)
        {
            var selectedlayer = selectedItem as Layer;

            foreach (var layer in World.Layers)
            {
                if (layer == selectedlayer)
                {
                    if (layer != null) layer.IsSelected = true;
                }
                else
                {
                    layer.IsSelected = false;
                }
            }

            RemoveLayerCommand.RaiseCanExecuteChanged();
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

        public MessageBoxResult Confirm()
        {
            return WindowsUtilities.ShowMessage(ConfigurationManager.AppSettings.Get("AppName"), "Are you sure?\nThere is no turning back!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
        }


        public MessageBoxResult AskToSave()
        {
            return WindowsUtilities.ShowMessage(ConfigurationManager.AppSettings.Get("AppName"), "Do you want to save your changes to the current map before closing it?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        }

        #endregion
    }
}
