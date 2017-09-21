using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Roguelike
{
    class Room : Layouts
    {
        public Room()
        {
        }

        public override void setLayout(int xPos, int yPos, int width, int height)
        {
            w = width; h = height;
            tiles = new int[width, height];
            left = xPos; top = yPos;

            if (yPos + height >= (Consts.mapHeight-1)) height -= 1;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if ((i == 0 || i == width - 1) && (j == 0 || j == height - 1))
                        tiles[i, j] = 2;
                    if (i == 0 || j == 0 || i == width - 1 || j == height - 1)
                        tiles[i, j] = 1;
                    else
                        tiles[i, j] = 0;
                }
            }
        }

        public Vector2 getCenter()
        {
            int rCx = left + (w / 2);
            int rCy = top + (h / 2);

            return new Vector2(rCx, rCy);
        }

        public EnemyBase addEnemy()
        {
            int xPos;
            int yPos;
            Random rand = new Random();

            while (true)
            {
                xPos = rand.Next(1, w - 1);// +left;
                yPos = rand.Next(1, h - 1);// +top;

                if (tiles[xPos, yPos] == 0)
                {
                    tiles[xPos, yPos] = 3;
                    break;
                }
            }

            EnemyBase enemy = new EnemyBase(xPos + left, yPos + top, 10, 100, 100, 11, 10);

            return enemy;

        }

        public EnemyBase addEnemy(int HP, int Atk, int AC, int CV, int SV)
        {
            int xPos;
            int yPos;

            Random rand = new Random();

            while (true)
            {
                xPos = rand.Next(2, w - 2);// +left;
                yPos = rand.Next(2, h - 2);// +top;

                if (tiles[xPos, yPos] == 0)             //0 = Red, 1 = Yellow, 2 = Green
                {
                    tiles[xPos, yPos] = CV;

                    //switch (CV)
                    //{
                    //    case 0:
                    //        tiles[xPos, yPos] = 3;
                    //        break;

                    //    case 1:
                    //        tiles[xPos, yPos] = 4;
                    //        break;

                    //    case 2:
                    //        tiles[xPos, yPos] = 5;
                    //        break;

                    //}
                    

                    break;
                }
            }

            EnemyBase enemy = new EnemyBase(xPos + left, yPos + top, HP, Atk, AC, CV, SV);

            return enemy;

        }
    }
}
