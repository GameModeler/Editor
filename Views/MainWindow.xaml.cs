using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Editor.ViewModels;
using Prism.Commands;

namespace Editor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        public DelegateCommand ToggleLayersCommand { get; }
        public DelegateCommand ToggleAssetsCommand { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Main window initialization.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Title = ConfigurationManager.AppSettings.Get("AppName");

            ToggleLayersCommand = new DelegateCommand(ToggleLayersCommand_OnExecuted, () => true);
            ToggleAssetsCommand = new DelegateCommand(ToggleAssetsCommand_OnExecuted, () => true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the status bar text for the new command.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void NewCommand_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Create a new map";
        }

        /// <summary>
        /// Sets the status bar text for the open command.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenCommand_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Open an existing map";
        }

        /// <summary>
        /// Sets the status bar text for the save command.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveCommand_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Save changes to the map";
        }

        /// <summary>
        /// Sets the status bar text for the save as command.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveAsCommand_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Save the map as a new copy";
        }

        /// <summary>
        /// Sets the status bar text for the close command.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CloseCommand_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Close the current map";
        }

        /// <summary>
        /// Sets the status bar text for the exit command.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ExitCommand_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Exit the map editor";
        }

        /// <summary>
        /// Sets the status bar text for the add assets command.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void AddAssetsCommand_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Add new map assets";
        }

        /// <summary>
        /// Sets the status bar text for the remove assets command.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void RemoveAssetsCommand_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Remove selected map assets";
        }

        /// <summary>
        /// Sets the status bar text for the add layer command.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void AddLayerCommand_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Add a layer to the map";
        }

        /// <summary>
        /// Sets the status bar text for the remove layers command.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void RemoveLayersCommand_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Remove selected layers from the map";
        }

        /// <summary>
        /// Sets the status bar text for the map settings command.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void MapSettingsCommand_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Open the map settings window";
        }

        /// <summary>
        /// Sets the status bar text for the toggle layers command.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ToggleLayersCommand_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Toggle the layers panel";
        }

        /// <summary>
        /// Sets the status bar text for the toggle assets command.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ToggleAssetsCommand_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Toggle the assets panel";
        }

        /// <summary>
        /// Sets the status bar text for the about command.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void AboutCommand_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Show credits and informations";
        }

        /// <summary>
        /// Sets the default status bar text.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Command_OnMouseLeave(object sender, MouseEventArgs e)
        {
            StatusBar.Text = "Ready";
        }

        private void ToggleLayersCommand_OnExecuted()
        {
            LayerListPanel.Visibility = LayerListPanel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ToggleAssetsCommand_OnExecuted()
        {
            AssetListPanel.Visibility = AssetListPanel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Forwards the selected layer to the view model for processing.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void LayerList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mainViewModel = DataContext as MainViewModel;
            mainViewModel?.HandleLayerListSelection(e.AddedItems[0]);
        }

        /// <summary>
        /// Forwards the list of selected and deselected assets to the view model for processing.
        /// </summary>
        /// <param name="sender">The UI element triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void AssetList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mainViewModel = DataContext as MainViewModel;
            mainViewModel?.HandleAssetListSelections(e.AddedItems, e.RemovedItems);
        }

        #endregion
    }
}
