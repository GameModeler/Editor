using Editor.ViewModels.Base;

namespace Editor.ViewModels
{
    public class PropertiesViewModel : BaseViewModel
    {
        #region Attributes

        private string _mapName;
        private int _mapWidth;
        private int _mapHeight;
        private int _tileWidth;
        private int _tileHeight;

        #endregion

        #region Properties

        public string MapName
        {
            get { return _mapName; }
            set
            {
                _mapName = value;
                OnPropertyChanged();
            }
        }

        public int MapWidth
        {
            get { return _mapWidth; }
            set
            {
                _mapWidth = value;
                OnPropertyChanged();
            }
        }

        public int MapHeight
        {
            get { return _mapHeight; }
            set
            {
                _mapHeight = value;
                OnPropertyChanged();
            }
        }

        public int TileWidth
        {
            get { return _tileWidth; }
            set
            {
                _tileWidth = value;
                OnPropertyChanged();
            }
        }

        public int TileHeight
        {
            get { return _tileHeight; }
            set
            {
                _tileHeight = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public PropertiesViewModel()
        {
            
        }

        #endregion
    }
}
