using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	class TexturedRectangle : TexturedShape
	{
		private float width;
		public float Width
		{
			get { return width; }
			set {
				width = value;
				UpdateRectangle();
			}
		}
		private float height;
		public float Height
		{
			get { return height; }
			set
			{
				height = value;
				UpdateRectangle();
			}
		}

		public TexturedRectangle(Texture texture)
			: this(0, 0, texture.Width, texture.Height, texture) { }

		public TexturedRectangle(float x, float y, Texture texture)
			: this(x, y, texture.Width, texture.Height, texture) { }

		public TexturedRectangle(Texture texture, float width, float height)
			: this(0, 0, width, height, texture) { }

		public TexturedRectangle(float x, float y, float width, float height, Texture texture)
			: this(x, y, width, height)
		{
			Texture = texture;
		}
		public TexturedRectangle(float x, float y, float width, float height)
			: this(width, height)
		{
			X = x;
			Y = y;
		}
		public TexturedRectangle(float width, float height)
		{
			Initialize(width, height);
		}

		private void Initialize(float width, float height)
		{
			Width = width;
			Height = height;

			UpdateRectangle();
		}

		private void UpdateRectangle()
		{
			FilledPolygons.Clear();
			FilledPolygons.AddLast(new Polygon(true,
				0, 0, 0, 0,
				Width, 0, 1, 0,
				Width, Height, 1, 1,
				0, Height, 0, 1));
			SetPolygons();
		}
	}
}
