/*
 * Luka Horvat
 * Date: 5.9.2011.
 * Time: 20:24
 */
using System;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace FireflyGL
{
	/// <summary>
	/// Description of FrameBuffer.
	/// </summary>
	public class FrameBuffer : IDeletable
	{
		private int id;
		public int Id
		{
			get { return id; }
			private set { id = value; }
		}
		private Texture texture;
		private int depthStencilRenderBuffer;

		private static Stack<FrameBuffer> stack = new Stack<FrameBuffer>();

		public FramebufferErrorCode Status
		{
			get
			{
				Bind();
				var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
				Unbind();
				return status;
			}
		}
		public int DepthStencilRenderBuffer { get; set; }

		public FrameBuffer()
		{
			int temp;
			GL.GenFramebuffers(1, out temp);
			Id = temp;
			GL.GenRenderbuffers(1, out depthStencilRenderBuffer);
		}

		public void Bind()
		{
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, Id);
			GL.Enable(EnableCap.StencilTest);
			stack.Push(this);
		}

		public void Unbind()
		{
			if (stack.Count > 0) stack.Pop();
			if (stack.Count > 0) GL.BindFramebuffer(FramebufferTarget.Framebuffer, stack.Peek().Id);
			else GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}

		public void AttachTexture(Texture texture)
		{
			this.texture = texture;
			Bind();
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture.Id, 0);

			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthStencilRenderBuffer);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, (int)texture.Width, (int)texture.Height);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, depthStencilRenderBuffer);

			DepthStencilRenderBuffer = depthStencilRenderBuffer;
			Unbind();
		}

		public void Delete()
		{
			try
			{
				GL.DeleteFramebuffers(1, ref id);
			}
			catch (AccessViolationException)
			{

			}
		}
	}
}
