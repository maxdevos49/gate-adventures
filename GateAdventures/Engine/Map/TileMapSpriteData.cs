using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GateAdventures.Engine.Map;

public class TileMapSpriteData
{
	private TileMap _tileMap;
	public Texture2D atlas { get; }
	public Dictionary<int, Rectangle> atlasTextures { get; } = new();
	public int spriteHeight { get; }
	public int spriteWidth { get; }
	public int rows { get; }
	public int columns { get; }

	public TileMapSpriteData(Texture2D atlasTexture, TileMap tileMap, int textureAtlasWidth, GraphicsDevice graphicsDevice)
	{
		_tileMap = tileMap;
		atlas = atlasTexture;
		spriteHeight = tileMap.tileheight;
		spriteWidth = tileMap.tilewidth;

		rows = textureAtlasWidth / spriteWidth;
		columns = textureAtlasWidth / spriteHeight;
		var tileID = tileMap.tilesets[0].firstgid;

		for (var y = 0; y < rows; y++)
		{
			for (var x = 0; x < columns; x++)
			{
				var textureLocation = new Rectangle(x * spriteWidth, y * spriteHeight, spriteWidth, spriteHeight);
				atlasTextures.Add(tileID, textureLocation);
				tileID++;
			}
		}
	}
}

