using System.Collections.Generic;
using GateAdventures.Engine.Map;
using GateAdventures.Engine.Scenes;
using Microsoft.Xna.Framework;

namespace GateAdventures;

public class TileScene : Scene
{
	private List<TileMapRenderer> _maps;
	public override void LoadContent()
	{
		_maps = TiledMapLoader.LoadAllMaps(_content, _graphicsDevice);
	}
	public override void HandleInput() { }

	public override void Update(GameTime gameTime) { }

	public override void Draw(GameTime gameTime)
	{
		_spriteBatch.Begin();
		foreach (TileMapRenderer mapRenderer in _maps)
		{
			mapRenderer.Draw(_spriteBatch);
		}
		_spriteBatch.End();
	}
}
