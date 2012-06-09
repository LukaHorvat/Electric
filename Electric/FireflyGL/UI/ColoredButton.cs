using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FireflyGL
{
	public class ColoredButton : Button
	{
		private Label label;

		public ColoredButton(string text, Font font, Brush textColor, float r, float g, float b, float a)
		{
			label = new Label(text, font, textColor);

			float width = label.Width + 20, height = label.Height + 10;

			var up =
				new ColoredRectangle(0, 0, width, height / 2, r, g, b, a);
			var upBottom =
				new ColoredRectangle(0, height / 2, width, height / 2, Math.Max(r - 0.1F, 0), Math.Max(g - 0.1F, 0), Math.Max(b - 0.1F, 0), a);
			up.AddChild(upBottom);

			var down =
				new ColoredRectangle(0, 0, width, height / 2, Math.Max(r - 0.05F, 0), Math.Max(g - 0.05F, 0), Math.Max(b - 0.05F, 0), a);
			var downBottom =
				new ColoredRectangle(0, height / 2, width, height / 2, Math.Min(r + 0.05F, 1), Math.Min(g + 0.05F, 1), Math.Min(b + 0.05F, 1), a);
			down.AddChild(downBottom);

			var hover =
				new ColoredRectangle(0, 0, width, height / 2, Math.Min(r + 0.1F, 1), Math.Min(g + 0.1F, 1), Math.Min(b + 0.1F, 1), a);
			var hoverBottom =
				new ColoredRectangle(0, height / 2, width, height / 2, r, g, b, a);
			hover.AddChild(hoverBottom);

			Skin = new ButtonSkin(up, down, hover);
			Initialize(width, height);
			AddChild(label);

			Resize();
		}

		public ColoredButton(string text, Font font, Brush textColor, ButtonSkin skin)
		{
			label = new Label(text, font, textColor);

			float width = label.Width + 20, height = label.Height + 10;

			Skin = skin.Clone();
			Initialize(width, height);
			AddChild(label);

			Resize();
		}

		public ColoredButton(string text, Font font, Brush textColor, ButtonSkin skin, float width, float height)
		{
			label = new Label(text, font, textColor);

			Skin = skin.Clone();
			Initialize(width, height);
			AddChild(label);

			Resize();
		}

        protected override void Resize()
        {
			Skin.Hover.ScaleX = Width / firstWidth;
			Skin.Hover.ScaleY = Height / firstHeight;
			Skin.Down.ScaleX = Width / firstWidth;
			Skin.Down.ScaleY = Height / firstHeight;
			Skin.Up.ScaleX = Width / firstWidth;
			Skin.Up.ScaleY = Height / firstHeight;

			label.X = (int)((Width - label.Width) / 2);
			label.Y = 5;
        }
	}
}
