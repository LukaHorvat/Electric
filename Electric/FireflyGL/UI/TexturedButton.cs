using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	class TexturedButton : Button
	{
		public TexturedButton(Texture up, Texture down, Texture hover)
		{
			var skin = new ButtonSkin(new TexturedRectangle(up), new TexturedRectangle(down), new TexturedRectangle(hover));

			Initialize(up.Width, up.Height);
		}

		public TexturedButton(Texture button)
		{
			var up = new TexturedRectangle(button);

			var down = new TexturedRectangle(button);
			var shadow = new ColoredRectangle(0, 0, button.Width, button.Height, 0, 0, 0, 0.1F);
			down.AddChild(shadow);

			var hover = new TexturedRectangle(button);
			var light = new ColoredRectangle(0, 0, button.Width, button.Height, 1, 1, 1, 0.1F);
			hover.AddChild(light);

			var skin = new ButtonSkin(up, down, hover);

			Initialize(button.Width, button.Height);
		}

		public TexturedButton(ButtonSkin skin, float width, float height)
			: base(skin, width, height)
		{ }
	}
}
