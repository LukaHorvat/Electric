using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electric
{
	class ChargeColor
	{
		public int Red, Green, Blue;

		public ChargeColor(int red, int green, int blue)
		{
			Red = Math.Min(255, red);
			Green = Math.Min(255, green);
			Blue = Math.Min(255, blue);
		}

		public static ChargeColor operator +(ChargeColor first, ChargeColor second)
		{
			return new ChargeColor(first.Red + second.Red, first.Green + second.Green, first.Blue + second.Blue);
		}
	}
}
