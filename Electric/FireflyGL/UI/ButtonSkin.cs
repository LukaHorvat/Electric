using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	class ButtonSkin
	{
		public DisplayObject Hover;
		public DisplayObject Up;
		public DisplayObject Down;

		public ButtonSkin(DisplayObject up, DisplayObject down, DisplayObject hover)
		{
			Up = up.Clone();
			Down = down.Clone();
			Hover = hover.Clone();
		}

		public ButtonSkin Clone()
		{
			return new ButtonSkin(Up, Down, Hover);
		}
	}
}
