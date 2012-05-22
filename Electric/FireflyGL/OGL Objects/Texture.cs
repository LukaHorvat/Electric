using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using TexLib;

namespace FireflyGL
{

	public class Texture : IDeletable
	{
		public int Id { get; set; }
		public float Height { get; set; }
		public float Width { get; set; }
		public int Frames { get; set; }
		public bool Animated { get; set; }
		private int pixelBuffer;

		public Texture(float width, float height)
		{
			this.Width = width;
			this.Height = height;

			this.Id = GL.GenTexture();
			Bind();
			var internalFormat = PixelInternalFormat.Rgba;
			var format = PixelFormat.Rgba;
			GL.TexImage2D<byte>(
			  TextureTarget.Texture2D,
			  0,
			  internalFormat,
			  (int)width, (int)height,
			  0,
			  format,
			  PixelType.UnsignedByte,
			  (byte[])null);
			TexUtil.SetParameters();

			this.Animated = false;

			GL.GenBuffers(1, out pixelBuffer);
		}

		public Texture(Bitmap Bmp)
		{
			this.Width = Bmp.Width;
			this.Height = Bmp.Height;

			this.Id = TexUtil.CreateTextureFromBitmap(Bmp);
			Animated = false;

			GL.GenBuffers(1, out pixelBuffer);
		}

		public Texture(string Path)
			: this(new Bitmap(Path))
		{
		}

		public Texture(Bitmap Bmp, int Frames)
		{
			this.Width = Bmp.Width / Frames;
			this.Height = Bmp.Height;

			this.Id = TexUtil.CreateTextureFromBitmap(Bmp);
			this.Frames = Frames;
			Animated = true;

			GL.GenBuffers(1, out pixelBuffer);
		}

		public Texture(string Path, int Frames)
		{
			Bitmap Bmp = new Bitmap(Path);
			this.Width = Bmp.Width / Frames;
			this.Height = Bmp.Height;

			this.Id = TexUtil.CreateTextureFromBitmap(Bmp);
			this.Frames = Frames;
			Animated = true;

			GL.GenBuffers(1, out pixelBuffer);
		}

		public void CopyFrom(Texture source)
		{
			source.Bind();
			GL.BindBuffer(BufferTarget.PixelUnpackBuffer, pixelBuffer);
			GL.BufferData(BufferTarget.PixelUnpackBuffer, new IntPtr((int)source.Width * (int)source.Height * 4), IntPtr.Zero, BufferUsageHint.StaticDraw);
			var pointer = GL.MapBuffer(BufferTarget.PixelUnpackBuffer, BufferAccess.WriteOnly);
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.Byte, pointer);
			GL.UnmapBuffer(BufferTarget.PixelUnpackBuffer);
			Bind();
			Width = source.Width;
			Height = source.Height;
			GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, (int)source.Width, (int)source.Height, PixelFormat.Rgba, PixelType.Byte, IntPtr.Zero);
			GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);
		}

		public void Fill(byte[] data, float width, float height)
		{
			Width = width;
			Height = height;
			Bind();
			var internalFormat = PixelInternalFormat.Rgba;
			var format = PixelFormat.Rgba;
			GL.TexImage2D<byte>(
			  TextureTarget.Texture2D,
			  0,
			  internalFormat,
			  (int)width, (int)height,
			  0,
			  format,
			  PixelType.UnsignedByte,
			  data);
			TexUtil.SetParameters();
		}

		public void Clear()
		{
			Bind();
			var internalFormat = PixelInternalFormat.Rgba;
			var format = PixelFormat.Rgba;
			GL.TexImage2D<byte>(
			  TextureTarget.Texture2D,
			  0,
			  internalFormat,
			  (int)Width, (int)Height,
			  0,
			  format,
			  PixelType.Byte,
			  (byte[])null);
		}

		public void Bind()
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, Id);
		}

		public void Delete()
		{
			try
			{
				GL.DeleteTexture(Id);
			}
			catch (AccessViolationException)
			{

			}
		}
	}
}
