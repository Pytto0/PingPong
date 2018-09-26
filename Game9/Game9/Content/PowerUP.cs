using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game9
{
    class PowerUp
    {
        public short X { get; set; }
        public short Y { get; set; }
        public Texture2D Sprite { get; set; }
        public PowerUp(short x, short y, Texture2D sprite)
        {
            Sprite = sprite;
            X = x;
            Y = y;
        }
    }
}
