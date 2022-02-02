using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RUDD;

namespace GenMapViewer
{
    public class Entity
    {
        public int X
        {
            get { return (int)position.X; }
            set { position.X = value; }
        }
        public int Y
        {
            get { return (int)position.Y; }
            set { position.Y = value; }
        }
        public int style;
        public Item.Style itemStyle;
        public Item.Type itemType;
        public bool active;
        public int type;
        public int maxLife;
        public int statLife;
        public int timeLeft;
        public Vector2 position;
        public Vector2 velocity;
        public int whoAmI, owner;
        public float scale = 1f;
        public float knockBack;
        public int damage;
        public Vector2 Center
        {
            get { return new Vector2(position.X + width / 2, position.Y + height / 2); }
        }
        public bool Proximity(Vector2 vec, float distance)
        {
            return vec.X < Center.X + distance && vec.X > Center.X - distance && vec.Y < Center.Y + distance && vec.Y > Center.Y - distance;
        }
        public float Distance(Vector2 start, Vector2 end)
        {
            float a = end.X - start.X;
            float b = end.Y - start.Y;
            return (float)(Math.Sqrt(Math.Pow(a, 2)) + Math.Sqrt(Math.Pow(b, 2)));
        }
        public float AngleTo(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(from.Y -to.Y, from.X - to.X);
        }
        public Vector2 AngleToSpeed(float angle, float amount)
        {
            float cos = (float)(amount * Math.Cos(angle));
            float sine = (float)(amount * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        
        public Rectangle hitbox;
        public Color color;
        public int width;
        public int height;
        public Image texture;
        public string text;
        public string Name;
    }
}
