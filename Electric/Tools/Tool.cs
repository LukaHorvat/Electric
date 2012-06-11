using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electric.Tools
{
	public enum ToolType
	{
		PlacerTool,
		RemoverTool
	}

	public abstract class Tool
	{
		public ToolType Type;

		public abstract void Click(float mouseX, float mouseY, int gridX, int gridY, Grid grid, GridPiece piece);
	}
}
