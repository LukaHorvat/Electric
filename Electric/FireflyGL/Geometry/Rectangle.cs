using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	public struct Rectangle
	{
		public float X, Y, Width, Height;
		public static Rectangle Zero = new Rectangle(0, 0, 0, 0);

		public Rectangle(float x, float y, float width, float height)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}

		public bool ContainsPoint(Point point)
		{
			return point.X >= X && point.Y >= Y && point.X <= X + Width && point.Y <= Y + Height;
		}

		public static bool operator ==(Rectangle first, Rectangle second)
		{
			if (first.X == second.X && first.Y == second.Y && first.Width == second.Width && first.Height == second.Height) return true;
			return false;
		}

		public static bool operator !=(Rectangle first, Rectangle second)
		{
			return !(first == second);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
