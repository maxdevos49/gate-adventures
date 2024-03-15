using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateAdventures.Engine.Map;
public class TileMap
{
	public int compressionLevel { get; set; }
	public int height { get; set; }
	public bool infinite { get; set; }
	public List<Layers> layers { get; set; }
	public int nextLayerId { get; set; }
	public int nextObjectId { get; set; }
	public string orientation { get; set; }
	public string renderOrder { get; set; }
	public string tiledversion { get; set; }
	public int tileheight { get; set; }
	public List<TileSets> tilesets { get; set; }
	public int tilewidth { get; set; }
	public string type { get; set; }
	public double version { get; set; }
	public int width { get; set; }
}
