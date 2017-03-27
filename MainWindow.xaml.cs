using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Editor.Models;
using Editor.ViewModels;
using Editor.Views;
using Microsoft.Win32;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Attributes
        
        private double _zoom;
        private Point _previousPosition;
        private double _offsetX;
        private double _offsetY;

        #endregion

        #region Properties
        
        public EditorViewModel EditorViewModel { get; set; }
        public PropertiesWindow PropertiesWindow { get; set; }

        #endregion

        #region Constructors

        public MainWindow()
        {
            // Initialization
            InitializeComponent();

            EditorViewModel = new EditorViewModel();
            PropertiesWindow = new PropertiesWindow();

            DataContext = PropertiesWindow.DataContext = EditorViewModel;

            // Command Bindings
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, NewCommandExecuted));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OpenCommandExecuted));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, SaveCommandExecuted, SaveCommandCanExecute));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseCommandExecuted));
            CommandBindings.Add(new CommandBinding(NavigationCommands.Zoom, ResetCommandExecuted, ResetCommandCanExecute));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, BrowseCommandExecuted, BrowseCommandCanExecute));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, DeleteCommandExecuted, DeleteCommandCanExecute));

            // Event handlers
            Closing += MainWindow_Closing;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Command invoked to create a new map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (EditorViewModel.WorldMap != null && !EditorViewModel.WorldMap.IsSaved)
            {
                string title = "Warning";
                string message = "Do you want to save the current map before creating a new one?\nAll unsaved modifications will be lost.";

                MessageBoxResult result = ShowMessage(this, title, message, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Todo: Save the current map
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            GenerateMap();
        }

        /// <summary>
        /// Command invoked to open a previously saved map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the currently opened map can be saved during the save command invoke. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !EditorViewModel.WorldMap?.IsSaved ?? false;
        }

        /// <summary>
        /// Command invoked to save the current map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Command invoked to quit the editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Checks if the zoom and pan can be modified.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditorViewModel.WorldMap != null;
        }

        /// <summary>
        /// Command invoked to reset the zoom and pan to their default values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _zoom = 1;
            _offsetX = 0;
            _offsetY = 0;

            MoveZoom();
        }

        /// <summary>
        /// Checks if new assets can be added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditorViewModel.WorldMap != null;
        }

        /// <summary>
        /// Command invoked to browse for assets to load into the editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Browse assets",
                Filter = "Images (*.jpg; *.png)|*.jpg;*.png",
                Multiselect = true
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                EditorViewModel.AddAssets(openFileDialog);
            }
        }

        /// <summary>
        /// Checks if the assets can be deleted from the editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AssetsList.SelectedItems.Count > 0;
        }

        /// <summary>
        /// Command invoked to delete assets from the editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            string title = "Remove tiles";
            string message = "Remove the selected tiles?";

            MessageBoxResult result = ShowMessage(this, title, message, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                EditorViewModel.RemoveAssets(AssetsList.SelectedItems);
            }
        }

        /// <summary>
        /// Generates a new empty map based on inputs from the user.
        /// </summary>
        private void GenerateMap()
        {
            PropertiesWindow.Owner = this;
            PropertiesWindow.ShowDialog();

            string mapName = PropertiesWindow.MapNameValue.Text;
            int mapWidth = 0;
            int mapHeight = 0;
            int tileWidth = 0;
            int tileHeight = 0;

            try
            {
                mapWidth = int.Parse(PropertiesWindow.MapWidthValue.Text);
                mapHeight = int.Parse(PropertiesWindow.MapHeightValue.Text);
                tileWidth = int.Parse(PropertiesWindow.TileWidthValue.Text);
                tileHeight = int.Parse(PropertiesWindow.TileHeightValue.Text);
            }
            catch (FormatException e)
            {
                string title = "Wrong format";
                string message = "We were not able to create the map because one or more values were not in the correct format!";

                ShowMessage(this, title, message, MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            EditorViewModel.WorldMap = new WorldMap(mapName, mapWidth, mapHeight, tileWidth, tileHeight);

            MapCanvas.Children.Clear();

            MapCanvas.Children.Add(new Rectangle()
            {
                Width = mapWidth * tileWidth,
                Height = mapHeight * tileHeight,
                Stroke = Brushes.LightGray,
                Fill = Brushes.Azure
            });

            for (int i = 0; i < mapWidth * tileWidth; i += tileWidth)
            {
                for (int j = 0; j < mapHeight * tileHeight; j += tileHeight)
                {
                    MapCanvas.Children.Add(new Rectangle()
                    {
                        Width = tileWidth,
                        Height = tileHeight,
                        Margin = new Thickness(i, j, 0, 0),
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 1
                    });
                }
            }

            _zoom = 1;
            _offsetX = 0;
            _offsetY = 0;

            EditorViewModel.RecropAssets(tileWidth, tileHeight);
        }

        /// <summary>
        /// Sets the Collection of tiles of a cell. 
        /// </summary>
        /// <param name="point">Position of the map to modify.</param>
        /// <param name="index">Index of the asset or -1 to represent its removal.</param>
        private void Draw(Point point, int index)
        {
            int x = (int) point.X / EditorViewModel.WorldMap.TileWidth;
            int y = (int) point.Y / EditorViewModel.WorldMap.TileHeight;

            Cell cell = EditorViewModel.WorldMap.Cells[x, y];

            if (index == -1)
            {
                if (cell.Tiles.Count > 0)
                {
                    cell.Tiles.RemoveAt(cell.Tiles.Count - 1);
                }
            }
            else if (x >= 0 && x < EditorViewModel.WorldMap.Width && y >= 0 && y < EditorViewModel.WorldMap.Height)
            {
                cell.Tiles.Add(AssetsList.Items[index] as Tile);
            }

            DrawOverCanvas(x, y, index);
        }

        /// <summary>
        /// Draws the selected asset at a specific position on the map.
        /// </summary>
        /// <param name="x">Horizontal position.</param>
        /// <param name="y">Vertical position.</param>
        /// <param name="assetIndex">Index of the selected asset.</param>
        private void DrawOverCanvas(int x, int y, int assetIndex)
        {
            if (assetIndex != -1)
            {
                var tile = AssetsList.Items[assetIndex] as Tile;
                if (tile != null)
                {
                    Image asset = new Image()
                    {
                        Source = tile.CroppedAsset
                    };
                    Image imageToDraw = new Image()
                    {
                        Source = asset.Source,
                        Width = EditorViewModel.WorldMap.TileWidth,
                        Height = EditorViewModel.WorldMap.TileHeight,
                        Margin = new Thickness(x * EditorViewModel.WorldMap.TileWidth,
                            y * EditorViewModel.WorldMap.TileHeight, 0, 0)
                    };
                    MapContentCanvas.Children.Add(imageToDraw);
                }
            }
            else
            {
                MapContentCanvas.Children.Add(new Rectangle()
                {
                    Width = EditorViewModel.WorldMap.TileWidth,
                    Height = EditorViewModel.WorldMap.TileHeight,
                    Margin = new Thickness(x * EditorViewModel.WorldMap.TileWidth,
                        y * EditorViewModel.WorldMap.TileHeight, 0, 0),
                    Stroke = Brushes.LightGray,
                    Fill = Brushes.Azure,
                    StrokeThickness = 1
                });
            }
        }

        /// <summary>
        /// Shows a customizable message box and returns the chosen answer.
        /// </summary>
        /// <param name="owner">Window owning the message box.</param>
        /// <param name="message">Message to show.</param>
        /// <param name="title">Title of the message box.</param>
        /// <param name="type">Accepted answers.</param>
        /// <param name="icon">Icon of the message.</param>
        /// <returns>The user's answer to the message.</returns>
        public MessageBoxResult ShowMessage(Window owner, string title, string message, MessageBoxButton type, MessageBoxImage icon)
        {
            return MessageBox.Show(owner, message, title, type, icon);
        }

        /// <summary>
        /// Performs checks to avoid closing the editor without saving changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (EditorViewModel.WorldMap != null && !EditorViewModel.WorldMap.IsSaved)
            {
                string title = "Warning";
                string message = "Do you want to save the map before closing?\nAll unsaved modifications will be lost.";

                MessageBoxResult result = ShowMessage(this, title, message, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Todo: Save the map
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                string title = "Exit the editor";
                string message = "Are you sure?";

                MessageBoxResult result = ShowMessage(this, title, message, MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Default status bat text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UIElement_MouseLeave(object sender, MouseEventArgs e)
        {
            StatusBarTxt.Text = "Ready";
        }

        /// <summary>
        /// Status bar text when hovering on File > New.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewCommand_MouseEnter(object sender, MouseEventArgs e)
        {
            StatusBarTxt.Text = "Create a new map";
        }

        /// <summary>
        /// Status bar text when hovering on File > Open.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCommand_MouseEnter(object sender, MouseEventArgs e)
        {
            StatusBarTxt.Text = "Open a saved map";
        }

        /// <summary>
        /// Status bar text when hovering on File > Save.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveCommand_MouseEnter(object sender, MouseEventArgs e)
        {
            StatusBarTxt.Text = "Save the map";
        }

        /// <summary>
        /// Status bar text when hovering on File > Exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitCommand_MouseEnter(object sender, MouseEventArgs e)
        {
            StatusBarTxt.Text = "Quit the editor";
        }

        /// <summary>
        /// Status bar text when hovering on Map > Reset zoom and pan.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetCommand_MouseEnter(object sender, MouseEventArgs e)
        {
            StatusBarTxt.Text = "Resets the zoom and pan to their default values";
        }

        /// <summary>
        /// Status bar text when hovering on Assets > Add assets.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddAssetsCommand_MouseEnter(object sender, MouseEventArgs e)
        {
            StatusBarTxt.Text = "Browse for assets to be included in the map";
        }

        /// <summary>
        /// Status bar text when hovering on Assets > Remove selected assets.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveAssetsCommand_MouseEnter(object sender, MouseEventArgs e)
        {
            StatusBarTxt.Text = "Remove the selected assets";
        }

        /// <summary>
        /// Exetutes different behaviours on the map depending on the mouse button being pressed.
        /// Left button draws the selected image of the ListView at the current position.
        /// Right button erases the image at the current position.
        /// Middle button pans on the map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Map_MouseMove(object sender, MouseEventArgs e)
        {
            if (EditorViewModel.WorldMap != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Draw(e.GetPosition(MapCanvas), AssetsList.SelectedIndex);
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    Draw(e.GetPosition(MapCanvas), -1);
                }
                else if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    var position = e.GetPosition(this);
                    var diff = position - _previousPosition;
                    _offsetX += diff.X;
                    _offsetY += diff.Y;

                    MoveZoom();
                }

                _previousPosition = e.GetPosition(this);
            }
        }

        /// <summary>
        /// Calls the appropriate action when pressing a mouse button on the map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Map_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (EditorViewModel.WorldMap != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    Map_MouseMove(sender, e);
                }
                else if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    _previousPosition = e.GetPosition(this);
                }
            }
        }

        /// <summary>
        /// Triggered when using the scroll wheel on the map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Map_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (EditorViewModel.WorldMap != null)
            {
                if (e.Delta > 0)
                {
                    _zoom *= 1.1;
                }
                else
                {
                    _zoom /= 1.1;
                }

                MoveZoom();
            }
        }

        /// <summary>
        /// Zooms in and out on the map.
        /// </summary>
        private void MoveZoom()
        {
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(_zoom, _zoom));
            transformGroup.Children.Add(new TranslateTransform(_offsetX, _offsetY));

            MapCanvas.RenderTransform = transformGroup;
            MapContentCanvas.RenderTransform = transformGroup;
        }

        #endregion
    }
}
