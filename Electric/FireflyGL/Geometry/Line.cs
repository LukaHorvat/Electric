using System;
using System.Collections.Generic;
using System.Text;

namespace FireflyGL
{
	public struct Line
	{
		public Point Start { get; set; }
		public Point End { get; set; }
		public Line(Point aStart, Point aEnd)
			:this()
		{
			Start = aStart;
			End = aEnd;
		}

		public Point AtPercent(float percent)
		{
			return (End - Start) / 100 * percent + Start;
		}

		public Point AtDistance(float Distance)
		{
			float distance = Geometry.Distance(Start, End);
			distance = Distance / distance;
			distance *= 100;
			return AtPercent(distance);
		}
	}
}
