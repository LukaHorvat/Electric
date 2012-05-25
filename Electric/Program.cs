using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireflyGL;

namespace Electric
{
	class Program
	{
		static void Main(string[] args)
		{
			Firefly.Initialize(800, 500, "Electric", OnLoad, true);
		}

		static void OnLoad(Stage stage)
		{
			var world = new World();
			world.AddGrid(new Grid(20, 20));
			stage.AddChild(world);
		}
	}
}
