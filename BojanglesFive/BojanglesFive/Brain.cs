using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Roguelike
{
    class Brain
    {

        public bool[,] closed;  //whether or not location is closed
        public float[,] cost;   //cost value for each location
        public Vector2[,] link;  //link for each location = coords
                                //of a neighbouring location
        public bool[,] inPath;  //whether or not a location
        public List<Vector2> path;
        //is in the final path


        public Brain()
        {
            closed = new bool[80, 48];
            cost = new float[80, 48];
            link = new Vector2[80, 48];
            inPath = new bool[80, 48];
            path = new List<Vector2>();
        }

        public void Build(int[,] level, EnemyBase bot, Player player)
        {
            for (int i = 0; i < 80; i++)
            {
                for (int j = 0; j < 48; j++)
                {
                    closed[i, j] = false;
                    cost[i, j] = 1000;
                    link[i, j] = new Vector2(-1, -1);
                    inPath[i, j] = false;
                }
            }

            closed[(int)bot.GridPosition.X, (int)bot.GridPosition.Y] = false;
            cost[(int)bot.GridPosition.X, (int)bot.GridPosition.Y] = 0;

            while (closed[(int)(player.GridPosition.X / 10), (int)(player.GridPosition.Y / 10)] == false)
            {

                int xPos = 0;
                int yPos = 0;

                Vector2 checkTile;
                Vector2 nextTile = new Vector2(-1, -1);
                float cheapest = 10000;
                for (int i = 0; i < 80; i++)
                {
                    for (int j = 0; j < 48; j++)
                    {
                        if (cost[i, j] < cheapest && closed[i, j] == false)
                        {
                            cheapest = cost[i, j];
                            checkTile = new Vector2(i, j);
                            if (level[(int)checkTile.X, (int)checkTile.Y] != 1)
                            {
                                nextTile = checkTile;
                                xPos = (int)nextTile.X;
                                yPos = (int)nextTile.Y;
                            }
                        }
                    }
                }

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0)
                            continue;

                        if (level[xPos + x, yPos + y] == 1)
                        {
                            closed[xPos, yPos] = true;

                            if (Math.Abs(x) == Math.Abs(y))
                            {
                                if (cost[xPos + x, yPos + y] > cost[xPos, yPos] + 1.4f)
                                {
                                    cost[xPos + x, yPos + y] = cost[xPos, yPos] + 1.4f;
                                    link[xPos + x, yPos + y] = nextTile;
                                }
                            }
                            else
                            {
                                if (cost[xPos + x, yPos + y] > cost[xPos, yPos] + 1f)
                                {
                                    cost[xPos + x, yPos + y] = cost[xPos, yPos] + 1f;
                                    link[xPos + x, yPos + y] = nextTile;
                                }
                            }
                        }
                    }
                }

            }

            //no while
            bool done = false; //set to true when we are back at the bot position
            Vector2 nextClosed = player.GridPosition; //start of path
            while (!done)
            {
                inPath[(int)nextClosed.X, (int)nextClosed.Y] = true;
                path.Add(new Vector2(nextClosed.X, nextClosed.Y));
                nextClosed = link[(int)nextClosed.X, (int)nextClosed.Y];
                if (nextClosed == bot.GridPosition) done = true;
            }

        }

        public Vector2 getNextTile()
        {
            return path[1];
        }

    }
}
