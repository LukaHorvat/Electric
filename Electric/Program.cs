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
			//var world = new World();
			//world.AddGrid(new Grid(20, 20));
			//stage.AddChild(world);

			var parent = new ColoredRectangle(0, 0, 100, 100, 1F, 1,1F, 1);
			var child = new ColoredRectangle(50, 50, 100, 100, 1, 1F, 1F, 1);
			stage.AddChild(parent);
			parent.AddChild(child);

			child.TintAlpha = 1F;
			child.TintRed = 1;
			parent.TintGreen = 1;
			parent.TintRed = 1;
			parent.TintAlpha = 1;
			parent.Alpha = 1F;
		}
	}
}
