using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roguelike
{
    class CorridorV : Layouts
    {
        public CorridorV()
        {

        }

        public override void setLayout(int xPos, int yPos, int length, int width)
        {
            w = length; h = width;
            tiles = new int[length, width];
            left = xPos - (length / 2); top = yPos;

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == 0 || i == length - 1)
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
            w = width;

            int r1Cx = room1.getLeft() + (room1.getWidth() / 2);
            int r1Cy = room1.getTop() + (room1.getHeight() / 2);

            int r2Cx = room2.getLeft() + (room2.getWidth() / 2);
            int r2Cy = room2.getTop() + (room2.getHeight() / 2);



            if(r1Cy > r2Cy)
                h = room1.getTop() - room2.getTop() - room2.getHeight();
            else
                h = room2.getTop() - room1.getTop() - room1.getHeight();



            if (room1.getWidth() >= room2.getWidth() )
            {
                if (room1.getWidth() >= 2 * room2.getWidth())
                {
                    left = r2Cx;
                }
                else
                    left = (r1Cx + r2Cx)/ 2;
            }
            else if (room2.getWidth() > room1.getWidth())
            {
                if (room2.getWidth() >= 2 * room1.getWidth())
                {
                    left = r1Cx;
                }
                else
                    left = (r1Cx + r2Cx) / 2;
            }
            
            top = room1.getTop() + room1.getHeight() - 1;

            if (h < 0)
                h *= -1;

            h += 2;

            setLayout(left, top, w, h);
        }
    }
}
