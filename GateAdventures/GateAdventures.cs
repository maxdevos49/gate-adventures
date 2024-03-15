using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GateAdventures.Engine.Scenes;

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
		SpriteBatch spriteBatch = new(GraphicsDevice);

		Services.AddService(spriteBatch);
		Services.AddService(_graphics);
		Services.AddService(GraphicsDevice);
		Services.AddService(_sceneManager);
		Services.AddService(Content);

		base.Initialize();
	}

	protected override void LoadContent()
	{
		_sceneManager.Start(new HelloScene());
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

		base.Draw(gameTime);
	}
}
