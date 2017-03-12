using System.Collections.ObjectModel;
using Editor.Models;

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

        

        #endregion
    }
}
