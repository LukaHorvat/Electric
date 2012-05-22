using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireflyGL;
using FireflyGL.Utility;

namespace Electric
{
	class Grid : DisplayObject
	{
		private const int SIZE = 30;
		private const int PADDING = 5;

		private Shape background;
		private List<List<GridPiece>> gridPieces;

		public Grid(int width, int height)
		{
			gridPieces = new List<List<GridPiece>>(width);
			for (int i = 0; i < width; ++i)
			{
				gridPieces.Add(new List<GridPiece>(height));
				for (int j = 0; j < height; ++j)
				{
					gridPieces[i].Add(null);
				}
			}

			background = new ColoredShape();
			AddChild(background);

			var brush = new ColoredRectangle(0, 0, SIZE, SIZE, 0.2F, 0.2F, 0.2F, 1);
			brush.AddOutline(0.7F, 0.7F, 0.7F, 1);

			for (int i = 0; i < width; ++i)
			{
				for (int j = 0; j < height; j++)
				{
					brush.X = i * (SIZE + PADDING);
					brush.Y = j * (SIZE + PADDING);
					brush.DrawToShapeGlobal(background);
				}
			}
		}
	}
}