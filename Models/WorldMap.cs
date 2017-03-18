namespace Editor.Models
{
    public class WorldMap
    {
        #region Attributes

        #endregion

        #region Properties

        /// <summary>
        /// Map's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Map's width (nb of tiles).
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Map's height (nb of tiles).
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Tiles width.
        /// </summary>
        public int TileWidth { get; set; }

        /// <summary>
        /// Tiles height.
        /// </summary>
        public int TileHeight { get; set; }

        /// <summary>
        /// Map's saving state.
        /// </summary>
        public bool IsSaved { get; set; }

        /// <summary>
        /// Map's cells.
        /// </summary>
        public Cell[,] Cells { get; set; }

        #endregion

        #region Constructors

        public WorldMap(string name, int width, int height, int tileWidth, int tileHeight)
        {
            Name = name;
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            IsSaved = false;
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
