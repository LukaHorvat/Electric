using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireflyGL;
using System.Text.RegularExpressions;

namespace Electric
{
	enum PieceType
	{
		Battery,
		Relay,
		UpwardRelay,
		DownwardRelay
	}

	abstract class GridPiece : TexturedRectangle
	{
		public PieceType Type;

		public GridPiece(PieceType type)
			: base(textures[type])
		{
			Type = type;
		}

		private static Dictionary<PieceType, Texture> textures;

		static GridPiece()
		{
			textures = new Dictionary<PieceType, Texture>();

			var strings = Enum.GetNames(typeof(PieceType));
			foreach (var str in strings)
			{
				var filename = new StringBuilder("graphics/");
				for (int i = 0; i < str.Length; ++i)
				{
					if (str[i] >= 'A' && str[i] <= 'Z' && i != 0)
					{
						filename.Append("_");
					}
					filename.Append(str.Substring(i, 1).ToLower());
				}
				filename.Append(".png");
				textures[(PieceType)Enum.Parse(typeof(PieceType), str)] = new Texture(filename.ToString());
			}
		}
	}
}
