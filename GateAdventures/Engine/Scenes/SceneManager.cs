using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GateAdventures.Engine.Scenes;

/// <summary>
/// Manage game scenes.
/// </summary>
public sealed class SceneManager
{
	private readonly List<IScene> _scenes;
	private readonly GameServiceContainer _services;

	/// <summary>
	/// Get a list of active scenes.
	/// </summary>
	public IScene[] Scenes { get => _scenes.ToArray(); }

	/// <summary>
	/// Create a scene manager instance.
	/// </summary>
	/// <param name="services">The game services.</param>
	public SceneManager(GameServiceContainer services)
	{
		_services = services;
		_scenes = new List<IScene>();
	}

	/// <summary>
	/// Stops all existing scenes and starts only the given scene.
	/// </summary>
	/// <param name="scene"></param>
	public void Start(IScene scene)
	{

		for (int i = _scenes.Count - 1; i >= 0; i--)
		{
			Stop(_scenes[i]);
		}

		scene.Initialize(_services);

		scene.LoadContent();

		_scenes.Add(scene);
	}

	/// <summary>
	/// Starts a scene on top of an existing scene.
	/// </summary>
	/// <param name="scene">An instance of a scene to start.</param>
	public void StartOverlay(IScene scene)
	{
		scene.Initialize(_services);

		scene.LoadContent();

		_scenes.Add(scene);
	}

	/// <summary>
	/// Stops a scene.
	/// </summary>
	/// <param name="scene">The scene to stop.</param>
	public void Stop(IScene scene)
	{
		scene.UnloadContent();

		_scenes.Remove(scene);
	}
}

/// <summary>
/// A game scene interface.
/// </summary>
public interface IScene
{
	/// <summary>
	/// Use to setup and validate internal state for a scene.
	/// </summary>
	public void Initialize(GameServiceContainer services);

	/// <summary>
	/// Load content needed for the scene.
	/// </summary>
	public void LoadContent();

	/// <summary>
	/// Unload content needed for this scene
	/// </summary>
	public void UnloadContent();

	/// <summary>
	/// Handle user input. Only called if the scene is the top most scene.
	/// </summary>
	public void HandleInput();

	/// <summary>
	/// Update the scene.
	/// </summary>
	/// <param name="gameTime">The game time.</param>
	public void Update(GameTime gameTime);

	/// <summary>
	/// Draws the scene.
	/// </summary>
	/// <param name="gameTime">The game time.</param>
	public void Draw(GameTime gameTime);
}
