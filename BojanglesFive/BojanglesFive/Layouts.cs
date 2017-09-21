using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roguelike
{
    class Layouts
    {

        protected int[,] tiles;
        protected int w;
        protected int h;
        protected int left;
        protected int top;

        public Layouts()
        {
        }

        public virtual void setLayout(int xPos, int yPos, int width, int height)
        {
            w = width; h = height;
            tiles = new int[width, height];
            left = xPos; top = yPos;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tiles[i, j] = 0;
                }
            }
        }

        public int[,] getLayout()
        {
            return tiles;
        }

        public int getLeft()
        {
            return left;
        }
        public int getTop()
        {
            return top;
        }
        public int getWidth()
        {
            return w;
        }
        public int getHeight()
        {
            return h;
        }

    }
}
