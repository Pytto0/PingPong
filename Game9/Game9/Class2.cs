using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game9
{
    public class Player
    {
        public short Speed { get; set; }
        public short X { get; set; }
        public short Y { get; set; }

        public Player(short speed, short x, short y) {
            Speed = speed;
            X = x;
            Y = y;
        }
    }
}
