using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace FireflyGL
{
	public enum Shapes
	{
		/// <summary>
		/// Center at 0, 0
		/// Circles parameters are: number of points, radius, outline only (-1 false; 1 true), outer r, outer g, outer b, outer a, inner r, inner g, inner b, inner a
		/// </summary>
		Circle,
		/// <summary>
		/// Center at 0, 0
		/// Squares parameters are: side length, outline only (-1 false; 1 true), 4 x 4 floats representing the colors of the corners in top left, top right, bottom right, bottom left order
		/// </summary>
		Square,
		/// <summary>
		/// Center at 0, 0
		/// Squares parameters are: top side length, bottom side, outline only (-1 false; 1 true), 4 x 4 floats representing the colors of the corners in top left, top right, bottom right, bottom left order
		/// </summary>
		Rectangle
	}

	public class Polygon
	{
		List<Vector4> points;
		public List<Vector4> Points
		{
			get { return points; }
			set { points = value; }
		}

		List<Vector4> colors;
		public List<Vector4> Colors
		{
			get { return colors; }
			set { colors = value; }
		}

		List<Vector2> texcoords;
		public List<Vector2> Texcoords
		{
			get { return texcoords; }
			set { texcoords = value; }
		}

		public Polygon(bool Textured, params float[] Coordinates)
		{
			points = new List<Vector4>();
			colors = new List<Vector4>();
			texcoords = new List<Vector2>();

			if (Textured)
			{
				for (int i = 0; i < Coordinates.Length; i += 4)
				{
					points.Add(new Vector4(Coordinates[i], Coordinates[i + 1], 1, 1));
					texcoords.Add(new Vector2(Coordinates[i + 2], Coordinates[i + 3]));
				}
			}
			else
			{
				for (int i = 0; i < Coordinates.Length; i += 6)
				{
					points.Add(new Vector4(Coordinates[i], Coordinates[i + 1], 1, 1));
					colors.Add(new Vector4(Coordinates[i + 2], Coordinates[i + 3], Coordinates[i + 4], Coordinates[i + 5]));
				}
			}
		}

		/// Circles parameters are: number of points, radius, outline only, outer r, outer g, outer b, outer a, inner r, inner g, inner b, inner a

		public Polygon(Shapes shape, params float[] shapeParams)
		{
			points = new List<Vector4>();
			colors = new List<Vector4>();
			texcoords = new List<Vector2>();

			switch (shape)
			{
				case Shapes.Circle:
					int numPoints = (int)shapeParams[0];
					float anglePerPoint = (Geometry.PI * 2) / (numPoints);
					float angle = 0;
					if (shapeParams[2] <= 0)
					{
						points.Add(new Vector4(0, 0, 1, 1));
						colors.Add(new Vector4(shapeParams[7], shapeParams[8], shapeParams[9], shapeParams[10]));
					}
					for (int i = 0; i <= numPoints; ++i)
					{
						var pos = new Vector4();
						pos.X = (float)Math.Cos(angle) * shapeParams[1];
						pos.Y = (float)Math.Sin(angle) * shapeParams[1];
						pos.Z = pos.W = 1;
						points.Add(pos);

						var color = new Vector4(shapeParams[3], shapeParams[4], shapeParams[5], shapeParams[6]);
						colors.Add(color);

						angle += anglePerPoint;
					}
					break;
				case Shapes.Square:
					throw new NotImplementedException();
				case Shapes.Rectangle:
					throw new NotImplementedException();
			}
		}
	}
}
