using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FireflyGL
{
	public class TextField : DisplayObject
	{
		private Layer layer;
		private Label text;
		private ColoredRectangle background;
		private bool focused;
		private ColoredShape arrow;

		private int cursorPosition = 0;
		public int CursorPosition
		{
			get { return cursorPosition; }
			set
			{
				cursorPosition = value;
				cursorPosition = Math.Min(text.Text.Length, Math.Max(0, cursorPosition));

				Region[] characters;
				StringFormat format = new StringFormat { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };
				RectangleF bounds;

				CharacterRange[] ranges = new CharacterRange[] { new CharacterRange((cursorPosition == 0 ? 1 : cursorPosition) - 1, 1) };
				format.SetMeasurableCharacterRanges(ranges);
				if (text.WordWrap.HasValue)
				{
					characters = Utility.Utility.MeasureGfx.MeasureCharacterRanges(text.Text == "" ? "." : text.Text, text.Font, new RectangleF(0, 0, text.WordWrap.Value.Width, text.WordWrap.Value.Height), format);
				}
				else
				{
					characters = Utility.Utility.MeasureGfx.MeasureCharacterRanges(text.Text == "" ? "." : text.Text, text.Font, new RectangleF(0, 0, float.MaxValue, float.MaxValue), format);
				}
				bounds = characters[0].GetBounds(Utility.Utility.MeasureGfx);

				arrow.X = text.X + Math.Min(bounds.X + (cursorPosition == 0 ? 0 : bounds.Width), background.Width + 3);
				arrow.Y = text.Y + bounds.Y + bounds.Height / 2 - 5;
			}
		}

		public string Text
		{
			get
			{
				return text.Text;
			}
			set
			{
				lastText = text.Text;
				text.Text = value;
				if (OnChange != null) OnChange.Invoke(this, value);
			}
		}
		public event Action<TextField, string> OnChange;
		public List<char> IllegalChars;

		private string lastText = "";

		public TextField(Font font, Brush textColor, uint backgroundColor, float width, float height, Rectangle? wordWrap = null)
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

			text = new Label("", font, textColor, wordWrap, LabelSizeMethod.SmallestPossible);
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

			CursorPosition = 0;
			OnChange += OnChangeHandler;

			IllegalChars = new List<char>();
			IllegalChars.Add('\b');
		}

		public void Focus()
		{
			focused = true;
			arrow.Visible = true;
		}

		public void Unfocus()
		{
			focused = false;
			arrow.Visible = false;
		}

		private void OnChangeHandler(TextField field, string str)
		{
			if (lastText == "" && str != "") CursorPosition = str.Length;
			else CursorPosition = CursorPosition; //To check if the cursor is out of bounds
		}

		public override void UpdateSelf()
		{
			base.UpdateSelf();
			if (Input.MouseButtons[MouseButton.Left] == InputState.Release)
			{
				if (IntersectsWithMouse && !focused)
				{
					Focus();
				}
				else if (!IntersectsWithMouse && focused)
				{
					Unfocus();
				}
			}
			if (focused)
			{
				char inp;
				if (Input.Keys[Key.Left] == InputState.Down && CursorPosition > 0 && Utility.Utility.GetCountdown("leftArrow_firefly") == 0)
				{
					CursorPosition--;
					Utility.Utility.StartCountdown("leftArrow_firefly", 5);
				}
				if (Input.Keys[Key.Right] == InputState.Down && CursorPosition < text.Text.Length && Utility.Utility.GetCountdown("rightArrow_firefly") == 0)
				{
					CursorPosition++;
					Utility.Utility.StartCountdown("rightArrow_firefly", 5);
				}

				string tail = text.Text.Substring(CursorPosition, text.Text.Length - CursorPosition);

				while (Input.TypedSymbols.Count > 0)
				{
					inp = Input.TypedSymbols.Pop();
					if (!IllegalChars.Contains(inp))
					{
						Text = text.Text.Substring(0, CursorPosition) + inp + tail;
						CursorPosition++;
					}
				}
				if (Input.Keys[Key.BackSpace] == InputState.Down && CursorPosition != 0 && Utility.Utility.GetCountdown("backspace") == 0)
				{
					CursorPosition--;
					Text = text.Text.Substring(0, CursorPosition) + tail;
					Utility.Utility.StartCountdown("backspace", 5);
				}
			}
		}
	}
}
