﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Electric.GridPieces;

namespace Electric.Tools
{
	public class RelayPlacer : Tool
	{
		public RelayPlacer()
		{
			Type = ToolType.PlacerTool;
		}

		public override void Click(float mouseX, float mouseY, int gridX, int gridY, Grid grid, GridPiece piece)
		{
			if (piece == null)
			{
				grid.AddPiece(new Relay(), gridX, gridY);
			}
		}
	}
}
