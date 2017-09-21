using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roguelike
{
    class Player
    {
        private Vector2 gridPosition;   //X position, Y position on grid
        private Vector2 targetPosition; //X position, Y position on grid
        private Vector2 screenPosition; //X position, Y position on grid
        private Vector2 exit;

        private int timerMs = 0;
        private const int moveTime = 50; //miliseconds

        private bool cleared;
        private bool canMove;
        private bool attacking;

        private int hp;
        private int damage;

        //accessors
        public Vector2 GridPosition
        {
            get { return gridPosition; }
        }
        public Vector2 TargetPosition
        {
            get { return targetPosition; }
        }
        public Vector2 ScreenPosition
        {
            get { return screenPosition; }
        }
        public bool isMove
        {
            set { canMove = value; }
            get { return canMove; }
        }
        public bool attack
        {
            set { attacking = value; }
            get { return attacking; }
        }
        public int Dmg
        {
            get { return damage; }
        }
        public int Health
        {
            get { return hp; }
        }


        //constructor: takes an initial position as arguments
        public Player(int x, int y)
        {
            gridPosition = new Vector2(x, y);
            targetPosition = new Vector2(x, y);
            screenPosition = new Vector2(x, y);
            timerMs = moveTime;
            cleared = false;
            canMove = true;
            attacking = false;

            damage = 10;
            hp = 10;

        }

        //Handles animation moving from current grid position (gridLocation) to next grid position (targetLocation)
        public void Update(GameTime gameTime)
        {
            
            if (gridPosition == exit * 10)
            {
                cleared = true;
            }

            //calculate screen position
            screenPosition = (gridPosition * 1) + ((((targetPosition * 1) - (gridPosition * 1)) * (moveTime - timerMs)) / (moveTime*2));
        }

        //sets next position for player to move to: called by keyboard processing functions. validates new position against level,
        //so can't move to blocked position, or position off grid
        public void jump(int newLocX, int newLocY, int validTarget)
        {
            
            switch (validTarget)
            {
                case 0:
                case 3:
                    jumpRegardless(new Vector2(newLocX, newLocY));
                    canMove = false;
                    break;

                case 1:
                case 2:
                    break;

                case 11:
                case 12:
                case 13:
                    targetPosition.X = newLocX * 10;
                    targetPosition.Y = newLocY * 10;
                    attacking = true;
                    canMove = false;
                    break;

            }

        }

        public void jumpRegardless(Vector2 newPosition)
        {
            gridPosition = newPosition * 10;
            targetPosition = gridPosition;
            screenPosition = targetPosition;
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
            hp = 10;
        }

        public int hit(float dmg)
        {
            if(dmg < 100)
            {
                Random bloop = new Random();
                if (bloop.Next(0, 100) <= dmg)
                {
                    dmg = 100;
                }
                else dmg = 0;
            }
            hp -= (int)(dmg/100);
            return hp;
        }

        public void respawn()
        {
            hp = 10;
        }

    }
}
