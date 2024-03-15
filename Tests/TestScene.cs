using GateAdventures.Engine.Scenes;
using Microsoft.Xna.Framework;

namespace Tests;

/// <summary>
/// Testing class for the scene manager.
/// </summary>
public class TestScene : IScene
{
	public int InitializeCalls { get; private set; }
	public void Initialize(GameServiceContainer services)
	{
		InitializeCalls++;
	}

	public int LoadContentCalls { get; private set; }
	public void LoadContent()
	{
		LoadContentCalls++;
	}

	public int UnloadContentCalls { get; private set; }
	public void UnloadContent()
	{
		UnloadContentCalls++;
	}

	public void HandleInput()
	{
		// Not tested
	}

	public void Update(GameTime gameTime)
	{
		// Not tested
	}

	public void Draw(GameTime gameTime)
	{
		// Not tested
	}
}
