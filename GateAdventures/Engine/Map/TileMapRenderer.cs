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
		foreach (Layers layer in _tileMap.layers)
		{
			List<int> layerData = layer.data;
			int height = 0;
			int width = 0;

			foreach (int item in layerData)
			{
				if (item != 0)
				{
					int whichAtlas = _spriteData.atlasTextures[item].atlasKey;
					Rectangle whereOnTheAtlasRectangle = _spriteData.atlasTextures[item].location;
					Rectangle whereOnTheMapRectangle = new Rectangle(
						width * _tileMap.tilewidth,
						height * _tileMap.tileheight,
						_tileMap.tilewidth,
						_tileMap.tileheight);
					_spriteBatch.Draw(_spriteData.textures[whichAtlas], whereOnTheMapRectangle, whereOnTheAtlasRectangle, Color.White);
				}

				width += 1;

				if (width <= _tileMap.width - 1) continue;
				width = 0;
				height += 1;
			}
		}
	}
}
