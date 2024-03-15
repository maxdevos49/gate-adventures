using System.Collections.Generic;
using GateAdventures.Engine;
using GateAdventures.Engine.Map;
using GateAdventures.Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GateAdventures;

public class TileScene : Scene
{
	private List<TileMapRenderer> _maps;

	public override void LoadContent()
	{
		_maps = TiledMapLoader.LoadAllMaps(_content, _graphicsDevice);
	}
	public override void HandleInput() { }

	public override void Update(GameTime gameTime)
	{
		_camera.UpdateCamera(_graphicsDeviceManager);
	}

	public override void Draw(GameTime gameTime)
	{
		_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwise, null, _camera.Transform);

		foreach (TileMapRenderer mapRenderer in _maps)
		{
			mapRenderer.Draw(_spriteBatch);
		}
		_spriteBatch.End();
	}
}
