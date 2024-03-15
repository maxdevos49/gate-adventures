using System.Collections.Generic;
using GateAdventures.Engine.Map;
using Microsoft.Xna.Framework;

namespace GateAdventures;

public class TileScene : Scene
{
	private List<TileMapRenderer> _maps;
	public override void LoadContent()
	{
		_maps = TiledMapLoader.LoadAllMaps(Content, GraphicsDevice);
	}
	public override void HandleInput() { }

	public override void Update(GameTime gameTime) { }

	public override void Draw(GameTime gameTime)
	{
		SpriteBatch.Begin();
		foreach (TileMapRenderer mapRenderer in _maps)
		{
			mapRenderer.Draw(SpriteBatch);
		}
		SpriteBatch.End();
	}
}
