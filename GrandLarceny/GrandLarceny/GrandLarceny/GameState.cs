﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	class GameState : States
	{
		private LinkedList<GameObject> m_gameObjectList;
		private LinkedList<GameObject> m_killList = new LinkedList<GameObject>();
		KeyboardState m_previous;
		KeyboardState m_current;

		private Player player;

		public GameState() 
		{

		}

		public override void load()
		{
			m_gameObjectList = Loader.getInstance().loadLevel(1);
			Game.getInstance().m_camera.setParentPosition(player.getPosition());
		}

		public override void setPlayer(Player a_player)
		{
			player = a_player;
		}

		/*
		Update-metod, går igenom alla objekt i scenen och kallas på deras update
		och kollar sedan om de ska dö och läggs därefter i dödslistan.
		Dödslistan loopas sedan igenom och tar bort de objekt som ska dö ifrån
		objektlistan.
		*/
		public override void update(GameTime a_gameTime)
		{
			m_current = Keyboard.GetState();

			foreach (GameObject t_gameObject in m_gameObjectList)
			{
				t_gameObject.update(a_gameTime);
			}

			if (m_current.IsKeyDown(Keys.R))
			{
				Game.getInstance().setState(new GameState());
			}

            foreach (GameObject t_firstGameObject in m_gameObjectList)
			{
				List<Entity> t_collided = new List<Entity>();
                
				if (t_firstGameObject is Entity)
				{
					foreach (GameObject t_secondGameObject in m_gameObjectList)
					{
						if (t_secondGameObject is Entity && t_firstGameObject != t_secondGameObject && checkBigBoxCollision(t_firstGameObject, t_secondGameObject))
						{
							t_collided.Add((Entity)t_secondGameObject);
						}
					}
					t_firstGameObject.collisionCheck(t_collided);
				}

				if (t_firstGameObject.isDead())
				{
					m_killList.AddLast(t_firstGameObject);
				}
			}

			foreach (GameObject t_gameObject in m_killList)
			{
				m_gameObjectList.Remove(t_gameObject);
			}
			m_previous = m_current;
		}
		/*
		Draw-metod, loopar igenom alla objekt och ber dem ritas ut på skärmen 
		*/
		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (GameObject t_gameObject in m_gameObjectList)
			{
				t_gameObject.draw(a_gameTime);
			}
		}

		public static bool checkBigBoxCollision(GameObject a_first, GameObject a_second)
		{
			return (a_first.getLeftPoint()-1 < a_second.getRightPoint() &&
				a_first.getRightPoint()+1 > a_second.getLeftPoint()) &&
				(a_first.getTopPoint() < a_second.getBottomPoint() &&
				a_first.getBottomPoint() > a_second.getTopPoint());
        }
		public static bool checkBoxCollision(GameObject a_first, GameObject a_second)
		{
			return (a_first.getLeftPoint() < a_second.getRightPoint() &&
				a_first.getRightPoint() > a_second.getLeftPoint()) &&
				(a_first.getTopPoint() < a_second.getBottomPoint() &&
				a_first.getBottomPoint() > a_second.getTopPoint());
		}
	}
}
