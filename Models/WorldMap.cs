using Editor.Models.Base;

namespace Editor.Models
{
    public class WorldMap : BaseModel
    {
        #region Attributes

        private string _name;
        private int _width;
        private int _height;
        private bool _isSaved;

        #endregion

        #region Properties

        /// <summary>
        /// Map's name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Map's width (nb of tiles).
        /// </summary>
        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Map's height (nb of tiles).
        /// </summary>
        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Map's saving state.
        /// </summary>
        public bool IsSaved
        {
            get { return _isSaved; }
            set
            {
                _isSaved = value;
                OnPropertyChanged();
            }
        }

        public Cell[,] Cells { get; set; }

        #endregion

        #region Constructors

        public WorldMap(string name, int width, int height)
        {
            Name = name;
            Width = width;
            Height = height;
            Cells = new Cell [Width, Height];

            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    Cells[i, j] = new Cell();
                }
            }
        }

        #endregion

        #region Methods
        #endregion
    }
}
