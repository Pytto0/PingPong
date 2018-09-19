using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game9
{
    public class Bal
    {
        public int Direction { get; set; }
        public int Speed { get; set; }
        public Vector2 Position { get; set; }

        public Bal(int direction, int speed, Vector2 position) {
            Direction = direction;
            Speed = speed;
            Position = position;

            
        }
    }
}
