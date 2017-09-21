using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roguelike
{
    class EnemyBase
    {
        private Vector2 gridPosition;   //X position, Y position on grid
        private Vector2 targetPosition;
        private Vector2 screenPosition;
        private Vector2 exit;
        private const int moveTime = 50; //miliseconds

        private int colourValue;

        private int APtotal;
        private int ACost;

        private float health;
        private float hpMax;
        private int attack;
        private int xpValue;

        private bool cleared;

        //accessors
        public Vector2 GridPosition
        {
            get { return gridPosition; }
        }
        public Vector2 ScreenPosition
        {
            get { return screenPosition; }
        }
        public int Atk
        {
            get { return attack; }
        }
        public float HP
        {
            get { return health; }
        }
        public int CV
        {
            get { return colourValue;  }
        }
        public int SV
        {
            get { return xpValue; }
        }

        //constructor: takes an initial position as arguments
        public EnemyBase(int x, int y, int hp, int atk, int AC, int CV, int XPV)
        {
            gridPosition = new Vector2(x, y);
            targetPosition = gridPosition;
            screenPosition = new Vector2(x, y);
            cleared = false;
            APtotal = 0;
            ACost = AC;
            hpMax = hp;
            health = 1;
            attack = atk;
            colourValue = CV;
            xpValue = XPV;
        }

        //Handles animation moving from current grid position (gridLocation) to next grid position (targetLocation)
        public void Update(GameTime gameTime)
        {
            if (screenPosition != gridPosition) screenPosition = gridPosition;
        }

        //sets next position for player to move to: called by keyboard processing functions. validates new position against level,
        //so can't move to blocked position, or position off grid
        public void SetNextLocation(int newLocX, int newLocY, int validTarget)
        {

            switch(validTarget)
            {
                case 0:
                case 2:
                    gridPosition = new Vector2(newLocX, newLocY);
                    break;

                case 1:
                case 3:
                    break;

            }

        }

        public int doMonsterTurn(Player target, int[,] map)
        {

            int numAtk = 0;

            while (APtotal > ACost)
            {
                targetPosition = gridPosition;

                int tileDist = (int)(Math.Abs(gridPosition.X - (target.GridPosition.X / 10)) + Math.Abs(gridPosition.Y - (target.GridPosition.Y / 10)));

                if (tileDist <= 1)
                {
                    numAtk++;
                }
                else if (tileDist < 10)
                {
                    if (target.GridPosition.X / 10 < gridPosition.X)
                    {
                        targetPosition.X -= 1;
                    }
                    else if (target.GridPosition.X / 10 > gridPosition.X)
                    {
                        targetPosition.X += 1;
                    }
                    else if (target.GridPosition.Y / 10 < gridPosition.Y)
                    {
                        targetPosition.Y -= 1;
                    }
                    else if (target.GridPosition.Y / 10 > gridPosition.Y)
                    {
                        targetPosition.Y += 1;
                    }
                    //targetPosition.X = tPos.X;
                    //targetPosition.Y = tPos.Y;

                    if (targetPosition != gridPosition)
                    {
                        map[(int)gridPosition.X, (int)gridPosition.Y] = 0;
                        SetNextLocation((int)targetPosition.X, (int)targetPosition.Y, map[(int)targetPosition.X, (int)targetPosition.Y]);
                        map[(int)gridPosition.X, (int)gridPosition.Y] = colourValue;
                    }
                }

                APtotal -= ACost;
            }
            return numAtk;

        }

        public void jumpTo(Vector2 newPosition)
        {
            gridPosition = newPosition * 10;
            screenPosition = gridPosition;
        }

        public void setExit(Vector2 coord)
        {
            exit = coord;
        }

        public bool isCleared()
        {
            return cleared;
        }

        public void newLevel()
        {
            cleared = false;
        }

        public void AddAP(int amt)
        {
            APtotal += amt;
        }

        public float hit(float damage)
        {
            float hpLoss = damage/hpMax;
            health -= hpLoss;
            return health;
        }

    }
}
