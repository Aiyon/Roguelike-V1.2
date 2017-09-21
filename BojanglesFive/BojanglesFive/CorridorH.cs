using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roguelike
{
    class CorridorH : Layouts
    {

        public CorridorH()
        {

        }

        public override void setLayout(int xPos, int yPos, int length, int width)
        {
            w = length; h = width;
            tiles = new int[length, width];
            left = xPos; top = yPos-(width/2);

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (j == 0 || j == width - 1)
                        tiles[i, j] = 1;
                    else
                        tiles[i, j] = 0;
                }
            }
        }

        public void noCorridor()
        {
            w = 1; h = 1;
            left = 0; top = 0;
            tiles = new int[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    tiles[i, j] = 0;
                }
            }
        }

        public void connectRooms(Room room1, Room room2, int width)
        {
            h = width;

            int r1Cx = room1.getLeft() + (room1.getWidth() / 2);
            int r1Cy = room1.getTop() + (room1.getHeight() / 2);

            int r2Cx = room2.getLeft() + (room2.getWidth() / 2);
            int r2Cy = room2.getTop() + (room2.getHeight() / 2);



            if(r1Cx > r2Cx)
                w = room1.getLeft() - room2.getLeft() - room2.getWidth();
            else
                w = room2.getLeft() - room1.getLeft() - room1.getWidth();



            if (room1.getHeight() >= room2.getHeight() )
            {
                if (room1.getHeight() >= 2 * room2.getHeight())
                {
                    top = r2Cy;
                }
                else
                    top = (r1Cy + r2Cy)/ 2;
            }
            else if (room2.getHeight() > room1.getHeight())
            {
                if (room2.getHeight() >= 2 * room1.getHeight())
                {
                    top = r1Cy;
                }
                else
                    top = (r1Cy + r2Cy) / 2;
            }
            
            left = room1.getLeft() + room1.getWidth() - 1;

            if (w < 0)
                w *= -1;

            w += 2;

            setLayout(left, top, w, h);
        }
    }
}
