using CirclePrefect.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GenMapViewer
{
    public class SquareBrush : Entity
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public bool Active = true;
        public bool Square = true;
        private bool isDoor = false;
        public Rectangle Hitbox
        {
            get { return new Rectangle(X, Y, Width, Height); }
        }
        public bool door(bool door)
        {
            return isDoor = door;
        }
        public SquareBrush(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
        public void Collision(Player player, int buffer = 4)
        {
            if (Hitbox.Contains(player.position.X, player.position.Y, Player.plrWidth, Player.plrHeight))
                player.collide = true;
            //  Directions
            if (Hitbox.Contains(player.position.X, player.position.Y - buffer, Player.plrWidth, 2))
                player.colUp = true;
            if (Hitbox.Contains(player.position.X, player.position.Y + Player.plrHeight + buffer, Player.plrWidth, 2))
                player.colDown = true;
            if (Hitbox.Contains(player.position.X + Player.plrWidth + buffer, player.position.Y, 2, Player.plrHeight))
                player.colRight = true;
            if (Hitbox.Contains(player.position.X - buffer, player.position.Y, 2, Player.plrHeight))
                player.colLeft = true;
        }
    }
}
