using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireflyGL;

namespace Electric
{
	class Grid : DisplayObject
	{
		private Shape display;

		public Grid(int width, int height)
		{
			display = new ColoredShape();
			AddChild(display);

			var brush = new ColoredRectangle(0, 0, 20, 20, 0.2F, 0.2F, 0.2F, 1);
			brush.AddOutline(0.7F, 0.7F, 0.7F, 1);

			for (int i = 0; i < width; ++i)
			{
				for (int j = 0; j < height; j++)
				{
					brush.X = i * 25;
					brush.Y = j * 25;
					brush.DrawToShapeGlobal(display);
				}
			}
		}
	}
}