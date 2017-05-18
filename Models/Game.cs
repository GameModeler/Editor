using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Models
{
    [Serializable()]
    public class Game
    {
        private int[,] game_map;
        private int round;
        private Boolean second_moove;
        private int down_limit;
        private int up_limit;
        private string[] images;

        public int[,] Game_map { get; set; }
        public int Round { get; set; }
        public Boolean Second_moove { get; set; }
        public int Up_limit { get; set; }
        public int Down_limit { get; set; }
        public string[] Images { get; set; }

        public Game(int[,] game_map, int round, int down_limit, int up_limit, Boolean second_moove, string[] images)
        {
            Game_map = game_map;
            Round = round;
            Down_limit = down_limit;
            Up_limit = up_limit;
            Second_moove = second_moove;
            Images = images;
        }

    }
}
