using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Roguelike
{
    class Coord
    {
        public int X;
        public int Y;

        public Coord(int a, int b) { X = a; Y = b; }
        public static Coord operator +(Coord c1, Coord c2)
        {
            Coord temp = new Coord(0,0);
            temp.X = (c1.X+c2.X);
            temp.Y = (c1.Y+c2.Y);
            return temp;
        }
        public static Coord operator -(Coord c1, Coord c2)
        {
            Coord temp = new Coord(0, 0);
            temp.X = (c1.X - c2.X);
            temp.Y = (c1.Y - c2.Y);
            return temp;
        }
        public static Coord operator *(Coord c1, int c2)
        {
            Coord temp = new Coord(0, 0);
            temp.X = c1.X * c2;
            temp.Y = c1.Y * c2;
            return temp;
        }
        public static Coord operator /(Coord c1, int c2)
        {
            Coord temp = new Coord(0, 0);
            temp.X = c1.X/c2;
            temp.Y = c1.Y/c2;
            return temp;
        }
        public static bool operator ==(Coord c1, Coord c2)
        {
            return ((c1.X == c2.X) && (c1.Y == c2.Y));
        }
        public static bool operator !=(Coord c1, Coord c2)
        {
            return ((c1.X != c2.X) || (c1.Y != c2.Y));
        }
        public static implicit operator Vector2(Coord c1)
        {
            Vector2 temp;
            temp.X = c1.X;
            temp.Y = c1.Y;
            return temp;
        }
        public override int GetHashCode()
        {
            return X + Y;
        }
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;
            Coord p = (Coord)obj;
            return (X == p.X) && (Y == p.Y);
        }
    }
}
