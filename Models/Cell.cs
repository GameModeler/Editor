using System.Collections.ObjectModel;
using Editor.Models.Base;

namespace Editor.Models
{
    public class Cell : BaseModel
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
