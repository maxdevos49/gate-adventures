using System.IO;
using GateAdventures.Engine.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GateAdventures
{
	public class Main : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private TileMapRenderer _tileMapRenderer;

		public Main()
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

			// Load TileMap
			// TODO: Move this to a TiledMapLoader Class
			string filePath = Path.Combine(Content.RootDirectory, "assets/maps/exampleMap.json");
			string jsonText = File.ReadAllText(filePath);
			JObject jsonObject = JObject.Parse(jsonText);
			TileMap tileMap = jsonObject.ToObject<TileMap>();
			Texture2D atlas = this.Content.Load<Texture2D>("assets/maps/tilesets/TX-Tileset-Grass");
			TileMapSpriteData mapSpriteData = new TileMapSpriteData(atlas, tileMap, atlas.Width, _graphics.GraphicsDevice);
			_tileMapRenderer = new TileMapRenderer(mapSpriteData, tileMap);
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

			_tileMapRenderer.Draw(_spriteBatch);

			_spriteBatch.End();
			base.Draw(gameTime);
		}
	}

	public static class Program
	{
		static void Main()
		{
			using (var game = new Main())
				game.Run();
		}
	}
}
