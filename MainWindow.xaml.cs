using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Security.Policy;
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
        
        // Dame
        private Boolean no_edit = false;
        private int[,] game;
        private int[] previous_position;
        private int round;
        private Boolean second_moove;
        private int[] next_piece;
        private int down_limit;
        private int up_limit;
        private Boolean dame;
        private Int32 mapWidth;
        private Int32 mapHeight;
        private Int32 tileWidth;
        private Int32 tileHeight;
        private string[] cases_pieces;
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
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PrintCommandExecuted));

            // Event handlers
            Closing += MainWindow_Closing;
        }

        #endregion

        #region Methods
        /************************************ TEST Jeu de Dame *****************************************************/
        private void PrintCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.draw_game_map(null);

            // Set map image
            string directory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            this.cases_pieces = new string[] { directory + "/media/image/case/white.png",
                                               directory + "/media/image/case/brown.jpg",
                                               directory + "/media/image/piece/white.png",
                                               directory + "/media/image/piece/black.png",
                                               directory + "/media/image/piece/white_dame.png",
                                               directory + "/media/image/piece/black_dame.png" };
            EditorViewModel.AddAssets(null, cases_pieces);

            int index;
            Point point = new Point();
            for(int i=0; i< this.mapWidth; i++)
            {
                for(int j=0; j< this.mapHeight; j++)
                {
                    // Draw map
                    index = (i + j) % 2;
                    int[] position = { i, j };
                    Draw(point, index, position);
                    if(index == 1)
                        this.game[i, j] = 0;

                    // Draw piece on map
                    int[] piece_position = { i, j };
                    if ((i + j) % 2 == 1 && (j < 4 | j > 5))
                    {
                        int piece_index = (j < 4) ? 2 : 3;
                        this.game[i, j] = piece_index;
                        Draw(point, piece_index, piece_position);
                    }
                }
            }
            this.set_game_attr(null);
        }

        private void restore_game(Game game)
        {
            this.draw_game_map(game);

            // Set map image
            this.cases_pieces = game.Images;
            EditorViewModel.AddAssets(null, this.cases_pieces);

            // Draw map
            int index;
            Point point = new Point();
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    index = (i + j) % 2;
                    int[] position = { i, j };
                    Draw(point, index, position);

                    // Draw piece on map
                    int[] piece_position = { i, j };
                    if (game.Game_map[i, j] != 0)
                        Draw(point, game.Game_map[i, j], piece_position);

                }
            }

            this.set_game_attr(game);
        }

        private void draw_game_map(Game game)
        {
            String mapName = "Dame";
            this.mapWidth = 10;
            this.mapHeight = 10;
            if(game == null)
            {
                this.game = new int[mapWidth, mapHeight];
            }
            else
            {
                this.game = game.Game_map;
            }

            this.tileWidth = 150;
            this.tileHeight = 150;
            EditorViewModel.WorldMap = new WorldMap(mapName, this.mapWidth, this.mapHeight, this.tileWidth, this.tileHeight);
            MapCanvas.Children.Clear();

            MapCanvas.Children.Add(new Rectangle()
            {
                Width = this.mapWidth * this.tileWidth,
                Height = this.mapHeight * this.tileHeight,
                Stroke = Brushes.LightGray,
                Fill = Brushes.Azure
            });

            for (int i = 0; i < this.mapWidth * this.tileWidth; i += this.tileWidth)
            {
                for (int j = 0; j < this.mapHeight * this.tileHeight; j += this.tileHeight)
                {
                    MapCanvas.Children.Add(new Rectangle()
                    {
                        Width = this.tileWidth,
                        Height = this.tileHeight,
                        Margin = new Thickness(i, j, 0, 0),
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 1
                    });
                }
            }
            _zoom = 1;
            _offsetX = 0;
            _offsetY = 0;
            EditorViewModel.RecropAssets(this.tileWidth, this.tileHeight);

            _zoom /= 2.5;
            MoveZoom();
        }

        private void set_game_attr(Game game)
        {
            // Hide and resize asset element
            AssetsList.Visibility = Visibility.Hidden;
            AssetsList.Height = 0;
            AssetsList.Width = 0;
            // stop action on left button
            this.no_edit = true;
            this.round = (game != null) ? game.Round : 0;
            this.second_moove = (game != null) ? game.Second_moove : false;
            this.previous_position = null;
            this.next_piece = null;
            this.down_limit = (game != null) ? game.Down_limit : 0;
            this.up_limit = (game != null) ? game.Up_limit : this.mapHeight - 1; ;
        }

        private void map_mouseDown_game(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(MapCanvas);
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if(this.second_moove == false)
                {
                    return;
                }
                int delta = 2;
                if (this.dame)
                    delta = 4;
                round_done(point, (this.round % 2) + delta, this.previous_position, false);
                this.second_moove = false;
            }
            Boolean over = false;             
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                int x = (int) point.X / EditorViewModel.WorldMap.TileWidth;
                int y = (int) point.Y / EditorViewModel.WorldMap.TileHeight;
                // position of case
                int[] position = { x, y };
                // black case
                int index = (x + y) % 2; // = 1   
                int piece_index = this.game[x, y]; // if white -> 2, black -> 3, empty => 0
                Cell cell = EditorViewModel.WorldMap.Cells[x, y];
                if (index == 0)
                {
                    return;
                }
                if (this.previous_position != null && this.previous_position[0] == position[0] && this.previous_position[1] == position[1])
                {
                    return;
                }   
                
                if ((((this.round % 2 == 0 && (piece_index == 2 || piece_index == 4)) || (this.round % 2 == 1 && (piece_index == 3 || piece_index == 5))) || second_moove))
                {
                    // round start 1/2
                    if (cell.Tiles.Count > 1 && !second_moove && (this.next_piece == null || (position[0] == this.next_piece[0] && position[1] == this.next_piece[1])))
                    {
                        this.dame = false;
                        if (piece_index == 4 || piece_index == 5)
                            this.dame = true;
                        clean_cell(position[0], position[1]);
                        Draw(e.GetPosition(MapCanvas), 1);
                        this.second_moove = true;
                        this.previous_position = position;
                        this.game[x, y] = 0;  
                    } 
                    else if(cell.Tiles.Count == 1) // round 2/2
                    {
                        
                        int init_x = this.previous_position[0];
                        int init_y = this.previous_position[1];
                                              
                        Boolean black_moove = this.round % 2 == 1 && ((init_x - 1 == position[0] || init_x + 1 == position[0]) && init_y - 1 == position[1]);
                        Boolean white_moove = this.round % 2 == 0 && ((position[0] == init_x + 1 || position[0] == init_x - 1) && position[1] == init_y + 1);
                        if (white_moove || black_moove)
                        {
                            int delta = 2;
                            if (this.dame)
                                delta = 4;
                            round_done(point, (this.round % 2) + delta, position);      
                        }                       
                        else
                        {
                            int real_x;
                            if (!this.dame)
                            {

                                if (round % 2 == 0) // white eat black
                                {
                                    int piece = 2;
                                    int[] pieces_check = { 3, 5 };
                                    if (((init_x + 2 == position[0] || init_x - 2 == position[0]) && init_y + 2 == position[1])
                                        && (this.check_map(init_x + 1, init_y + 1, pieces_check) || check_map(init_x - 1, init_y + 1, pieces_check)))
                                    {
                                        real_x = (position[0] == init_x + 2 && position[1] == init_y + 2) ? init_x + 1 : init_x - 1;
                                        round_behaviour(point, real_x, init_y + 1, x, y, position, piece_index, piece);
                                    }
                                    else if (((init_x + 2 == position[0] || init_x - 2 == position[0]) && init_y - 2 == position[1])
                                        && (this.check_map(init_x + 1, init_y - 1, pieces_check) || check_map(init_x - 1, init_y - 1, pieces_check)))
                                    {
                                        real_x = (position[0] == init_x + 2 && position[1] == init_y - 2) ? init_x + 1 : init_x - 1;
                                        round_behaviour(point, real_x, init_y - 1, x, y, position, piece_index, piece);
                                    }
                                } // black eat white
                                else if (round % 2 == 1)
                                {
                                    int piece = 3;
                                    int[] pieces_check = { 2, 4 };
                                    if (((init_x - 2 == position[0] || init_x + 2 == position[0]) && init_y - 2 == position[1])
                                        && (check_map(init_x - 1, init_y - 1, pieces_check) || check_map(init_x + 1, init_y - 1, pieces_check)))
                                    {
                                        real_x = (position[0] == init_x - 2 && position[1] == init_y - 2) ? init_x - 1 : init_x + 1;
                                        round_behaviour(point, real_x, init_y - 1, x, y, position, piece_index, piece);
                                    }
                                    else if (((init_x + 2 == position[0] || init_x - 2 == position[0]) && init_y + 2 == position[1])
                                        && (check_map(init_x - 1, init_y + 1, pieces_check) || check_map(init_x + 1, init_y + 1, pieces_check)))
                                    {
                                        real_x = (position[0] == init_x - 2 && position[1] == init_y + 2) ? init_x - 1 : init_x + 1;
                                        round_behaviour(point, real_x, init_y + 1, x, y, position, piece_index, piece);
                                    }
                                }
                            }
                            else
                            {
                                int nb_way_piece = 0;
                                int n_x = position[0] - init_x;
                                int n_y = position[1] - init_y;
                                if(n_x == n_y || n_x == -n_y)
                                {
                                    int piece = 4;
                                    int[] pieces_check = { 3, 5 };
                                    if (this.round % 2 == 1)
                                    {
                                        pieces_check[0] = 2;
                                        pieces_check[1] = 4;
                                        piece = 5;
                                    }
                                    int eat_on_way_x = -1;
                                    int eat_on_way_y = -1;


                                    if(n_x > 0)
                                    {
                                        for (int i = 1; i <= n_x; i++)
                                        {

                                            if ((n_x == n_y && check_map(init_x + i, init_y + i, pieces_check)) || (n_x == -n_y && check_map(init_x + i, init_y - i, pieces_check)))
                                            {
                                                nb_way_piece += 1;
                                                eat_on_way_x = init_x + i;
                                                eat_on_way_y = init_y - i;
                                                if (n_x == n_y)
                                                {
                                                    eat_on_way_x = init_x + i;
                                                    eat_on_way_y = init_y + i;
                                                }                                                  
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 1; i <= -n_x; i++)
                                        {

                                            if ((n_x == n_y && check_map(init_x - i, init_y - i, pieces_check)) || (n_x == -n_y && check_map(init_x - i, init_y + i, pieces_check)))
                                            {
                                                nb_way_piece += 1;
                                                eat_on_way_x = init_x - i;
                                                eat_on_way_y = init_y + i;
                                                if (n_x == n_y)
                                                {
                                                    eat_on_way_x = init_x - i;
                                                    eat_on_way_y = init_y - i;
                                                }
                                            }
                                        }
                                    }
                              

                                    if (nb_way_piece == 1 || nb_way_piece == 0)
                                    {
                                        if(nb_way_piece == 1)
                                        {
                                            int delta_x = position[0] - eat_on_way_x;
                                            int delta_y = position[1] - eat_on_way_y;
                                            if ((delta_x < -1 || delta_x > 1) && (delta_y < -1 || delta_y > 1))
                                                return;
                                        }
                                        round_behaviour(point, eat_on_way_x, eat_on_way_y, x, y, position, piece_index, piece, nb_way_piece == 1);
                                    }
                                }                               
                            }                                        
                        }                      
                    }
                }
            }
           
        }

        private void round_behaviour(Point point, int real_x, int real_y, int x, int y, int[] position, int piece_index, int piece, Boolean eat = true)
        {
            if(eat)
                eating_piece(point, real_x, real_y);
            Boolean round_continue = check_possibilities(piece_index, x, y);
            if (!eat && this.dame)
                round_continue = false;
            round_done(point, piece, position, !round_continue);
            if (round_continue)
                this.next_piece = position;
        }

        private Boolean check_map(int x, int y, int[] pieces_index)
        {
            foreach(int piece_index in pieces_index)
            {
                if (x < this.down_limit || y < this.down_limit || x > this.up_limit || y > this.up_limit || this.game[x, y] == piece_index)
                {
                    return true;
                }
            }                
            return false;
        }

        private void eating_piece(Point point, int x, int y)
        {
            this.game[x, y] = 0;
            int index = (x + y) % 2;
            int[] adv_position = {x, y};
            clean_cell(x, y);
            Draw(point, index, adv_position);

        }

        public void clean_cell(int x, int y, int number_clean = 2)
        {
            Cell cell = EditorViewModel.WorldMap.Cells[x, y];
            for(int i=0; i<number_clean; i++)
            {
                cell.Tiles.RemoveAt(cell.Tiles.Count - 1);
            }
        }


        public void round_done(Point point, int index, int[] position, Boolean next_round = true)
        {
            Draw(point, index, position);
            this.game[position[0], position[1]] = index;
            this.second_moove = false;
            this.previous_position = null;

            if ((position[1] == this.down_limit && this.round % 2 == 1) || (position[1] == this.up_limit && this.round % 2 == 0))
                convert_piece(point, position[0], position[1]);

            if (next_round)
            {
                this.round += 1;
                this.next_piece = null;
                this.dame = false;
                piece_count();
            }
            
        }

        private void piece_count()
        {
            int white_piece = 0;
            int black_piece = 0;
            for(int i=this.down_limit; i<=this.up_limit; i++)
            {
                for (int j = this.down_limit; j <= this.up_limit; j++)
                {
                    if (this.game[i, j] == 2 || this.game[i, j] == 4)
                        white_piece += 1;
                    if (this.game[i, j] == 3 || this.game[i, j] == 5)
                        black_piece += 1;
                }
            }
            if(white_piece == 0 || black_piece == 0)
            {
                game_over((white_piece == 0)?2:3);
            }
        }

        private void game_over(int index_win)
        {
            String winner = "white";
            if (index_win == 2)
                winner = "black";
            MessageBoxResult result = ShowMessage(this, "Win", "Win color => " + winner, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private bool check_possibilities(int index, int x, int y)
        {
            int piece_dame_index = 4;
            int piece_index = 2;
            if (this.round % 2 == 0)
            {
                piece_index = 3;
                piece_dame_index = 5;
            }
            Boolean possiblities = possibility(x + 1, y + 1, x + 2, y + 2, piece_index, piece_dame_index) || 
                                   possibility(x + 1, y - 1, x + 2, y - 2, piece_index, piece_dame_index) ||
                                   possibility(x - 1, y + 1, x - 2, y + 2, piece_index, piece_dame_index) ||
                                   possibility(x - 1, y - 1, x - 2, y - 2, piece_index, piece_dame_index);
            return possiblities;
        }

        private bool possibility(int x, int y, int next_x, int next_y, int piece_index, int piece_dame_index)
        {
            if (x >= this.down_limit && y >= this.down_limit && x <= this.up_limit && y <= this.up_limit && (this.game[x, y] == piece_index || this.game[x, y] == piece_dame_index))
            {
                if (next_x >= this.down_limit && next_y >= this.down_limit && next_x <= this.up_limit && next_y <= this.up_limit && this.game[next_x, next_y] == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void convert_piece(Point point, int x, int y)
        {
            int piece_index = 5;
            if (this.round % 2 == 0)
                piece_index = 4;
            int[] position = { x, y };

            clean_cell(x, y, 1);
            Draw(point, piece_index, position);
            this.game[x, y] = piece_index;
        }

        private void save_game()
        {
            if (this.no_edit == false)
                return;
            string directory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            if (this.cases_pieces.Length == 0)
            {
                this.cases_pieces = new string[] {  directory + "/media/image/case/white.png",
                                                    directory + "/media/image/case/brown.jpg",
                                                    directory + "/media/image/piece/white.png",
                                                    directory + "/media/image/piece/black.png",
                                                    directory + "/media/image/piece/white_dame.png",
                                                    directory + "/media/image/piece/black_dame.png" };
            }
            Game game = new Game(this.game, this.round, this.down_limit, this.up_limit, this.second_moove, cases_pieces);
            DataBase.Binary.BinaryManager.WriteToBinaryFile<Game>(directory + "/media/save/", "game.bin", game);
        }

        /************************************************************************************************/

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
                    SaveMap(EditorViewModel.WorldMap);
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
            if (this.no_edit == false)
                return;
            string directory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            Game game = DataBase.Binary.BinaryManager.ReadFromBinaryFile<Game>(directory + "/media/save/", "game.bin");
            this.restore_game(game);
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
            this.save_game();
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


        public void SaveMap(WorldMap worldMap)
        {
            this.save_game();
        }

        /// <summary>
        /// Sets the Collection of tiles of a cell. 
        /// </summary>
        /// <param name="point">Position of the map to modify.</param>
        /// <param name="index">Index of the asset or -1 to represent its removal.</param>
        private void Draw(Point point, int index, int[] position = null)
        {
            int x;
            int y;
            if (position == null)
            {
                x = (int)point.X / EditorViewModel.WorldMap.TileWidth;
                y = (int)point.Y / EditorViewModel.WorldMap.TileHeight;
            }
            else
            {
                x = position[0];
                y = position[1];
            }

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
                    SaveMap(EditorViewModel.WorldMap);
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
            if (no_edit)
            {
                map_mouseDown_game(sender, e);
                return;
            }
            if (EditorViewModel.WorldMap != null)
            {
                if (!this.no_edit && e.LeftButton == MouseButtonState.Pressed)
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
                if (this.no_edit)
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
