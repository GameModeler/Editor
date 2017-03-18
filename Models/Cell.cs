using System.Collections.ObjectModel;

namespace Editor.Models
{
    public class Cell
    {
        #region Attributes
        #endregion

        #region Properties

        public ObservableCollection<Tile> Tiles { get; set; }

        #endregion

        #region Constructors

        public Cell()
        {
            Tiles = new ObservableCollection<Tile>();
        }

        #endregion

        #region Methods
        #endregion
    }
}
