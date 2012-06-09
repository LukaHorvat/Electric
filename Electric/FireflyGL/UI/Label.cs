using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	public enum LabelSizeMethod
	{
		/// <summary>
		/// The label will always have the size given by the word wrap rectangle
		/// </summary>
		FixedSize,
		/// <summary>
		/// The label will have the size of the drawn text that fitted in the word wrap rectangle
		/// </summary>
		SmallestPossible
	}

	public class Label : TexturedRectangle
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
		public LabelSizeMethod SizeMethod;

		public Label(string text, Font font, Brush brush, Rectangle? wordWrap = null, LabelSizeMethod method = LabelSizeMethod.FixedSize)
			: base(Utility.Utility.MakeTextureFromText(text, font, brush, wordWrap: wordWrap, method: method))
		{
			this.text = text;
			this.font = font;
			this.brush = brush;
			WordWrap = wordWrap;
			SizeMethod = method;
		}

		public void UpdateText()
		{
			Texture = Utility.Utility.MakeTextureFromText(text, font, brush, wordWrap: WordWrap, method:SizeMethod);
			Width = Texture.Width;
			Height = Texture.Height;
		}
	}
}
