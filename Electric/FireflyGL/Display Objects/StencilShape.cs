using System;
using OpenTK.Graphics.OpenGL;

namespace FireflyGL
{
	public class StencilShape : ColoredShape
	{
		public StencilShape()
		{
		}
		
		public override void Render()
		{
			GL.StencilFunc(StencilFunction.Always, 0x1, 0x1);
			GL.StencilOp(StencilOp.Invert, StencilOp.Invert, StencilOp.Invert);
			GL.ColorMask(false, false, false, false);
			base.Render();
			GL.ColorMask(true, true, true, true);
			GL.StencilFunc(StencilFunction.Notequal, 0x0, 0x1);
			GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
		}
	}
}
