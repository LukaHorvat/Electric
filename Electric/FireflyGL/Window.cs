using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FireflyGL
{
	public class Window : IDisposable
	{
		float major, minor;
		internal GameWindow GameWindow;

		public int Width
		{
			get { return GameWindow.Width; }
			set { GameWindow.Width = value; }
		}
		public int Height
		{
			get { return GameWindow.Height; }
			set { GameWindow.Height = value; }
		}

		internal KeyboardDevice Keyboard
		{
			get { return GameWindow.Keyboard; }
		}

		private System.Drawing.Color clearColor;
		public System.Drawing.Color ClearColor
		{
			get { return clearColor; }
			set
			{
				clearColor = value;
				GL.ClearColor(value);
			}
		}

		public Window(int Width, int Height, string Title, bool UseOGLThree = false)
		{
			var tempWindowForContext = new GameWindow(100, 100) { Visible = false };
			GL.GetFloat(GetPName.MajorVersion, out major);
			GL.GetFloat(GetPName.MinorVersion, out minor);
			tempWindowForContext.Dispose();

			Firefly.LatestAvailableVersion = (int)major * 10 + (int)minor;

			GameWindow = new GameWindow(Width, Height,
				new GraphicsMode(new ColorFormat(8, 8, 8, 8),//RGBA
					8, 8, 8),//Depth, stencil, samples 
				Title,
				GameWindowFlags.Default,//Fullscreen/windowed
				DisplayDevice.Default, //Monitor
				UseOGLThree ? 3 : 2, UseOGLThree ? 0 : 0, UseOGLThree ? GraphicsContextFlags.ForwardCompatible : GraphicsContextFlags.Default);//OGL version
			GameWindow.VSync = VSyncMode.Off;
		}

		public void Dispose()
		{
			GameWindow.Dispose();
		}
	}
}
