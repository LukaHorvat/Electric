using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Electric.GridPieces;

namespace Electric.Tools
{
	public class BatteryPlacer : Tool
	{
		public int R, G, B;

		public BatteryPlacer(int r, int g, int b)
		{
			R = r;
			G = g;
			B = b;
			Type = ToolType.PlacerTool;
		}

		public override void Click(float mouseX, float mouseY, int gridX, int gridY, Grid grid, GridPiece piece)
		{
			if (piece == null)
			{
				grid.AddPiece(new Battery(new ChargeColor(R, G, B)), gridX, gridY);
			}
		}
	}
}
