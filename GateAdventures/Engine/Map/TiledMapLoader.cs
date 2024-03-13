using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;

namespace GateAdventures.Engine.Map;
public static class TiledMapLoader
{
	/**
	 * Function to load all configured maps. Maps must have a path to the JSON, 
	 * and then have tileset sources set with paths to the built tilesets in the Content directory.
	 */
	public static List<TileMapRenderer> LoadAllMaps(ContentManager content, GraphicsDevice graphics)
	{
		List<TileMapRenderer> loadedMaps = new List<TileMapRenderer>
		{
			// Add Maps here
			LoadSingleMap(content, graphics, "assets/maps/exampleMap.json")
		};

		return loadedMaps;
	}

	public static TileMapRenderer LoadSingleMap(ContentManager content, GraphicsDevice graphics, string mapFilePath)
	{
		// Load JSON into TileMap object
		string filePath = Path.Combine(content.RootDirectory, mapFilePath);
		string jsonText = File.ReadAllText(filePath);
		JObject jsonObject = JObject.Parse(jsonText);
		TileMap tileMap = jsonObject.ToObject<TileMap>();

		// Create a TileMapRenderer from our TileMap
		List<Texture2D> atlasList = new List<Texture2D>();
		List<int> firstGids = new List<int>();
		foreach (TileSets tileSet in tileMap.tilesets)
		{
			Texture2D tileAtlas = content.Load<Texture2D>(tileSet.source);
			firstGids.Add(tileSet.firstgid);
			atlasList.Add(tileAtlas);
		}
		TileMapSpriteData mapSpriteData = new TileMapSpriteData(atlasList, firstGids, tileMap, graphics);

		return new TileMapRenderer(mapSpriteData, tileMap);

	}
}
