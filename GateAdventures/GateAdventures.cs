using System.Collections.Generic;
using System.IO;
using GateAdventures.Engine.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GateAdventures
{
	public class GateAdventures : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private List<TileMapRenderer> _maps;

		public GateAdventures()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			// TileMap width/height. We'll need to change this to 1920x1080
			_graphics.PreferredBackBufferWidth = 960;
			_graphics.PreferredBackBufferHeight = 960;

			_graphics.ApplyChanges();
			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// Load all configured TileMaps and their corresponding TileSets
			_maps = TiledMapLoader.LoadAllMaps(Content, _graphics.GraphicsDevice);
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			_spriteBatch.Begin();

			// TODO: associate a TileMap with a scene and draw it there, rather than looping through and drawing all tilemaps
			foreach (TileMapRenderer mapRenderer in _maps)
			{
				mapRenderer.Draw(_spriteBatch);
			}

			_spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
