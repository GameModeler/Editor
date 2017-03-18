using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Editor.Models;
using Microsoft.Win32;

namespace Editor.ViewModels
{
    public class EditorViewModel
    {
        #region Attributes
        #endregion

        #region Properties

        public WorldMap WorldMap { get; set; }
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
        /// <param name="openFileDialog"></param>
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

        #endregion
    }
}
