using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireflyGL;

namespace FireflyExtensions
{
	enum TransitionType
	{
		LERP,
		Cos
	}

	class TransitioningImage : DisplayObject
	{
		public TexturedRectangle First { get; set; }
		public TexturedRectangle Second { get; set; }
		public TransitionType Type { get; set; }
		public bool AnimateFirstImageAlpha;
		public int Frames { get; set; }
		private int currentFrame;
		private int direction = -1;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <param name="type">Linear interpolation or cosine</param>
		/// <param name="frames">Number of frames until the picture completely changes to the second one, then goes back to the first one</param>
		public TransitioningImage(Texture first, Texture second, TransitionType type, int frames, bool animateFirstAlpha = true)
		{
			First = new TexturedRectangle(first);
			Second = new TexturedRectangle(second);
			Type = type;
			Frames = frames;
			AnimateFirstImageAlpha = animateFirstAlpha;

			AddChild(First);
			AddChild(Second);
		}

		public override void UpdateSelf()
		{
			base.UpdateSelf();
			if (currentFrame == Frames || currentFrame == 0) direction *= -1;
			currentFrame += direction;

			switch (Type)
			{
				case TransitionType.LERP:
					int remainder = Frames - currentFrame;
					if (AnimateFirstImageAlpha) First.Alpha = remainder / (float)Frames;
					Second.Alpha = currentFrame / (float)Frames;
					break;
				case TransitionType.Cos:
					float angle = currentFrame / (float)Frames * Geometry.PI;
					float percent = (float)Math.Cos(angle) * 0.5F + 0.5F;
					float inverse = 1 - percent;

					if (AnimateFirstImageAlpha) First.Alpha = percent;
					Second.Alpha = inverse;
					break;
			}
		}
	}
}
