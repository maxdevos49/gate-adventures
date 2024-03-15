using Microsoft.Xna.Framework;

namespace GateAdventures.Engine.Scenes;

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
