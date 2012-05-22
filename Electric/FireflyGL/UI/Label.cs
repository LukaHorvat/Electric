using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	class Label : TexturedRectangle
	{
		private string text;
		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				UpdateText();
			}
		}
		private Font font;
		public Font Font
		{
			get { return font; }
			set
			{
				font = value;
				UpdateText();
			}
		}

		private Brush brush;
		public Brush Brush
		{
			get { return brush; }
			set
			{
				brush = value;
				UpdateText();
			}
		}

		public Rectangle? WordWrap;

		public Label(string text, Font font, Brush brush, Rectangle? wordWrap = null)
			: base(Utility.Utility.MakeTextureFromText(text, font, brush, wordWrap: wordWrap))
		{
			this.text = text;
			this.font = font;
			this.brush = brush;
			WordWrap = wordWrap;
		}

		public void UpdateText()
		{
			Texture = Utility.Utility.MakeTextureFromText(text, font, brush, wordWrap: WordWrap);
			Width = Texture.Width;
			Height = Texture.Height;
		}
	}
}
