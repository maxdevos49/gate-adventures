using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateAdventures.Engine.Map;
public class Layers
{
	public List<int> data { get; set; }
	public int height { get; set; }
	public int id { get; set; }
	public string name { get; set; }
	public int opacity { get; set; }
	public string type { get; set; }
	public bool visible { get; set; }
	public int width { get; set; }
	public int x { get; set; }
	public int y { get; set; }
}
