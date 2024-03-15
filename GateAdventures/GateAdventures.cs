using GateAdventures.Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GateAdventures;

public class GateAdventures : Game
{
	private readonly GraphicsDeviceManager _graphics;
	private readonly SceneManager _sceneManager;
	// private List<TileMapRenderer> _maps;

	public GateAdventures()
	{
		_graphics = new GraphicsDeviceManager(this);
		_sceneManager = new SceneManager(Services);

		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	protected override void Initialize()
	{
		_graphics.PreferredBackBufferWidth = 960;
		_graphics.PreferredBackBufferHeight = 960;
		_graphics.ApplyChanges();

		SpriteBatch spriteBatch = new(GraphicsDevice);

		Services.AddService(spriteBatch);
		Services.AddService(_graphics);
		Services.AddService(GraphicsDevice);
		Services.AddService(_sceneManager);
		Services.AddService(Content);

		_sceneManager.Start(new HelloScene());

		base.Initialize();
	}

	protected override void LoadContent()
	{
		// Load all configured TileMaps and their corresponding TileSets
		// _maps = TiledMapLoader.LoadAllMaps(Content, _graphics.GraphicsDevice);
	}

	protected override void Update(GameTime gameTime)
	{
		IScene[] scenes = _sceneManager.Scenes;
		int sceneCount = scenes.Length - 1;

		for (int i = 0; i <= sceneCount; i++)
		{
			IScene scene = scenes[i];
			if (i == sceneCount)
			{
				scene.HandleInput();
			}

			scene.Update(gameTime);
		}

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		IScene[] scenes = _sceneManager.Scenes;
		foreach (IScene scene in scenes)
		{
			scene.Draw(gameTime);
		}

		// // TODO: associate a TileMap with a scene and draw it there, rather than looping through and drawing all tilemaps
		// foreach (TileMapRenderer mapRenderer in _maps)
		// {
		// 	mapRenderer.Draw(_spriteBatch);
		// }

		base.Draw(gameTime);
	}
}




