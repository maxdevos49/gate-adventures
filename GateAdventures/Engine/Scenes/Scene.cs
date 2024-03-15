using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GateAdventures.Engine.Scenes;

/// <summary>
/// A base for most scene implementations.
/// </summary>
public abstract class Scene : IScene
{
	protected GraphicsDeviceManager _graphicsDeviceManager { get; private set; }
	protected GraphicsDevice _graphicsDevice { get; private set; }
	protected SpriteBatch _spriteBatch { get; private set; }
	protected SceneManager _sceneManager { get; private set; }
	protected ContentManager _content { get; private set; }

	public virtual void Initialize(GameServiceContainer services)
	{
		_sceneManager = services.GetService<SceneManager>() ?? throw new NullReferenceException("\"SceneManager\" service is null");
		_spriteBatch = services.GetService<SpriteBatch>() ?? throw new NullReferenceException("\"SpriteBatch\" service is null");
		_graphicsDeviceManager = services.GetService<GraphicsDeviceManager>() ?? throw new NullReferenceException("\"GraphicsDeviceManager\" service is null");
		_graphicsDevice = services.GetService<GraphicsDevice>() ?? throw new NullReferenceException("\"GraphicsDevice\" service is null");

		ContentManager globalContentManager = services.GetService<ContentManager>() ?? throw new NullReferenceException("\nContentManager\n service is null");
		_content = new ContentManager(services, globalContentManager.RootDirectory);
	}

	public abstract void LoadContent();

	public virtual void UnloadContent()
	{
		_content.Unload();
	}

	public abstract void HandleInput();

	public abstract void Update(GameTime gameTime);

	public abstract void Draw(GameTime gameTime);
}
