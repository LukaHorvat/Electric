using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{

	/// <summary>
	/// Contrains static for geometric calculations
	/// </summary>
	class Geometry
	{

		//1/50 of a PI
		public const double TP = Math.PI / 50;
		public const float PI = (float)Math.PI;

		//Returns the intersection point between 2 lines or null if they don't intersect
		public static Point Intersection(Point A, Point B, Point E, Point F)
		{

			Point ip;
			float a1;
			float a2;
			float b1;
			float b2;
			float c1;
			float c2;

			a1 = B.Y - A.Y;
			b1 = A.X - B.X;
			c1 = B.X * A.Y - A.X * B.Y;
			a2 = F.Y - E.Y;
			b2 = E.X - F.X;
			c2 = F.X * E.Y - E.X * F.Y;

			float denom = a1 * b2 - a2 * b1;

			if (denom == 0)
			{
				return Point.Null;
			}

			ip = new Point();
			ip.X = (b1 * c2 - b2 * c1) / denom;
			ip.Y = (a2 * c1 - a1 * c2) / denom;

			if (Math.Pow(ip.X - B.X, 2) + Math.Pow(ip.Y - B.Y, 2) > Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2))
			{
				return Point.Null;
			}

			if (Math.Pow(ip.X - A.X, 2) + Math.Pow(ip.Y - A.Y, 2) > Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2))
			{
				return Point.Null;
			}

			if (Math.Pow(ip.X - F.X, 2) + Math.Pow(ip.Y - F.Y, 2) > Math.Pow(E.X - F.X, 2) + Math.Pow(E.Y - F.Y, 2))
			{
				return Point.Null;
			}

			if (Math.Pow(ip.X - E.X, 2) + Math.Pow(ip.Y - E.Y, 2) > Math.Pow(E.X - F.X, 2) + Math.Pow(E.Y - F.Y, 2))
			{
				return Point.Null;
			}

			return ip;
		}

		//Calculates the distance between 2 pairs of coordinates
		public static float Distance(float x1, float y1, float x2, float y2)
		{

			return (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
		}

		//Calculates the distance between 2 points
		public static float Distance(Point ao1, Point ao2)
		{

			return (float)Math.Sqrt((ao1.X - ao2.X) * (ao1.X - ao2.X) + (ao1.Y - ao2.Y) * (ao1.Y - ao2.Y));
		}

		//Calculates c from c = sqrt( a * a + b * b )
		public static float Pyth(float a, float b)
		{

			return (float)Math.Sqrt(a * a + b * b);
		}

		//Point as parameter
		public static float Pyth(Point point)
		{

			return (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
		}

		//Calculates the angle between two sets of points
		public static float Angle(Point A, Point B)
		{

			return (float)Math.Atan2(B.Y - A.Y, B.X - A.X);
		}

		//Converts radians to degrees
		public static float RadiansToDegrees(float arg)
		{

			return arg * 57.29577951308232F;
		}

		//Converts degrees to radians
		public static float DegreesToRadians(float arg)
		{

			return arg / (180F / 3.141592653589793F);
		}

		//Checks if the first value is between the second and third var
		public static bool isBetween(float what, float aBound1, float aBound2)
		{

			return (what >= aBound1 && what <= aBound2) || (what <= aBound1 && what >= aBound2);
		}

		//Binds the first value to a set range
		public static void Bind(ref float what, float from, float to)
		{

			what = what < from ? from : (what > to ? to : what);
		}

		//Binds an angle to a set range
		public static void BindAngle(ref float what, float from, float to)
		{

			what = AngLess(what, from) ? from : (AngMore(what, to) ? to : what);
		}

		public static float AngleDifference(float alpha, float beta)
		{
			MakeAngle(ref alpha);
			MakeAngle(ref beta);

			float difference = beta - alpha;
			while (difference < -PI) difference += 2 * PI;
			while (difference > PI) difference -= 2 * PI;
			return difference;
		}

		//Checks if the first angle is to the left of the second angle
		public static bool AngLess(float a, float b)
		{

			MakeAngle(ref a);
			MakeAngle(ref b);
			if (a == b)
			{

				return false;
			}
			return !AngMore(a, b);
		}

		//Checks if the first angle is to the right of the second angle
		public static bool AngMore(float a, float b)
		{

			MakeAngle(ref a);
			MakeAngle(ref b);
			if (a == b)
			{

				return false;
			}
			if (a < Math.PI && b > Math.PI)
			{

				b -= Geometry.PI;
				return a < b;

			}
			else if (a > Math.PI && b > Math.PI)
			{

				return a > b;
			}
			else if (a < Math.PI && b < Math.PI)
			{

				return a > b;
			}
			else if (a > Math.PI && b < Math.PI)
			{

				b += Geometry.PI;
				return a < b;
			}

			return false;
		}

		//Takes a number and converts it to an angle from -PI to PI
		public static void MakeAngle(ref float aang)
		{
			if (float.IsNaN(aang))
			{
				aang = 0;
				return;
			}
			int temp = (int)(aang / (PI * 2));
			aang = aang - (temp * PI);
			if (aang > PI) aang = -(2 * PI - aang);
			else if (aang < -PI) aang = 2 * PI + aang;
		}

		//Calculates x and y coordinates of the point at set angle and distance from the start point
		public static Point Polar(float Angle, float Amount)
		{

			Point returnVector = new Point((float)Math.Cos(Angle) * Amount, (float)Math.Sin(Angle) * Amount);
			return returnVector;
		}

		//Takes the second Point and moves it to the set length from the first Point keeping the angle
		public static void Leash(Point Anchor, ref Point Mover, float aLen)
		{

			Line line = new Line(Anchor, Mover);
			Mover = line.AtDistance(aLen);
		}

		//Decreases the absolute value of the number by the set amount
		public static void ToZero(ref float Target, float Amount)
		{

			Target = Target > 0 ? Target - Amount : Target < 0 ? Target + Amount : Target;
		}

		//Decreases the absolute X and Y values of the point by the set amount
		public static void ToZero(ref Point Target, float Amount)
		{

			Target.X = Target.X > 0 ? Target.X - Amount : Target.X < 0 ? Target.X + Amount : Target.X;
			Target.Y = Target.Y > 0 ? Target.Y - Amount : Target.Y < 0 ? Target.Y + Amount : Target.Y;
		}

		//Sets the number to zero if it's close enough
		public static void RoundToZero(ref float Target, float Distance)
		{
			Target = Math.Abs(Target) <= Distance ? 0 : Target;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="point"></param>
		/// <param name="anchor"></param>
		/// <param name="aAngle">Add to the angle</param>
		public static void RotateAroundPoint(ref Point point, Point anchor, float aAngle)
		{

			float angle = Angle(anchor, point);
			float distance = Distance(point, anchor);
			angle += aAngle;
			point.X = anchor.X + (float)Math.Cos(angle) * distance;
			point.Y = anchor.Y + (float)Math.Sin(angle) * distance;
		}

		public static Point VectorAtAngle(Point Vector, float Angle)
		{

			return Geometry.Polar(Angle, (float)Math.Cos(Geometry.Angle(Point.Zero, Vector) - Angle) * Geometry.Pyth(Vector));
		}

		public static Point MoveAtAngle(Point Origin, float Angle, float Amount)
		{

			return new Point((float)Math.Cos(Angle) * Amount, (float)Math.Sin(Angle) * Amount) + Origin;
		}

		public static float PointLineDistane(Point point, Point line1, Point line2)
		{

			if (line1.X == line2.X) return Math.Abs(point.X - line1.X);
			float a = Math.Abs(line1.Y - line2.Y) / Math.Abs(line1.X - line2.X);
			float b = line1.Y - line1.X * a;

			return (Math.Abs(a * point.X - point.Y + b) / (float)Math.Sqrt(a * a + 1));
		}

		public static bool PointInRectangle(Point point, Point topleft, Point topright)
		{

			if (point.X >= topleft.X && point.X <= topright.X && point.Y >= topleft.Y && point.Y <= topright.Y) return true;
			return false;
		}

		public static Point LineRectangleIntersection(Point A, Point B, Point topleft, Point bottomright)
		{

			Point intersect = Intersection(A, B, bottomright, new Point(bottomright.X, topleft.Y));
			if (intersect != default(Point))
			{

				return intersect;
			}
			intersect = Intersection(A, B, topleft, new Point(bottomright.X, topleft.Y));
			if (intersect != default(Point))
			{

				return intersect;
			}
			intersect = Intersection(A, B, topleft, new Point(topleft.X, bottomright.Y));
			if (intersect != default(Point))
			{

				return intersect;
			}
			intersect = intersect = Intersection(A, B, bottomright, new Point(topleft.X, bottomright.Y));
			if (intersect != default(Point))
			{

				return intersect;
			}
			return default(Point);
		}
	}
}
