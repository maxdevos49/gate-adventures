using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GateAdventures.Engine.Map;
public class TileMapRenderer
{
	private TileMapSpriteData _spriteData;
	private TileMap _tileMap;

	public TileMapRenderer(TileMapSpriteData spriteData, TileMap tileMap)
	{
		_spriteData = spriteData;
		_tileMap = tileMap;
	}

	public void Draw(SpriteBatch _spriteBatch)
	{
		foreach (var layer in _tileMap.layers)
		{
			var layerData = layer.data;
			var height = 0;
			var width = 0;

			foreach (var item in layerData)
			{
				var whereOnTheAtlasRectangle = _spriteData.atlasTextures[item];
				var whereOnTheMapRectangle = new Rectangle(width * _tileMap.tilewidth, height * _tileMap.tileheight,
					_tileMap.tilewidth,
					_tileMap.tileheight);
				_spriteBatch.Draw(_spriteData.atlas, whereOnTheMapRectangle, whereOnTheAtlasRectangle, Color.White);

				width++;

				if (width <= _tileMap.width - 1) continue;
				width = 0;
				height++;
			}
		}
	}
}
