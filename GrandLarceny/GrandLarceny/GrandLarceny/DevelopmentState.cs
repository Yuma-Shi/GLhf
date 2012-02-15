﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	class DevelopmentState : States
	{
		private LinkedList<GameObject> m_gameObjectList;
		private LinkedList<GameObject> m_buildObjectList;
		private LinkedList<GuiObject> m_guiList;
		private MouseState m_previousMouse;
		private MouseState m_currentMouse;
		private KeyboardState m_previousKeyboard;
		private KeyboardState m_currentKeyboard;

		private GameObject m_selectedObject;
		private string m_levelToLoad;

		private string m_currentMode = "Select";
		private Vector2 m_selectedInfoV2;

		private SpriteFont m_courierNew;
		private Vector2 m_worldMouse;
		private GameObject m_player;

		private Position m_UIcurrentModePosition = new CartesianCoordinate(new Vector2(0, 0));
		private Position m_UIselectedObjectPosition = new CartesianCoordinate(new Vector2(0, 0));
		private GuiObject m_UItextBackground = new GuiObject(new Vector2(0, 0), "Images//guiderp");

		private enum State
		{
			Platform,
			Player,
			Background,
			Ladder,
			SpotLight,
			Delete,
			None
		}
		private State m_itemToCreate = State.None;

		public DevelopmentState(string a_levelToLoad)
		{
			m_levelToLoad = a_levelToLoad;
			Game.getInstance().m_camera.setPosition(new Vector2(0, 0));
		}

		private Vector2 getTile(Vector2 a_pixelPosition) {
			if (a_pixelPosition.X % 72 >= 36)
				a_pixelPosition.X = a_pixelPosition.X + (72 - (a_pixelPosition.X % 72));	
			else if (a_pixelPosition.X % 72 < 36)
				a_pixelPosition.X = a_pixelPosition.X - (a_pixelPosition.X % 72);

			if (a_pixelPosition.Y % 72 >= 36)
				a_pixelPosition.Y = a_pixelPosition.Y + (72 - (a_pixelPosition.Y % 72));
			else if (a_pixelPosition.Y % 72 < 36)
				a_pixelPosition.Y = a_pixelPosition.Y - (a_pixelPosition.Y % 72);

			return a_pixelPosition;
		}

		public override void load()
		{
			m_courierNew = Game.getInstance().Content.Load<SpriteFont>("Fonts//Courier New");
			m_gameObjectList = Loader.getInstance().loadLevel(m_levelToLoad);
			foreach (GameObject t_gameObject in m_gameObjectList)
			{
				if (t_gameObject is Player)
				{
					m_player = t_gameObject;
				}
			}
			Game.getInstance().m_camera.setPosition(new Vector2(0, 0));

			m_buildObjectList = new LinkedList<GameObject>();
			m_itemToCreate = State.None;

			m_UIselectedObjectPosition.setParentPosition(Game.getInstance().m_camera.getPosition());
			m_UIselectedObjectPosition.setX(-Game.getInstance().m_graphics.PreferredBackBufferWidth / 2 + 10);
			m_UIselectedObjectPosition.setY(-Game.getInstance().m_graphics.PreferredBackBufferHeight / 2 + 8);

			m_UIcurrentModePosition.setParentPosition(Game.getInstance().m_camera.getPosition());
			m_UIcurrentModePosition.setX(-Game.getInstance().m_graphics.PreferredBackBufferWidth / 2 + 10);
			m_UIcurrentModePosition.setY(-Game.getInstance().m_graphics.PreferredBackBufferHeight / 2 + 40);

			m_UItextBackground.getPosition().setParentPosition(Game.getInstance().m_camera.getPosition());
			m_UItextBackground.getPosition().setX(m_UItextBackground.getImg().getSize().X / 2 - 1);
			m_UItextBackground.getPosition().setY(m_UItextBackground.getImg().getSize().Y / 2 - 1);
		}

		public override void update(GameTime a_gameTime)
		{
			m_currentKeyboard = Keyboard.GetState();
			m_currentMouse = Mouse.GetState();
			
			m_worldMouse.X = 
				Mouse.GetState().X / Game.getInstance().m_camera.getZoom()
				+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().X
				- ((Game.getInstance().m_graphics.PreferredBackBufferWidth / 2) / Game.getInstance().m_camera.getZoom());
			m_worldMouse.Y = 
				Mouse.GetState().Y / Game.getInstance().m_camera.getZoom() 
				+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().Y
				- ((Game.getInstance().m_graphics.PreferredBackBufferHeight / 2) / Game.getInstance().m_camera.getZoom());

			updateCamera();
			updateKeyboard();
			updateMouse();
			updateGUI();

			m_previousKeyboard = m_currentKeyboard;
			m_previousMouse = m_currentMouse;
		}

		private void updateCamera()
		{
			if (m_currentKeyboard.IsKeyDown(Keys.Right))
				Game.getInstance().m_camera.move(new Vector2(15 / Game.getInstance().m_camera.getZoom(), 0));
			if (m_currentKeyboard.IsKeyDown(Keys.Left))
				Game.getInstance().m_camera.move(new Vector2(-15 / Game.getInstance().m_camera.getZoom(), 0));
			if (m_currentKeyboard.IsKeyDown(Keys.Up))
				Game.getInstance().m_camera.move(new Vector2(0, -15 / Game.getInstance().m_camera.getZoom()));
			if (m_currentKeyboard.IsKeyDown(Keys.Down))
				Game.getInstance().m_camera.move(new Vector2(0, 15 / Game.getInstance().m_camera.getZoom()));
			if (m_currentMouse.ScrollWheelValue > m_previousMouse.ScrollWheelValue)
				Game.getInstance().m_camera.zoomIn(0.1f);
			if (m_currentMouse.ScrollWheelValue < m_previousMouse.ScrollWheelValue)
				Game.getInstance().m_camera.zoomOut(0.1f);
		}

		private void updateGUI() {
			if (m_selectedObject != null) {
				m_selectedInfoV2.X = getTile(m_selectedObject.getPosition().getGlobalCartesianCoordinates()).X / 72;
				m_selectedInfoV2.Y = getTile(m_selectedObject.getPosition().getGlobalCartesianCoordinates()).Y / 72;
			}
		}

		private void updateKeyboard()
		{
			if (m_currentKeyboard.IsKeyDown(Keys.R))
			{
				Game.getInstance().setState(new GameState(m_levelToLoad));
			}
			if (m_currentKeyboard.IsKeyDown(Keys.P) && m_previousKeyboard.IsKeyUp(Keys.P))
			{
				m_itemToCreate = State.Platform;
				m_currentMode = "Create Platform";
			}
			if (m_currentKeyboard.IsKeyDown(Keys.L) && m_previousKeyboard.IsKeyUp(Keys.L))
			{
				m_itemToCreate = State.Ladder;
				m_currentMode = "Create Ladder";
			}
			if (m_currentKeyboard.IsKeyDown(Keys.B) && m_previousKeyboard.IsKeyUp(Keys.B))
			{
				m_itemToCreate = State.Background;
				m_currentMode = "Create Background";
			}
			if (m_currentKeyboard.IsKeyDown(Keys.D) && m_previousKeyboard.IsKeyUp(Keys.D))
			{
				m_itemToCreate = State.Delete;
				m_currentMode = "Delete Object";
			}
			if (m_currentKeyboard.IsKeyDown(Keys.H) && m_previousKeyboard.IsKeyUp(Keys.H))
			{
				m_itemToCreate = State.Player;
				m_currentMode = "Create Hero";
			}
			if (m_currentKeyboard.IsKeyDown(Keys.S) && m_previousKeyboard.IsKeyUp(Keys.S))
			{
				m_itemToCreate = State.None;
				m_currentMode = "Select";
			}
			if (m_currentKeyboard.IsKeyDown(Keys.T) && m_previousKeyboard.IsKeyUp(Keys.T))
			{
				m_itemToCreate = State.SpotLight;
				m_currentMode = "Create SpotLight";
			}
			if (m_currentKeyboard.IsKeyDown(Keys.LeftControl) && m_currentKeyboard.IsKeyDown(Keys.S) && m_previousKeyboard.IsKeyUp(Keys.S))
			{
				if (m_selectedObject != null) {
					m_selectedObject.setColor(Color.White);
					m_selectedObject = null;
				}
				Level t_saveLevel = new Level();
				t_saveLevel.setLevelObjects(m_gameObjectList);
				Serializer.getInstace().SaveLevel("Level3.txt", t_saveLevel);

			}
			if (m_currentKeyboard.IsKeyDown(Keys.LeftControl) && m_currentKeyboard.IsKeyDown(Keys.O) && m_previousKeyboard.IsKeyUp(Keys.O))
			{
				Level t_newLevel = Serializer.getInstace().loadLevel("Level3.txt");
				m_gameObjectList = t_newLevel.getLevelObjects();
				foreach (GameObject f_gb in m_gameObjectList)
				{
					f_gb.loadContent();
				}
			}
		}

		private void updateMouse()
		{
			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Pressed && m_selectedObject != null) 
				updateMouseDrag();
			
			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Released && m_itemToCreate != State.None)
			{
				switch (m_itemToCreate)
				{
					case State.Player:
					{
						createPlayer();
						break;
					}
					case State.Background:
					{
						createBackground();
						break;
					}
					case State.Ladder:
					{
						createLadder();
						break;
					}
					case State.Platform:
					{
						createPlatform();
						break;
					}
					case State.SpotLight:
					{
						createSpotLight();
						break;
					}
				}
			}

			if (m_currentMouse.RightButton == ButtonState.Pressed && m_itemToCreate != State.None)
			{
				m_itemToCreate = State.None;
				m_currentMode = "Selection";
			}

			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Released) 
			{
				if (m_selectedObject != null)
				{
					m_selectedObject.setColor(Color.White);
					m_selectedObject = null;
					m_selectedInfoV2 = Vector2.Zero;
				}
				Rectangle t_mouseClick = new Rectangle((int)m_worldMouse.X, (int)m_worldMouse.Y, 1, 1);

				foreach (GameObject t_gameObject in m_gameObjectList)
				{
					if (t_gameObject.getBox().Contains((int)m_worldMouse.X, (int)m_worldMouse.Y))
					{
						m_selectedObject = t_gameObject;
					}
				}
				if (m_selectedObject != null && m_itemToCreate == State.Delete) {
					deleteObject(m_selectedObject);
					m_selectedInfoV2 = Vector2.Zero;
				}
				if (m_selectedObject != null) {
					m_selectedObject.setColor(Color.Yellow);
				}
			}
		}

		private void updateMouseDrag()
		{
			Vector2 t_mousePosition = getTile(m_worldMouse);

			m_selectedObject.getPosition().setX((t_mousePosition.X - (m_selectedObject.getImg().getSize().X / 2)) + 36);
			m_selectedObject.getPosition().setY((t_mousePosition.Y - (m_selectedObject.getImg().getSize().Y / 2)) + 36);
		}

		private void createPlayer()
		{
			if (m_player == null) {
				m_player = new Player(getTile(m_worldMouse), "Images//hero_stand", 0.250f);
				m_gameObjectList.AddLast(m_player);
			}
		}

		private void createPlatform()
		{
			Platform t_platform = new Platform(getTile(m_worldMouse), "Images//tile", 0.350f);
			m_gameObjectList.AddLast(t_platform);
		}

		private void createLadder()
		{
			Ladder t_ladder = new Ladder(getTile(m_worldMouse), "Images//ladder", 0.350f);
			m_gameObjectList.AddLast(t_ladder);
		}

		private void createSpotLight()
		{
			SpotLight t_sl = new SpotLight(getTile(m_worldMouse), "Images//WalkingSquareStand", 0.2f, (float)(Math.PI * 1.5f), true);
			m_gameObjectList.AddLast(t_sl);
		}

		private void createBackground()
		{
			Environment t_environment = new Environment(getTile(m_worldMouse), "Images//test", 0.750f);
			m_gameObjectList.AddLast(t_environment);
		}

		private void deleteObject(GameObject a_gameObject)
		{
			if (a_gameObject is Player)
			{
				m_player = null;
			}
			m_gameObjectList.Remove(a_gameObject);
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			a_spriteBatch.DrawString(m_courierNew, m_selectedInfoV2.ToString(), m_UIselectedObjectPosition.getGlobalCartesianCoordinates(), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
			a_spriteBatch.DrawString(m_courierNew, m_currentMode, m_UIcurrentModePosition.getGlobalCartesianCoordinates(), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
			foreach (GuiObject t_guiObject in m_guiList) {
				t_guiObject.draw(a_gameTime);
			}
			foreach (GameObject t_gameObject in m_gameObjectList)
			{
				t_gameObject.draw(a_gameTime);
			}
		}
		public override void addObject(GameObject a_object)
		{
			m_gameObjectList.AddLast(a_object);
		}
	}
}
