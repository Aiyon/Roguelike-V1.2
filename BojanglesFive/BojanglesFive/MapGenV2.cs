using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roguelike
{
    class MapGenV2
    {


        int w;
        int h;
        Random rand = new Random();
        Room spawnRoom;
        
        private EnemyBase[] enemyList = new EnemyBase[30];
        List<Room> roomList = new List<Room>();

        Vector2 exit;

        int[,] maps = new int[Consts.mapWidth,Consts.mapHeight];

        public MapGenV2()
        {
        }

        public int[,] map()
        {

            roomList = new List<Room>();

            for (int i = 0; i < Consts.mapWidth; i++)
            {
                for (int j = 0; j < Consts.mapHeight; j++)
                    maps[i, j] = -1;
            }

            spawnRoom = new Room();
            spawnRoom.setLayout((Consts.mapWidth - Consts.spawnWidth) / 2, (Consts.mapHeight - Consts.spawnHeight) / 2, Consts.spawnWidth, Consts.spawnHeight);     //xPos, yPos, width, height.
            roomList.Add(spawnRoom);

            getSubTiles(spawnRoom, maps);

            //eCounter = 0;
            //maps = new int[128, 128];
            bool done = false;
            int numRooms = 1;

            while (!done)
            {
                int temp = rand.Next(0, roomList.Count);
                int tTop = 0;
                int tLeft = 0;
                int nesw;   //0 = N, 1 = E, 2 = S, 3 = W.

                rSeed();

                Vector2 TL = new Vector2(roomList[temp].getLeft(), roomList[temp].getTop());                //top left corner of selected room.

                if (rand.Next(0, 2) == 1)
                {
                    TL.X += rand.Next((-w / 2) + 1, roomList[temp].getWidth() - ((w / 2) + 1));
                    if (rand.Next(0, 2) == 1)
                    {
                        TL.Y += roomList[temp].getHeight() - 1;
                        tTop = (int)TL.Y;
                        nesw = 2;
                    }
                    else
                    {
                        tTop = (int)TL.Y - h + 1;
                        nesw = 0;
                    }
                    tLeft = (int)TL.X;

                }
                else
                {
                    TL.Y += rand.Next((-h / 2) + 1, roomList[temp].getHeight() - ((h / 2) + 1));
                    if (rand.Next(0, 2) == 1)
                    {
                        TL.X += roomList[temp].getWidth() - 1;
                        tLeft = (int)TL.X;
                        nesw = 1;
                    }
                    else
                    {
                        tLeft = (int)TL.X + 1 - w;
                        nesw = 3;
                    }
                    tTop = (int)TL.Y;
                }

                if (0 < tLeft && tLeft + w < (Consts.mapWidth - 1) && 0 < tTop && tTop + h < (Consts.mapHeight - 3))       //checks if the new room is within the bounds of the map.
                {
                    bool overlap = false;

                    foreach(Room room in roomList)
                    {
                        for(int i = 1; i < room.getWidth() - 1; i++)                //checks if the room overlaps any other room by more than one tile.
                        {
                            for(int j = 1; j < room.getHeight() - 1; j++)
                            {

                                for(int ii = tLeft; ii < tLeft + w; ii++)
                                {
                                    for(int jj = tTop; jj < tTop + h; jj++)
                                    {
                                        if(i + room.getLeft() == ii && j + room.getTop() == jj)
                                        {
                                            overlap = true;
                                            break;
                                        }
                                    }
                                    if (overlap) break;
                                }
                                if (overlap) break;
                            }
                            if (overlap) break;
                        }
                        if (overlap) break;
                    }

                    if(!overlap)
                    {
                        Room tRoom = new Room();
                        tRoom.setLayout(tLeft, tTop, w, h);
                        roomList.Add(tRoom);
                        getSubTiles(tRoom, maps);
                        switch (nesw)   //0 = N, 1 = E, 2 = S, 3 = W.
                        {
                            case 0:
                                maps[(int)TL.X + (w/2), (int)TL.Y] = 0;
                                break;
                            
                            case 1:
                                maps[(int)TL.X, (int)TL.Y + (h/2)] = 0;
                                break;

                            case 2:
                                maps[(int)TL.X + (w / 2), (int)TL.Y] = 0;
                                break;

                            case 3:
                                maps[(int)TL.X, (int)TL.Y + (h / 2)] = 0;
                                break;

                        }
                        numRooms++;
                    }

                    float pFilled = 0;
                    for (int i = 0; i < Consts.mapWidth; i++)
                    {
                        for (int j = 0; j < Consts.mapHeight; j++)
                        {
                            if (maps[i, j] != -1) pFilled += 1;
                        }
                    }

                    pFilled = pFilled / (Consts.mapWidth * Consts.mapHeight);
                    if (pFilled > Consts.filledPercentage)
                        done = true;
                }
            }

            bool eDone = false;
            while (!eDone)                                  //find a valid spot for the exit.
            {
                int eRoom = rand.Next(1, roomList.Count());
                Vector2 ePos = roomList[eRoom].getCenter();

                if (maps[(int)ePos.X, (int)ePos.Y] == 0)
                {
                    exit = ePos;
                    eDone = true;
                }
            }

            maps[(int)exit.X, (int)exit.Y] = 3;

            for (int i = 0; i < 30; i++)
            {
                switch(rand.Next(0,3))
                {
                    case 0:
                        enemyList[i] = roomList[rand.Next(1, roomList.Count)].addEnemy(10, 100, 100, 11, 10);
                        break;

                    case 1:
                        enemyList[i] = roomList[rand.Next(1, roomList.Count)].addEnemy(15, 200, 150, 12, 15);
                        break;
                            
                    case 2:
                        enemyList[i] = roomList[rand.Next(1, roomList.Count)].addEnemy(2, 25, 33, 13, 5);
                        break;
                }
                


                Vector2 temp = enemyList[i].GridPosition;
                maps[(int)temp.X, (int)temp.Y] = enemyList[i].CV;
            }
            
            return maps;
        }

        void rSeed()
        {
            float ratio = 0;
            do
            {
                w = rand.Next(Consts.minRoomWidth, Consts.maxRoomWidth);
                h = rand.Next(Consts.minRoomHeight, Consts.maxRoomHeight);
                ratio = w / h;
            } while (ratio < 2 && ratio > 0.5);
        }

        void getSubTiles(Layouts layout, int[,]tiles)
        {

            int[,] subTile = new int[26, 24];

            subTile = layout.getLayout();

            int left = layout.getLeft();
            int top = layout.getTop();
            
            for (int i = 0; i < layout.getWidth(); i++)
            {
                for (int j = 0; j < layout.getHeight(); j++)
                {
                    tiles[i + left, j + top] = subTile[i, j];
                }
            }
        }

        public Vector2 getSpawn()
        {
            return spawnRoom.getCenter();
        }
        public Vector2 getExit()
        {
            return exit;
        }

        public EnemyBase[] getEnemies()
        {
            return enemyList;
        }
    }
}
