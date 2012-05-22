using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FireflyGL
{
	class TextField : DisplayObject
	{
		private Layer layer;
		private Label text;
		private ColoredRectangle background;
		private bool focused;
		private ColoredShape arrow;

		public string Text
		{
			get
			{
				return text.Text;
			}
			set
			{
				text.Text = value;
				if (OnChange != null) OnChange.Invoke(this, text.Text);
			}
		}
		public event Action<TextField, string> OnChange;

		public TextField(Font font, Brush textColor, int backgroundColor, float width, float height, Rectangle? wordWrap = null)
		{
			layer = new Layer();
			AddChild(layer);
			float a = (backgroundColor & 0xFF) / 255F;
			float b = ((backgroundColor >> 8) & 0xFF) / 255F;
			float g = ((backgroundColor >> 16) & 0xFF) / 255F;
			float r = ((backgroundColor >> 24) & 0xFF) / 255F;
			background = new ColoredRectangle(0, 0, width, height, r, g, b, a);
			layer.AddChild(background);
			layer.StencilMasks.AddLast(background);

			text = new Label("", font, textColor, wordWrap);
			layer.AddChild(text);

			InteractsWithMouse = true;

			arrow = new ColoredShape();
			arrow.FilledPolygons.AddLast(new Polygon(false,
				0, 5, 1, 1, 1, 1,
				5, 0, 1, 1, 1, 1,
				5, 10, 1, 1, 1, 1));
			arrow.FilledPolygons.AddLast(new Polygon(false,
				5, 3, 1, 1, 1, 1,
				5, 7, 1, 1, 1, 1,
				10, 7, 1, 1, 1, 1,
				10, 3, 1, 1, 1, 1));
			arrow.SetPolygons();
			arrow.Visible = false;
			AddChild(arrow);
		}

		public override void UpdateSelf()
		{
			base.UpdateSelf();
			if (Input.MouseButtons[MouseButton.Left] == InputState.Release)
			{
				if (IntersectsWithMouse && !focused)
				{
					focused = true;
					arrow.Visible = true;
				}
				else if (!IntersectsWithMouse && focused)
				{
					focused = false;
					arrow.Visible = false;
				}
			}
			if (focused)
			{
				arrow.X = text.X + Math.Min(text.Width, background.Width + 3);
				arrow.Y = text.Y + text.Height / 2 - 5;

				char inp;
				while (Input.TypedSymbols.Count > 0)
				{
					inp = Input.TypedSymbols.Pop();
					if (inp != '\b')
					{
						Text += inp;
					}
				}
				if (Input.Keys[Key.BackSpace] == InputState.Down && text.Text.Length != 0 && Utility.Utility.GetCountdown("backspace") == 0)
				{
					Text = text.Text.Substring(0, text.Text.Length - 1);
					Utility.Utility.StartCountdown("backspace", 5);
				}
			}
		}
	}
}
