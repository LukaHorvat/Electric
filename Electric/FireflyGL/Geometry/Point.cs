using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	public struct Point
	{
		private static Point nulledPoint;
		public static Point Null
		{
			get { return nulledPoint; }
		}
		private float x, y;
		public float X
		{
			get { return x; }
			set { x = value; }
		}
		public float Y
		{
			get { return y; }
			set { y = value; }
		}

		private static Point zero = new Point(0, 0);
		public static Point Zero
		{
			get { return zero; }
		}

		private static Point max = new Point(float.MaxValue, float.MaxValue);
		public static Point Max
		{
			get { return max; }
		}

		public Point(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		static Point()
		{
			nulledPoint = new Point();
		}

		public float GetMagnitude()
		{
			return Geometry.Distance(0, 0, X, Y);
		}

		public static Point operator +(Point firstPoint, Point secondPoint)
		{

			return new Point(firstPoint.X + secondPoint.X, firstPoint.Y + secondPoint.Y);
		}

		public static Point operator -(Point firstPoint, Point secondPoint)
		{

			return new Point(firstPoint.X - secondPoint.X, firstPoint.Y - secondPoint.Y);
		}

		public static Point operator -(Point thisPoint)
		{

			return new Point(-thisPoint.X, -thisPoint.Y); ;
		}

		public static Point operator *(Point firstPoint, Point secondPoint)
		{

			return new Point(firstPoint.X * secondPoint.X, firstPoint.Y * secondPoint.Y);
		}

		public static Point operator /(Point firstPoint, Point secondPoint)
		{

			return new Point(firstPoint.X / secondPoint.X, firstPoint.Y / secondPoint.Y);
		}

		public static Point operator *(Point point, float multiplier)
		{

			return new Point(point.X * multiplier, point.Y * multiplier);
		}

		public static Point operator /(Point point, float devider)
		{

			return new Point(point.X / devider, point.Y / devider);
		}

		public static Point operator *(Point point, double multiplier)
		{
			return point * (float)multiplier;
		}

		public static Point operator /(Point point, double devider)
		{
			return point / (float)devider;
		}

		public static bool operator !=(Point point1, Point point2)
		{

			return (point1.X != point2.X || point1.Y != point2.Y);
		}

		public static bool operator ==(Point point1, Point point2)
		{

			return (point1.X == point2.X && point1.Y == point2.Y);
		}

		public override bool Equals(object obj)
		{

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{

			return base.GetHashCode();
		}

		public override string ToString()
		{

			return "X: " + X + " Y: " + Y;
		}

		public bool InsideRectangle(Rectangle rect)
		{
			return rect.ContainsPoint(this);
		}
	}
}
