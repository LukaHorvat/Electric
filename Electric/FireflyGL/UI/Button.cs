using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FireflyGL
{
    enum ButtonState
    {
        Up, Down, Hover
    }

    abstract class Button : DisplayObject
    {
		public ButtonSkin Skin;
        public ButtonState State { get; set; }
        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                size = new Point(Width, Height);
                Resize();
            }
        }
        public float Height
        {
            get { return height; }
            set
            {
                height = value;
                size = new Point(Width, Height);
                Resize();
            }
        }
        public bool Clicked { get; set; }
		public event Action<Button> OnClick;

        protected float firstWidth, firstHeight;
        protected float lastWidth, lastHeight, width, height;
        protected Point size;

		public Button(ButtonSkin skin, float width, float height)
		{
			Skin = skin.Clone();

			Initialize(width, height);
		}

        public Button(DisplayObject up, DisplayObject down, DisplayObject hover, float width, float height)
        {
			Skin = new ButtonSkin(up, down, hover);

            Initialize(width, height);
        }

        public Button()
        {
        }

        protected void Initialize(float width, float height)
        {
            InteractsWithMouse = true;

            State = ButtonState.Up;
            lastWidth = firstWidth = this.width = width;
            lastHeight = firstHeight = this.height = height;
            size = new Point(Width, Height);

            AddChild(Skin.Up);
			AddChild(Skin.Down);
			AddChild(Skin.Hover);

            Resize();
        }

        public override void UpdateSelf()
        {
            base.UpdateSelf();
            Clicked = false;

			if (Input.MouseButtons[MouseButton.Left] == InputState.Press && IntersectsWithMouse)
			{
				State = ButtonState.Down;
			}
			if (Input.MouseButtons[MouseButton.Left] == InputState.Up && IntersectsWithMouse)
			{
				State = ButtonState.Hover;
			}
			if (!IntersectsWithMouse && State == ButtonState.Hover)
			{
				State = ButtonState.Up;
			}
			if (Input.MouseButtons[MouseButton.Left] == InputState.Release && State == ButtonState.Down)
			{
				State = ButtonState.Up;
				Clicked = true;
				Input.MouseHandled = true;
				if (OnClick != null) OnClick.Invoke(this);
			}
        }

        protected virtual void Resize()
        {
            ScaleX = Width / firstWidth;
            ScaleY = Height / firstHeight;
        }

        public override void RenderSelf()
        {
            base.RenderSelf();

			Skin.Up.Visible = false;
			Skin.Down.Visible = false;
			Skin.Hover.Visible = false;
			if (State == ButtonState.Up) Skin.Up.Visible = true;
			if (State == ButtonState.Down) Skin.Down.Visible = true;
			if (State == ButtonState.Hover) Skin.Hover.Visible = true;
        }
    }
}
