using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireflyGL;
using FireflyGL.Utility;
using System.Drawing;

namespace Electric
{
	public class Program
	{
		static ColoredRectangle rectangle;

		static void Main(string[] args)
		{
			Firefly.Initialize(800, 500, "Electric", OnLoad, true);
		}

		static void OnLoad(Stage stage)
		{
			DebugConsole.CheckForNonPublicTypes();

			var world = new World();
			world.AddGrid(new Grid(20, 20));
			stage.AddChild(world);

			//rectangle = new ColoredRectangle(-50, -50, 100, 100, 0.7F, 0, 0, 1);
			//stage.AddChild(rectangle);

			//var console = new DebugConsole();
			//console.ExposedReferences["rectangle"] = rectangle;
			//stage.AddChild(console);
		}

		public static void Test()
		{
			Console.WriteLine("Calling a function from the console");
		}
	}
}
