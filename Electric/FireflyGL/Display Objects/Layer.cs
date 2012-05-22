using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace FireflyGL
{
	class Layer : DisplayObject
	{
		public LinkedList<DisplayObject> StencilMasks { get; set; }
		private FrameBuffer frameBuffer;

		public Layer()
		{
            StencilMasks = new LinkedList<DisplayObject>();
			frameBuffer = new FrameBuffer();
		}

		public override void Render()
		{
			if (Visible)
            {
				if (StencilMasks.Count > 0) GL.ClearStencil(0x0);
				else GL.ClearStencil(0x1);
                GL.Clear(ClearBufferMask.StencilBufferBit);

                GL.StencilFunc(StencilFunction.Always, 0x1, 0x1);
                GL.StencilOp(StencilOp.Invert, StencilOp.Invert, StencilOp.Invert);
                GL.ColorMask(false, false, false, false);
                foreach (var mask in StencilMasks) mask.Render();
                GL.ColorMask(true, true, true, true);
                GL.StencilFunc(StencilFunction.Notequal, 0x0, 0x1);
                GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);

				foreach (var item in children) item.Render();
				if (StencilMasks.Count > 0)
				{
					GL.ClearStencil(0x1);
					GL.Clear(ClearBufferMask.StencilBufferBit);
				}
			}
		}

		public Texture RenderToNewTexture(int width, int height)
		{
			var temp = new Texture(width, height);
			RenderToTexture(temp, false);
			return temp;
		}

		public void RenderToTexture(Texture texture, bool clear)
		{
			frameBuffer.AttachTexture(texture);
			frameBuffer.Bind();

			int oldWidth = Firefly.ViewportWidth;
			int oldHeight = Firefly.ViewportHeight;

			Firefly.ChangeViewPort((int)texture.Width, (int)texture.Height);
			if (clear) GL.Clear(ClearBufferMask.ColorBufferBit);
			Render();
			Firefly.ChangeViewPort(oldWidth, oldHeight);

			frameBuffer.Unbind();
		}
	}
}
