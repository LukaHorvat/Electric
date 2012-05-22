using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireflyGL;

namespace FireflyGL
{
	class CenteredTexture : TexturedShape
	{
		public CenteredTexture(string path)
			:this(new Texture(path))
		{}
		
		public CenteredTexture(Texture texture)
		{
			Texture = texture;
			FilledPolygons.AddLast(new Polygon(true,
			                                   -Texture.Width / 2, -Texture.Height / 2, 0, 0,
			                                   Texture.Width / 2, -Texture.Height / 2, 1, 0,
			                                   Texture.Width / 2, Texture.Height / 2, 1, 1,
			                                   Texture.Width / 2, Texture.Height / 2, 1, 1,
			                                   -Texture.Width / 2, Texture.Height / 2, 0, 1,
			                                   -Texture.Width / 2, -Texture.Height / 2, 0, 0));
			SetPolygons();
		}

		public void ReverseTexcoords()
		{
			FilledPolygons.Clear();
			FilledPolygons.AddLast(new Polygon(true,
											   -Texture.Width / 2, -Texture.Height / 2, 1, 1,
											   Texture.Width / 2, -Texture.Height / 2, 0, 1,
											   Texture.Width / 2, Texture.Height / 2, 0, 0,
											   -Texture.Width / 2, Texture.Height / 2, 1, 0));
			SetPolygons();
		}
	}
}
