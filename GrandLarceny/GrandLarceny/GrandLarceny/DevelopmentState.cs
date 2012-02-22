﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text.RegularExpressions;

namespace GrandLarceny
{
	class DevelopmentState : States
	{
		private LinkedList<GameObject> m_gameObjectList;
		private LinkedList<GameObject> m_buildObjectList;
		private LinkedList<GuiObject> m_guiList;
		private LinkedList<Button> m_buttonList;
		private LinkedList<Button> m_assetButtonList;
		private LinkedList<Text> m_textList;
		private MouseState m_previousMouse;
		private MouseState m_currentMouse;
		private KeyboardState m_previousKeyboard;
		private KeyboardState m_currentKeyboard;

		private GameObject m_selectedObject;
		private GameObject m_objectPreview;
		private string m_levelToLoad;

		private Vector2 m_selectedInfoV2;

		private SpriteFont m_courierNew;
		private Vector2 m_worldMouse;
		private GameObject m_player;

		private Text m_textCurrentMode;
		private Text m_textSelectedObjectPosition;
		private Text m_textGuardInfo;
		private GuiObject m_UItextBackground;

		private Button m_btnLadderHotkey;
		private Button m_btnTileHotkey;
		private Button m_btnBackgroundHotkey;
		private Button m_btnDeleteHotkey;
		private Button m_btnHeroHotkey;
		private Button m_btnSelectHotkey;
		private Button m_btnSpotlightHotkey;
		private Button m_btnGuardHotkey;
		private Button m_btnWallHotkey;
		private Button m_btnPropHotkey;

		private int TILE_WIDTH = 72;
		private int TILE_HEIGHT = 72;
		private string assetToCreate = null;

		private enum State
		{
			Platform,
			Player,
			Background,
			Ladder,
			SpotLight,
			Delete,
			None,
			Guard,
			Wall,
			GuardLeft,
			GuardRight,
			Prop
		}
		private State m_itemToCreate;

		public DevelopmentState(string a_levelToLoad)
		{
			m_levelToLoad = a_levelToLoad;
			Game.getInstance().m_camera.getPosition().setParentPosition(null);
			Game.getInstance().m_camera.setPosition(new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth / 2, Game.getInstance().m_graphics.PreferredBackBufferHeight / 2));
		}

		private Vector2 getTile(Vector2 a_pixelPosition)
		{
			if (a_pixelPosition.X >= 0)
				a_pixelPosition.X = a_pixelPosition.X - (a_pixelPosition.X % TILE_WIDTH);
			else
				a_pixelPosition.X = a_pixelPosition.X - (a_pixelPosition.X % TILE_WIDTH) - TILE_WIDTH;

			if (a_pixelPosition.Y >= 0)
				a_pixelPosition.Y = a_pixelPosition.Y - (a_pixelPosition.Y % TILE_HEIGHT);
			else
				a_pixelPosition.Y = a_pixelPosition.Y - (a_pixelPosition.Y % TILE_HEIGHT) - TILE_HEIGHT;

			return a_pixelPosition;
		}

