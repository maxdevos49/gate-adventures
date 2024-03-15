using System;
using System.Collections;
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
	public List<Texture2D> textures { get; }
	public Dictionary<int, AtlasTexture> atlasTextures { get; } = new();
	public int spriteHeight { get; }
	public int spriteWidth { get; }
	public int rows { get; }
	public int columns { get; }

	public TileMapSpriteData(List<Texture2D> textures, List<int> firstGids, TileMap tileMap, GraphicsDevice graphicsDevice)
	{
		_tileMap = tileMap;
		this.textures = textures;
		spriteHeight = tileMap.tileheight;
		spriteWidth = tileMap.tilewidth;

		int tileID = tileMap.tilesets[0].firstgid;
		int atlasTextureKey = 0;

		foreach (Texture2D texture in textures)
		{
			int rows = texture.Width / spriteWidth;
			int columns = texture.Height / spriteHeight;

			for (int y = 0; y < rows; y++)
			{
				for (int x = 0; x < columns; x++)
				{
					Rectangle textureLocation = new Rectangle(x * spriteWidth, y * spriteHeight, spriteWidth, spriteHeight);
					AtlasTexture atlasTexture = new AtlasTexture(textureLocation, atlasTextureKey);
					atlasTextures.Add(tileID, atlasTexture);
					tileID += 1;

					if (firstGids.Contains(tileID))
					{
						atlasTextureKey += 1;
					}
				}
			}
		}
	}
}

public class AtlasTexture
{
	public Rectangle location { get; }
	public int atlasKey { get; }
	public AtlasTexture(Rectangle location, int atlasKey)
	{
		this.location = location;
		this.atlasKey = atlasKey;
	}
}


