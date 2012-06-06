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
			//var world = new World();
			//world.AddGrid(new Grid(20, 20));
			//stage.AddChild(world);

			rectangle = new ColoredRectangle(-50, -50, 100, 100, 0.7F, 0.1F, 0.3F, 0.9F);
			stage.AddChild(rectangle);

			var console = new DebugConsole();
			stage.AddChild(console);
			console.ExposedReferences["rectangle"] = rectangle;
			console.ExposedReferences["test"] = new Action(Test);

		}

		public static void Test()
		{
			Console.WriteLine("Calling a function from the console");
		}
	}
}
