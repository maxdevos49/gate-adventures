using GateAdventures.Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GateAdventures;

public class GateAdventures : Game
{
	private readonly GraphicsDeviceManager _graphics;
	private readonly SceneManager _sceneManager;


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

		Camera camera = new Camera(Services);
		Services.AddService(camera);

		_sceneManager.Start(new TileScene());
		_sceneManager.StartOverlay(new HelloScene());

			
		}

	protected override void Update(GameTime gameTime)
	{
		IScene[] scenes = _sceneManager.Scenes;
		for (int i = 0; i < scenes.Length; i++)
		{
			IScene scene = scenes[i];
			if (i == (scenes.Length - 1))
			{
				scene.HandleInput();
			}

			scene.Update(gameTime);
			Camera camera = Services.GetService<Camera>();
			camera.UpdateCamera(Services);

			base.Update(gameTime);
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

		base.Draw(gameTime);
	}
}
