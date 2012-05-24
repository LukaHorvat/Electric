using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireflyGL;
using FireflyGL.Utility;
using Electric.GridPieces;
using ChargeData = System.Tuple<int, int, Electric.ChargeColor, int, int>;

namespace Electric
{
	class Grid : DisplayObject
	{
		private const int SIZE = 30;
		private const int PADDING = 5;

		private Shape background;
		private List<List<GridPiece>> gridPieces;
		private List<ChargeData> chargeLocations;

		private bool isReset = true;

		private int width { get { return gridPieces.Count; } }
		private int height { get { return gridPieces[0].Count; } }

		public Grid(int width, int height)
		{
			chargeLocations = new List<ChargeData>();
			gridPieces = new List<List<GridPiece>>(width);
			Utility.InitializeList<GridPiece>(gridPieces, width, height);

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

		public void AddPiece(GridPiece piece, int x, int y)
		{
			gridPieces[x][y] = piece;
			Reset();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public void Reset()
		{
			chargeLocations.Clear();

			for (int i = 0; i < width; ++i)
			{
				for (int j = 0; j < height; ++j)
				{
					if (gridPieces[i][j].Type == PieceType.Battery)
					{
						//Add new charges where the batteries are. Each battery also carries color information.
						//The last two integers are the charges last position (so it doesn't bounce back) but 
						//since the battery is where it spawns, we set it's last coordinates to (-1, -1)
						chargeLocations.Add(new ChargeData(i, j, (gridPieces[i][j] as Battery).Color, -1, -1));
					}
				}
			}

			isReset = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="timePart">In seconds</param>
		public void Step(float timePart)
		{
			var newCharges = new List<Tuple<int, int, ChargeColor, int, int>>();

			foreach (var charge in chargeLocations)
			{
				var neighbors = GetNeighbors(charge.Item1, charge.Item2);
				neighbors.RemoveAll(x => x.Item1 == charge.Item4 && x.Item2 == charge.Item5); //Remove the relay this charge originated from to avoid bouncing

				foreach (var neighbor in neighbors)
				{
					//In order: New x, new y, color, old x, old y
					newCharges.Add(new ChargeData(neighbor.Item1, neighbor.Item2, charge.Item3, charge.Item1, charge.Item2));
				}
			}

			newCharges.Sort();
			ChargeData last = null;
			for (int i = 0; i < newCharges.Count; ++i)
			{
				if (last == null || newCharges[i].Item1 != last.Item1 || newCharges[i].Item2 != last.Item2) last = newCharges[i];
				else
				{
					newCharges.Remove(last);
					--i;
					newCharges[i] = new ChargeData(last.Item1, last.Item2, last.Item3 + newCharges[i].Item3, -1, - 1);
					last = newCharges[i];
				}
			}

			chargeLocations = newCharges;

			isReset = false;
		}

		public List<Tuple<int, int, GridPiece>> GetNeighbors(int x, int y)
		{
			var list = new List<Tuple<int, int, GridPiece>>();

			int left = Math.Max(0, x - 1);
			int right = Math.Min(x + 1, width - 1);
			int top = Math.Max(0, y - 1);
			int bottom = Math.Min(y + 1, height - 1);
			for (int i = left; i <= right; ++i)
			{
				for (int j = top; j <= bottom; ++j)
				{
					if (i == x && j == y) continue;
					if (gridPieces[i][j] != null) list.Add(new Tuple<int, int, GridPiece>(i, j, gridPieces[i][j]));
				}
			}

			return list;
		}
	}
}