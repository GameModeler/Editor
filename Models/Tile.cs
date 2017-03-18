using System.Windows.Media.Imaging;
using Editor.Utilities;

namespace Editor.Models
{
    public class Tile
    {
        #region Attributes
        #endregion

        #region Properties

        /// <summary>
        /// Tile's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tile's original asset.
        /// </summary>
        public BitmapImage OriginalAsset { get; set; }

        /// <summary>
        /// Tile's cropped asset.
        /// </summary>
        public CroppedBitmap CroppedAsset { get; set; }

        #endregion

        #region Constructors

        public Tile()
        {
            OriginalAsset = new BitmapImage();
            CroppedAsset = new CroppedBitmap();
        }

        public Tile(string name) : this()
        {
            Name = name;
        }

        public Tile(string name, int width, int height, BitmapImage sourceAsset)
        {
            Name = name;
            OriginalAsset = sourceAsset;
            CroppedAsset = AssetUtils.GetCroppedBitmap(width, height, OriginalAsset);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the name of the asset.
        /// </summary>
        /// <returns>The asset's name.</returns>
        public override string ToString()
        {
            return string.Format($"Asset: {Name}");
        }

        #endregion
    }
}
