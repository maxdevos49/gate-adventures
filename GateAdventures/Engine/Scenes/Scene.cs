using System;
using GateAdventures.Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GateAdventures;

/// <summary>
/// A base for most scene implementations.
/// </summary>
public abstract class Scene : IScene
{
	protected GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
	protected GraphicsDevice GraphicsDevice { get; private set; }
	protected SpriteBatch SpriteBatch { get; private set; }
	protected SceneManager SceneManager { get; private set; }
	protected ContentManager Content { get; private set; }

	public virtual void Initialize(GameServiceContainer services)
	{
		SceneManager = services.GetService<SceneManager>() ?? throw new NullReferenceException("\"SceneManager\" service is null");
		SpriteBatch = services.GetService<SpriteBatch>() ?? throw new NullReferenceException("\"SpriteBatch\" service is null");
		GraphicsDeviceManager = services.GetService<GraphicsDeviceManager>() ?? throw new NullReferenceException("\"GraphicsDeviceManager\" service is null");
		GraphicsDevice = services.GetService<GraphicsDevice>() ?? throw new NullReferenceException("\"GraphicsDevice\" service is null");

		ContentManager globalContentManager = services.GetService<ContentManager>() ?? throw new NullReferenceException("\nContentManager\n service is null");
		Content = new ContentManager(services, globalContentManager.RootDirectory);
	}

	public abstract void LoadContent();

	public virtual void UnloadContent()
	{
		Content.Unload();
	}

	public abstract void HandleInput();

	public abstract void Update(GameTime gameTime);

	public abstract void Draw(GameTime gameTime);
}
