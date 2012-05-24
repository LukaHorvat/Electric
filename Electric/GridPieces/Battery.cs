using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electric.GridPieces
{
	class Battery : GridPiece
	{
		public ChargeColor Color;

		public Battery(ChargeColor color)
			: base(PieceType.Battery)
		{
			Color = color;
		}
	}
}
