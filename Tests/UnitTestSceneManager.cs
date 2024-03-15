using GateAdventures.Engine.Scenes;
using Microsoft.Xna.Framework;

namespace Tests;

public class UnitTestSceneManager
{

	[Fact(DisplayName = "Starting a new scene correctly calls the Initialize and LoadContent methods")]
	public void SceneManagerStartCallsCorrectMethodsOnScene()
	{
		GameServiceContainer services = new();
		SceneManager manager = new(services);

		Assert.Empty(manager.Scenes);

		TestScene scene1 = new();
		Assert.Equal(0, scene1.InitializeCalls);
		Assert.Equal(0, scene1.LoadContentCalls);
		Assert.Equal(0, scene1.UnloadContentCalls);

		// Starting a scene should call init and load.
		manager.Start(scene1);
		Assert.Equal(1, scene1.InitializeCalls);
		Assert.Equal(1, scene1.LoadContentCalls);
		Assert.Equal(0, scene1.UnloadContentCalls);
		Assert.Single(manager.Scenes);

		TestScene scene2 = new();
		Assert.Equal(0, scene2.InitializeCalls);
		Assert.Equal(0, scene2.LoadContentCalls);
		Assert.Equal(0, scene2.UnloadContentCalls);

		// Starting a new scene with a existing scene running should stop
		// the existing scene and start the new one by calling init and
		// load.
		manager.Start(scene2);
		Assert.Equal(1, scene1.InitializeCalls);
		Assert.Equal(1, scene1.LoadContentCalls);
		Assert.Equal(1, scene1.UnloadContentCalls);

		Assert.Equal(1, scene2.InitializeCalls);
		Assert.Equal(1, scene2.LoadContentCalls);
		Assert.Equal(0, scene2.UnloadContentCalls);

		Assert.Single(manager.Scenes);
	}

	[Fact(DisplayName = "Stopping a scene correctly calls UnloadContent on the scene")]
	public void SceneManagerStopCallsCorrectMethodsOnScene()
	{
		GameServiceContainer services = new();
		SceneManager manager = new(services);

		TestScene scene1 = new();

		manager.Start(scene1);
		Assert.Equal(0, scene1.UnloadContentCalls);
		Assert.Single(manager.Scenes);

		// Stopping a scene correctly calls unloadContent on the scene.
		manager.Stop(scene1);
		Assert.Equal(1, scene1.UnloadContentCalls);
		Assert.Empty(manager.Scenes);
	}

	[Fact(DisplayName = "Calling StartOverlay correctly starts a new scene without stopping an existing scene.")]
	public void TestStartOverlay()
	{
		GameServiceContainer services = new();
		SceneManager manager = new(services);

		TestScene baseScene1 = new();
		TestScene overlayScene1 = new();

		manager.Start(baseScene1);
		Assert.Single(manager.Scenes);

		manager.StartOverlay(overlayScene1);
		Assert.Equal(1, overlayScene1.InitializeCalls);
		Assert.Equal(1, overlayScene1.LoadContentCalls);
		Assert.Equal(0, overlayScene1.UnloadContentCalls);
		Assert.Equal(2, manager.Scenes.Length);

		TestScene baseScene2 = new();
		manager.Start(baseScene2);

		Assert.Equal(1, baseScene1.UnloadContentCalls);
		Assert.Equal(1, overlayScene1.UnloadContentCalls);
		Assert.Single(manager.Scenes);
	}
}

