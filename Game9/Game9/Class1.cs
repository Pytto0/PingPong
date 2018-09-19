using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game9
{
    public class Ball
    {
        public int Direction { get; set; }
        public double Speed { get; set; }
        public Vector2 Position { get; set; }

        public Ball(int direction, double speed, Vector2 position) {
            Direction = direction;
            Speed = speed;
            Position = position;

            
        }
    }
}
