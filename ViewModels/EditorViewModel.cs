using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Editor.Models;
using Editor.ViewModels.Base;
using Microsoft.Win32;

namespace Editor.ViewModels
{
    public class EditorViewModel : BaseViewModel
    {
        #region Attributes

        private WorldMap _worldMap;

        #endregion

        #region Properties

        public WorldMap WorldMap
        {
            get { return _worldMap; }
            set
            {
                _worldMap = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Tile> Tiles { get; set; }

        #endregion

        #region Constructors

        public EditorViewModel()
        {
            WorldMap = null;
            Tiles = new ObservableCollection<Tile>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds selected assets to the tiles list.
        /// </summary>
        /// <param name="openFileDialog">Dialog returning the assets to add.</param>
        public void AddAssets(OpenFileDialog openFileDialog)
        {
            foreach (string file in openFileDialog.FileNames)
            {
                BitmapImage original = new BitmapImage(new Uri(file));
                CroppedBitmap cropped = new CroppedBitmap(original, new Int32Rect(0, 0, WorldMap.TileWidth, WorldMap.TileHeight));

                Tile tile = new Tile
                {
                    Name = Path.GetFileName(file),
                    OriginalAsset = original,
                    CroppedAsset = cropped
                };

                Tiles.Add(tile);
            }
        }

        /// <summary>
        /// Adds selected assets to the tiles list.
        /// </summary>
        /// <param name="openFileDialog">Dialog returning the assets to add.</param>
        public void AddAssets(string[] assetsFiles)
        {
            foreach (string file in assetsFiles)
            {
                BitmapImage original = new BitmapImage(new Uri(file));
                CroppedBitmap cropped = new CroppedBitmap(original, new Int32Rect(0, 0, 0, 0));

                Tile tile = new Tile
                {
                    Name = Path.GetFileName(file),
                    OriginalAsset = original,
                    CroppedAsset = cropped
                };

                Tiles.Add(tile);
            }
        }

        /// <summary>
        /// Recrop assets already loaded after a change of the map tiles size.
        /// </summary>
        /// <param name="width">New tile width.</param>
        /// <param name="height">New tile height.</param>
        public void RecropAssets(int width, int height)
        {
            foreach (Tile tile in Tiles)
            {
                tile.CroppedAsset = new CroppedBitmap(tile.OriginalAsset, new Int32Rect(0, 0, width, height));
            }
        }

        /// <summary>
        /// Removes selected assets from the tiles list.
        /// </summary>
        /// <param name="selection">Selected assets to remove.</param>
        public void RemoveAssets(IList selection)
        {
            var selectedAssets = selection.Cast<Tile>().ToList();

            foreach (Tile asset in selectedAssets)
            {
                Tiles.Remove(asset);
            }
        }

        #endregion
    }
}