		public override void load()
		{
			m_courierNew = Game.getInstance().Content.Load<SpriteFont>("Fonts//Courier New");
			m_gameObjectList = Loader.getInstance().loadLevel(m_levelToLoad);
			m_guiList			= new LinkedList<GuiObject>();
			m_textList			= new LinkedList<Text>();
			m_buttonList		= new LinkedList<Button>();
			m_buildObjectList	= new LinkedList<GameObject>();
			m_assetButtonList	= new LinkedList<Button>();
			m_objectPreview = null;


			foreach (GameObject t_gameObject in m_gameObjectList)
			{
				if (t_gameObject is Player)
				{
					m_player = t_gameObject;
					break;
				}
			}

			m_textCurrentMode				= new Text(new Vector2(12, 10), "null", m_courierNew, Color.Black, false);
			m_textSelectedObjectPosition	= new Text(new Vector2(12, 42), "Nothing Selected", m_courierNew, Color.Black, false);
			m_textGuardInfo = new Text(new Vector2(12, 74), "", m_courierNew, Color.Black, false);
			m_textList.AddLast(m_textCurrentMode);
			m_textList.AddLast(m_textSelectedObjectPosition);
			m_textList.AddLast(m_textGuardInfo);

			m_UItextBackground = new GuiObject(new Vector2(0, 0), "dev_bg_info");
			m_guiList.AddLast(m_UItextBackground);

			m_btnLadderHotkey = new Button("btn_ladder_hotkey_normal", "btn_ladder_hotkey_hover", "btn_ladder_hotkey_pressed"
				, new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth - TILE_WIDTH * 1
					, Game.getInstance().m_graphics.PreferredBackBufferHeight - TILE_HEIGHT * 1), null, 0.002f);
			m_btnTileHotkey = new Button("btn_tile_hotkey_normal", "btn_tile_hotkey_hover", "btn_tile_hotkey_pressed"
				, new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth - TILE_WIDTH * 2
					, Game.getInstance().m_graphics.PreferredBackBufferHeight - TILE_HEIGHT * 1), null, 0.002f);
			m_btnBackgroundHotkey = new Button("btn_background_hotkey_normal", "btn_background_hotkey_hover", "btn_background_hotkey_pressed"
				, new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth - TILE_WIDTH * 3
					, Game.getInstance().m_graphics.PreferredBackBufferHeight - TILE_HEIGHT * 1), null, 0.002f);
			m_btnDeleteHotkey = new Button("btn_delete_hotkey_normal", "btn_delete_hotkey_hover", "btn_delete_hotkey_pressed"
				, new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth - TILE_WIDTH * 4
					, Game.getInstance().m_graphics.PreferredBackBufferHeight - TILE_HEIGHT * 1), null, 0.002f);
			m_btnHeroHotkey = new Button("btn_hero_hotkey_normal", "btn_hero_hotkey_hover", "btn_hero_hotkey_pressed"
				, new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth - TILE_WIDTH * 1
				, Game.getInstance().m_graphics.PreferredBackBufferHeight - TILE_HEIGHT * 2), null, 0.002f);
			m_btnSelectHotkey = new Button("btn_select_hotkey_normal", "btn_select_hotkey_hover", "btn_select_hotkey_pressed"
				, new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth - TILE_WIDTH * 2
					, Game.getInstance().m_graphics.PreferredBackBufferHeight - TILE_HEIGHT * 2), null, 0.002f);
			m_btnSpotlightHotkey = new Button("btn_spotlight_hotkey_normal", "btn_spotlight_hotkey_hover", "btn_spotlight_hotkey_pressed"
				, new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth - TILE_WIDTH * 3
					, Game.getInstance().m_graphics.PreferredBackBufferHeight - TILE_HEIGHT * 2), null, 0.002f);
			m_btnGuardHotkey = new Button("btn_guard_hotkey_normal", "btn_guard_hotkey_hover", "btn_guard_hotkey_pressed"
				, new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth - TILE_WIDTH * 4
					, Game.getInstance().m_graphics.PreferredBackBufferHeight - TILE_HEIGHT * 2), null, 0.002f);
			m_btnWallHotkey = new Button("btn_wall_hotkey_normal", "btn_wall_hotkey_hover", "btn_wall_hotkey_pressed"
				, new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth - TILE_WIDTH * 1
					, Game.getInstance().m_graphics.PreferredBackBufferHeight - TILE_HEIGHT * 3), null, 0.002f);
			m_btnPropHotkey = new Button("btn_tile_hotkey_normal", "btn_tile_hotkey_hover", "btn_tile_hotkey_pressed"
				, new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth - TILE_WIDTH * 2
					, Game.getInstance().m_graphics.PreferredBackBufferHeight - TILE_HEIGHT * 4), null, 0.002f);

			m_buttonList.AddLast(m_btnLadderHotkey);
			m_buttonList.AddLast(m_btnTileHotkey);
			m_buttonList.AddLast(m_btnBackgroundHotkey);
			m_buttonList.AddLast(m_btnDeleteHotkey);
			m_buttonList.AddLast(m_btnHeroHotkey);
			m_buttonList.AddLast(m_btnSelectHotkey);
			m_buttonList.AddLast(m_btnSpotlightHotkey);
			m_buttonList.AddLast(m_btnGuardHotkey);
			m_buttonList.AddLast(m_btnWallHotkey);
			m_buttonList.AddLast(m_btnPropHotkey);

			m_btnLadderHotkey.m_clickEvent		+= new Button.clickDelegate(guiButtonClick);
			m_btnTileHotkey.m_clickEvent		+= new Button.clickDelegate(guiButtonClick);
			m_btnBackgroundHotkey.m_clickEvent	+= new Button.clickDelegate(guiButtonClick);
			m_btnHeroHotkey.m_clickEvent		+= new Button.clickDelegate(guiButtonClick);
			m_btnSelectHotkey.m_clickEvent		+= new Button.clickDelegate(guiButtonClick);
			m_btnSpotlightHotkey.m_clickEvent	+= new Button.clickDelegate(guiButtonClick);
			m_btnGuardHotkey.m_clickEvent		+= new Button.clickDelegate(guiButtonClick);
			m_btnWallHotkey.m_clickEvent		+= new Button.clickDelegate(guiButtonClick);
			m_btnDeleteHotkey.m_clickEvent		+= new Button.clickDelegate(guiButtonClick);
			m_btnPropHotkey.m_clickEvent		+= new Button.clickDelegate(guiButtonClick);

			setBuildingState(State.None);
		}

		public override void update(GameTime a_gameTime)
		{
			m_currentKeyboard = Keyboard.GetState();
			m_currentMouse = Mouse.GetState();

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

		private void updateGUI()
		{
			if (m_objectPreview != null) {
				m_objectPreview.getPosition().setX(m_worldMouse.X + 15);
				m_objectPreview.getPosition().setY(m_worldMouse.Y + 15);
			}

			foreach (Button t_button in m_assetButtonList)
				t_button.update();
			foreach (Button t_button in m_buttonList)
				t_button.update();

			if (m_selectedObject != null)
			{
				m_selectedInfoV2.X = getTile(m_selectedObject.getPosition().getGlobalCartesianCoordinates()).X / TILE_WIDTH;
				m_selectedInfoV2.Y = getTile(m_selectedObject.getPosition().getGlobalCartesianCoordinates()).Y / TILE_HEIGHT;
				m_textSelectedObjectPosition.setText(m_selectedInfoV2.ToString());
				if (m_selectedObject is Guard) {
					Guard t_guard = (Guard)m_selectedObject;
					m_textGuardInfo.setText("R: " + t_guard.getRightpatrolPoint() + " L: " + t_guard.getLeftpatrolPoint());
				}
			}
		}

		private void updateKeyboard()
		{
			if (m_currentKeyboard.IsKeyDown(Keys.R) && m_previousKeyboard.IsKeyUp(Keys.R))
			{
				Game.getInstance().setState(new GameState(m_levelToLoad));
			}
			if (m_currentKeyboard.IsKeyDown(Keys.P) && m_previousKeyboard.IsKeyUp(Keys.P))
				setBuildingState(State.Platform);

			if (m_currentKeyboard.IsKeyDown(Keys.L) && m_previousKeyboard.IsKeyUp(Keys.L))
				setBuildingState(State.Ladder);

			if (m_currentKeyboard.IsKeyDown(Keys.B) && m_previousKeyboard.IsKeyUp(Keys.B))
				setBuildingState(State.Background);

			if (m_currentKeyboard.IsKeyDown(Keys.D) && m_previousKeyboard.IsKeyUp(Keys.D))
				setBuildingState(State.Delete);

			if (m_currentKeyboard.IsKeyDown(Keys.H) && m_previousKeyboard.IsKeyUp(Keys.H))
				setBuildingState(State.Player);

			if (m_currentKeyboard.IsKeyDown(Keys.S) && m_previousKeyboard.IsKeyUp(Keys.S))
				setBuildingState(State.None);

			if (m_currentKeyboard.IsKeyDown(Keys.T) && m_previousKeyboard.IsKeyUp(Keys.T))
				setBuildingState(State.SpotLight);

			if (m_currentKeyboard.IsKeyDown(Keys.G) && m_previousKeyboard.IsKeyUp(Keys.G))
				setBuildingState(State.Guard);

			if (m_currentKeyboard.IsKeyDown(Keys.W) && m_previousKeyboard.IsKeyUp(Keys.W))
				setBuildingState(State.Wall);

			if (m_currentKeyboard.IsKeyDown(Keys.N) && m_previousKeyboard.IsKeyUp(Keys.N))
				setBuildingState(State.GuardLeft);

			if (m_currentKeyboard.IsKeyDown(Keys.M) && m_previousKeyboard.IsKeyUp(Keys.M))
				setBuildingState(State.GuardRight);

			if (m_currentKeyboard.IsKeyDown(Keys.O) && m_previousKeyboard.IsKeyUp(Keys.O))
				setBuildingState(State.Prop);

			if (m_currentKeyboard.IsKeyDown(Keys.Space) && m_previousKeyboard.IsKeyUp(Keys.Space))
				if (m_gameObjectList != null)
					Game.getInstance().m_camera.setPosition(m_gameObjectList.First().getPosition().getGlobalCartesianCoordinates());

			if (m_currentKeyboard.IsKeyDown(Keys.LeftControl) && m_currentKeyboard.IsKeyDown(Keys.S) && m_previousKeyboard.IsKeyUp(Keys.S))
			{
				if (m_selectedObject != null)
				{
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
			m_worldMouse.X = 
				Mouse.GetState().X / Game.getInstance().m_camera.getZoom()
				+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().X
				- ((Game.getInstance().m_graphics.PreferredBackBufferWidth / 2) / Game.getInstance().m_camera.getZoom());
			m_worldMouse.Y = 
				Mouse.GetState().Y / Game.getInstance().m_camera.getZoom() 
				+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().Y
				- ((Game.getInstance().m_graphics.PreferredBackBufferHeight / 2) / Game.getInstance().m_camera.getZoom());

			if (m_currentMouse.MiddleButton == ButtonState.Pressed && m_previousMouse.MiddleButton == ButtonState.Pressed) {
				Vector2 t_difference = Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates();
				t_difference.X = (Mouse.GetState().X - Game.getInstance().m_graphics.PreferredBackBufferWidth / 2) / 10;
				t_difference.Y = (Mouse.GetState().Y - Game.getInstance().m_graphics.PreferredBackBufferHeight / 2) / 10;
				Game.getInstance().m_camera.getPosition().plusWith(t_difference);
			}
			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Pressed && m_selectedObject != null) 
				updateMouseDrag();
			
			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Released && m_itemToCreate != State.Delete && m_itemToCreate != State.None && !collidedWithGui())
			{
				if (assetToCreate != null) {
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
						case State.Guard:
						{
							createGuard();
							break;
						}
						case State.Wall:
						{
							createWall();
							break;
						}
						case State.Prop:
						{
							createProp();
							break;
						}
					}
				} else {
					switch (m_itemToCreate) {
						case State.GuardLeft:
						{
							setGuardPoint((Guard)m_selectedObject, false);
							break;
						}
						case State.GuardRight:
						{
							setGuardPoint((Guard)m_selectedObject, true);
							break;
						}
					}
				}
				return;
			}

			if (m_currentMouse.RightButton == ButtonState.Pressed && m_itemToCreate != State.None)
				setBuildingState(State.None);

			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Released) 
			{
				if (m_selectedObject != null)
				{
					m_selectedObject.setColor(Color.White);
					m_selectedObject = null;
					m_selectedInfoV2 = Vector2.Zero;
				}
				Rectangle t_mouseClick = new Rectangle((int)m_worldMouse.X, (int)m_worldMouse.Y, 1, 1);

				if (!collidedWithGui()) {
					foreach (GameObject t_gameObject in m_gameObjectList)
					{
						if (t_gameObject.getBox().Contains((int)m_worldMouse.X, (int)m_worldMouse.Y))
						{
							if (t_gameObject is LightCone)
								continue;
							if (m_selectedObject == null || m_selectedObject.getLayer() > t_gameObject.getLayer())
								m_selectedObject = t_gameObject;
						}
					}
					if (m_itemToCreate == State.Delete)
					{
						deleteObject(m_selectedObject);
						m_selectedInfoV2 = Vector2.Zero;
						m_selectedObject = null;
						return;
					}
					if (m_selectedObject != null)
					{
						if (m_selectedObject is Guard)
							showGuardInfo((Guard)m_selectedObject);
						m_selectedObject.setColor(Color.Yellow);
					}
				}
			}
		}

		private void updateMouseDrag()
		{
			Vector2 t_mousePosition = getTile(m_worldMouse);

			m_selectedObject.getPosition().setX(t_mousePosition.X);
			m_selectedObject.getPosition().setY(t_mousePosition.Y);
		}

		private void resetButtonStates()
		{
			foreach (Button t_button in m_buttonList)
				t_button.setState(0);
		}

		private bool collidedWithGui()
		{
			foreach (GuiObject t_guiObject in m_guiList)
				if (t_guiObject.getBox().Contains((int)m_worldMouse.X, (int)m_worldMouse.Y))
					return true;
			foreach (Button t_button in m_buttonList)
				if (t_button.getBox().Contains((int)Mouse.GetState().X, (int)Mouse.GetState().Y))
					return true;
			foreach (Button t_button in m_assetButtonList)
				if (t_button.getBox().Contains((int)Mouse.GetState().X, (int)Mouse.GetState().Y))
					return true;
			return false;
		}

		public bool collidedWithObject() {
			Rectangle t_rectangle = new Rectangle((int)getTile(m_worldMouse).X, (int)getTile(m_worldMouse).Y, 1, 1);

			foreach (GameObject t_gameObject in m_gameObjectList) {
				if (t_gameObject is Environment)
					continue;
				if (t_gameObject.getBox().Contains(t_rectangle))
					return true;
			}
			return false;
		}

		public void guiButtonClick(Button a_button)
		{
			if (a_button == m_btnLadderHotkey)
				setBuildingState(State.Ladder);
			if (a_button == m_btnTileHotkey)
				setBuildingState(State.Platform);
			if (a_button == m_btnBackgroundHotkey)
				setBuildingState(State.Background);
			if (a_button == m_btnDeleteHotkey)
				setBuildingState(State.Delete);
			if (a_button == m_btnHeroHotkey)
				setBuildingState(State.Player);
			if (a_button == m_btnSelectHotkey)
				setBuildingState(State.None);
			if (a_button == m_btnSpotlightHotkey)
				setBuildingState(State.SpotLight);
			if (a_button == m_btnGuardHotkey)
				setBuildingState(State.Guard);
			if (a_button == m_btnWallHotkey)
				setBuildingState(State.Wall);
			if (a_button == m_btnDeleteHotkey)
				setBuildingState(State.Delete);
			if (a_button == m_btnPropHotkey)
				setBuildingState(State.Prop);
		}
		
		private void createAssetList(string a_assetDirectory) {
			m_assetButtonList = new LinkedList<Button>();
			if (a_assetDirectory == null)
				return;
			string[] t_levelList = Directory.GetFiles(a_assetDirectory);
			for (int i = 0, j = 0; i < t_levelList.Length; i++) {
				if ((t_levelList[i].EndsWith(".xnb") == false))
					continue;
				Button t_button = new Button(
					"btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed",
					new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth - 160, 21 * j), null, 0.002f
				);
				string[] t_splitPath = Regex.Split(t_levelList[i], "//");
				t_button.setText(t_splitPath[t_splitPath.Length - 1].Remove(t_splitPath[t_splitPath.Length - 1].Length - 4), new Vector2(7, 1));
				t_button.m_clickEvent += new Button.clickDelegate(selectAsset);
				m_assetButtonList.AddLast(t_button);
				j++;
			}
		}

		private void setBuildingState(State a_state) {
			m_itemToCreate = a_state;
			assetToCreate = null;
			m_objectPreview = null;
			resetButtonStates();

			switch (m_itemToCreate) {
				case State.Platform:
				{
					m_btnTileHotkey.setState(2);
					m_textCurrentMode.setText("Create Platform");
					createAssetList("Content//Images//Tile//");
					selectAsset(m_assetButtonList.First());
					break;
				}
				case State.Ladder:
				{
					m_btnLadderHotkey.setState(2);
					m_textCurrentMode.setText("Create Ladder");
					createAssetList("Content//Images//Tile//");
					selectAsset(m_assetButtonList.First());
					break;
				}
				case State.Background:
				{
					m_btnBackgroundHotkey.setState(2);
					m_textCurrentMode.setText("Create Background");
					createAssetList("Content//Images//Background//");
					selectAsset(m_assetButtonList.First());
					break;
				}
				case State.Delete:
				{
					m_btnDeleteHotkey.setState(2);
					m_textCurrentMode.setText("Delete Object");
					createAssetList(null);
					break;
				}
				case State.Player:
				{
					m_btnHeroHotkey.setState(2);
					m_textCurrentMode.setText("Create Hero");
					createAssetList("Content//Images//Sprite//");
					selectAsset(m_assetButtonList.First());
					break;
				}
				case State.None:
				{
					m_btnSelectHotkey.setState(2);
					m_textCurrentMode.setText("Select");
					createAssetList(null);
					break;
				}
				case State.SpotLight:
				{
					m_btnSpotlightHotkey.setState(2);
					m_textCurrentMode.setText("Create SpotLight");
					createAssetList("Content//Images//LightCone//");
					selectAsset(m_assetButtonList.First());
					break;
				}
				case State.Guard:
				{
					m_btnGuardHotkey.setState(2);
					m_textCurrentMode.setText("Create Guard");
					createAssetList("Content//Images//Sprite//");
					selectAsset(m_assetButtonList.First());
					break;
				}
				case State.Wall:
				{
					m_btnWallHotkey.setState(2);
					m_textCurrentMode.setText("Create Wall");
					createAssetList("Content//Images//Tile//");
					selectAsset(m_assetButtonList.First());
					break;
				}
				case State.GuardRight:
				{
					m_textCurrentMode.setText("Set Right Point");
					break;
				}
				case State.GuardLeft:
				{
					m_textCurrentMode.setText("Set Left Point");
					break;
				}
				case State.Prop:
				{
					m_btnPropHotkey.setState(2);
					m_textCurrentMode.setText("Create Prop");
					createAssetList("Content//Images//Prop//");
					selectAsset(m_assetButtonList.First());
					break;
				}
			}
		}

		private void showGuardInfo(Guard a_guard) {
			m_textGuardInfo.setText("R: " + a_guard.getRightpatrolPoint() + " L: " + a_guard.getLeftpatrolPoint());
		}

		private void setGuardPoint(Guard a_guard, bool a_right) {
			if (a_right)
				a_guard.setRightGuardPoint(getTile(m_worldMouse).X);
			else
				a_guard.setLeftGuardPoint(getTile(m_worldMouse).X);
		}

		private void selectAsset(Button a_button)
		{
			assetToCreate = a_button.getText();
			switch (m_itemToCreate) {
				case State.Platform:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//" + assetToCreate, 0.000f);
					break;
				case State.Wall:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//" + assetToCreate, 0.000f);
					break;
				case State.Delete:
					m_objectPreview = null;
					break;
				case State.Guard:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Sprite//" + assetToCreate, 0.000f);
					break;
				case State.Ladder:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//" + assetToCreate, 0.000f);
					break;
				case State.None:
					m_objectPreview = null;
					break;
				case State.Player:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Sprite//" + assetToCreate, 0.000f);
					break;
				case State.SpotLight:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//LightCone//" + assetToCreate, 0.000f);
					break;
				case State.Background:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Background//" + assetToCreate, 0.000f);
					break;
				case State.Prop:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Prop//" + assetToCreate, 0.000f);
					break;
			}
		}

		private void createPlayer()
		{
			if (m_player == null)
			{
				if (collidedWithObject())
					return;
				m_player = new Player(getTile(m_worldMouse), "Images//Sprite//" + assetToCreate, 0.250f);
				m_gameObjectList.AddLast(m_player);
			}
		}

		private void createPlatform()
		{
			if (collidedWithObject())
				return;
			Platform t_platform = new Platform(getTile(m_worldMouse), "Images//Tile//" + assetToCreate, 0.350f);
			m_gameObjectList.AddLast(t_platform);
		}

		private void createLadder()
		{
			if (collidedWithObject())
				return;
			Ladder t_ladder = new Ladder(getTile(m_worldMouse), "Images//Tile//" + assetToCreate, 0.350f);
			m_gameObjectList.AddLast(t_ladder);
		}

		private void createSpotLight()
		{
			if (collidedWithObject())
				return;
			SpotLight t_sl = new SpotLight(getTile(m_worldMouse), "Images//LightCone//"  + assetToCreate, 0.2f, (float)(Math.PI * 0.5f), true);
			m_gameObjectList.AddLast(t_sl);
		}

		private void createBackground()
		{
			Environment t_environment = new Environment(getTile(m_worldMouse), "Images//Background//"  + assetToCreate, 0.750f);
			m_gameObjectList.AddLast(t_environment);
		}

		private void createGuard()
		{
			if (collidedWithObject())
				return;
			Guard t_guard = new Guard(getTile(m_worldMouse), "Images//Sprite//" + assetToCreate, 0.0f, 0.0f, true, false, 0.300f);
			m_gameObjectList.AddLast(t_guard);
		}

		private void createWall()
		{
			if (collidedWithObject())
				return;
			Wall t_wall = new Wall(getTile(m_worldMouse), "Images//Tile//" + assetToCreate, 0.350f);
			m_gameObjectList.AddLast(t_wall);
		}

		private void deleteObject(GameObject a_gameObject)
		{
			if (a_gameObject is Player)
				m_player = null;
			if (a_gameObject is SpotLight)
			{
				LightCone t_lightCone = ((SpotLight)a_gameObject).getLightCone();
				if (t_lightCone != null)
					m_gameObjectList.Remove(t_lightCone);
			}
			m_gameObjectList.Remove(a_gameObject);

		}

		private void createProp()
		{
			if (collidedWithObject())
				return;
			Environment t_prop = new Environment(getTile(m_worldMouse), "Images//Prop//" + assetToCreate, 0.749f);
			m_gameObjectList.AddLast(t_prop);
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (Text t_textObject in m_textList)
				t_textObject.draw(a_spriteBatch);
			foreach (GuiObject t_guiObject in m_guiList)
				t_guiObject.draw(a_gameTime);
			foreach (GameObject t_gameObject in m_gameObjectList)
				t_gameObject.draw(a_gameTime);
			foreach (Button t_button in m_buttonList)
				t_button.draw(a_gameTime, a_spriteBatch);
			foreach (Button t_button in m_assetButtonList)
				t_button.draw(a_gameTime, a_spriteBatch);
			if (m_objectPreview != null)
				m_objectPreview.draw(a_gameTime);
		}

		public override void addObject(GameObject a_object)
		{
			m_gameObjectList.AddLast(a_object);
		}
	}
}