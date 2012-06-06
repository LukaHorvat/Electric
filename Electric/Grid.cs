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
	public class Grid : DisplayObject
	{
		private const int SIZE = 30;
		private const int PADDING = 5;

		private Shape background;
		private List<List<GridPiece>> gridPieces;
		private List<ChargeData> chargeLocations;
		private List<ColoredShape> chargeMarkers;

		private ColoredShape originalChargeMarker;

		private bool isReset = true;

		private int width { get { return gridPieces.Count; } }
		private int height { get { return gridPieces[0].Count; } }

		private Layer piecesLayer;
		private Layer markerLayer;
		private Layer effectsLayer;

		public Grid(int width, int height)
		{
			originalChargeMarker = new ColoredShape();
			originalChargeMarker.FilledPolygons.AppendMany(
				new Polygon(false,
				0, 0, 1, 1, 1, 0.7F,
				SIZE, 0, 1, 1, 1, 0.7F,
				SIZE - 5, 5, 1, 1, 1, 0,
				5, 5, 1, 1, 1, 0),

				new Polygon(false,
				SIZE, 0, 1, 1, 1, 0.7F,
				SIZE, SIZE, 1, 1, 1, 0.7F,
				SIZE - 5, SIZE - 5, 1, 1, 1, 0,
				SIZE - 5, 5, 1, 1, 1, 0),

				new Polygon(false,
				SIZE, SIZE, 1, 1, 1, 0.7F,
				0, SIZE, 1, 1, 1, 0.7F,
				5, SIZE - 5, 1, 1, 1, 0,
				SIZE - 5, SIZE - 5, 1, 1, 1, 0),

				new Polygon(false,
				0, SIZE, 1, 1, 1, 0.7F,
				0, 0, 1, 1, 1, 0.7F,
				5, 5, 1, 1, 1, 0,
				5, SIZE - 5, 1, 1, 1, 0));

			originalChargeMarker.SetPolygons();

			piecesLayer = new Layer();
			markerLayer = new Layer();
			effectsLayer = new Layer();

			chargeMarkers = new List<ColoredShape>();
			chargeLocations = new List<ChargeData>();
			gridPieces = new List<List<GridPiece>>(width);
			Utility.InitializeList<GridPiece>(gridPieces, width, height);

			background = new ColoredShape();

			var brush = new ColoredRectangle(0, 0, SIZE, SIZE, 0.2F, 0.2F, 0.2F, 1);
			brush.AddOutline(0.7F, 0.7F, 0.7F, 1);

			background.BeginMassUpdate();
			for (int i = 0; i < width; ++i)
			{
				for (int j = 0; j < height; j++)
				{
					brush.X = i * (SIZE + PADDING);
					brush.Y = j * (SIZE + PADDING);
					brush.DrawToShapeGlobal(background);
				}
			}
			background.EndMassUpdate();

			AddChild(background);
			AddChild(piecesLayer);
			AddChild(markerLayer);
			AddChild(effectsLayer);
		}

		public void AddPiece(GridPiece piece, int x, int y)
		{
			gridPieces[x][y] = piece;
			piece.X = x * (SIZE + PADDING);
			piece.Y = y * (SIZE + PADDING);
			piecesLayer.AddChild(piece);
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
					if (gridPieces[i][j] == null) continue;
					if (gridPieces[i][j].Type == PieceType.Battery)
					{
						//Add new charges where the batteries are. Each battery also carries color information.
						//The last two integers are the charges last position (so it doesn't bounce back) but 
						//since the battery is where it spawns, we set it's last coordinates to (-1, -1)
						chargeLocations.Add(new ChargeData(i, j, (gridPieces[i][j] as Battery).Color, -1, -1));
					}
				}
			}
			UpdateMarkers();

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
					newCharges[i] = new ChargeData(last.Item1, last.Item2, last.Item3 + newCharges[i].Item3, -1, -1);
					last = newCharges[i];
				}
			}

			chargeLocations = newCharges;

			UpdateMarkers();

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

		private void UpdateMarkers()
		{
			for (int i = 0; i < chargeLocations.Count; ++i)
			{
				if (chargeMarkers.Count == i)
				{
					chargeMarkers.Add((ColoredShape)originalChargeMarker.Clone());
					markerLayer.AddChild(chargeMarkers[i]);
				}
				var marker = chargeMarkers[i];
				marker.Active = true;
				marker.X = chargeLocations[i].Item1 * (SIZE + PADDING);
				marker.Y = chargeLocations[i].Item2 * (SIZE + PADDING);
				marker.TintAlpha = 1;
				marker.TintRed = chargeLocations[i].Item3.Red;
				marker.TintGreen = chargeLocations[i].Item3.Green;
				marker.TintBlue = chargeLocations[i].Item3.Blue;
			}
			if (chargeMarkers.Count > chargeLocations.Count)
			{
				for (int i = chargeLocations.Count; i < chargeMarkers.Count; ++i)
				{
					chargeMarkers[i].Active = false; ;
				}
			}
		}
	}
}