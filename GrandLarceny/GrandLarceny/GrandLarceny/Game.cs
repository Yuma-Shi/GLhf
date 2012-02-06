using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GrandLarceny
{
	public class Game : Microsoft.Xna.Framework.Game
	{
        private static Game m_myGame;

        internal GraphicsDeviceManager m_graphics;
		private SpriteBatch m_spriteBatch;
		
        private States m_nextState;
		private States m_currentState;

		internal Camera m_camera;

		public static Game getInstance()
		{
			if (m_myGame != null)
			{
				return m_myGame;
			}
			else
			{
				m_myGame = new Game();
				return m_myGame;
			}
		}

		private Game()
		{
			m_graphics = new GraphicsDeviceManager(this);
			m_graphics.PreferredBackBufferWidth = 1280;
			m_graphics.PreferredBackBufferHeight = 720;
			Content.RootDirectory = "Content";
		}

		public SpriteBatch getSpriteBatch()
		{
			return m_spriteBatch;
		}

		protected override void Initialize()
		{
			m_camera = new Camera();
			m_currentState = new GameState();
			m_currentState.load();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			m_spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		protected override void UnloadContent()
		{
			
		}

		protected override void Update(GameTime a_gameTime)
		{
			if (m_nextState != null)
			{
				m_currentState = m_nextState;
				m_currentState.load();
				m_nextState = null;
			}

			if (m_currentState != null)
			{
				m_currentState.update(a_gameTime);
			}
			
			base.Update(a_gameTime);
		}

		protected override void Draw(GameTime a_gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, m_camera.getTransformation(m_graphics.GraphicsDevice));
			m_currentState.draw(a_gameTime, m_spriteBatch);
			m_spriteBatch.End();

			base.Draw(a_gameTime);
		}

		internal void setState(GameState a_newState)
		{
			m_nextState = a_newState;
		}
		internal States getState()
		{
			return m_currentState;
		}
    }
}
