using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RUDD;

namespace GenMapViewer
{
    public class Entity : Main
    {
        public int style;
        public bool active;
        public int type;
        public int maxLife;
        public int statLife;
        public int timeLeft;
        public Vector2 position;
        public Vector2 velocity;
        public int whoAmI;
        public float scale = 1f;
        public Vector2 Center
        {
            get { return new Vector2(position.X + width / 2, position.Y + height / 2); }
        }
        public Rectangle hitbox;
        public Color color;
        public int width;
        public int height;
        public Image texture;
        public new string Name;
    }
}
