using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Globalization;

namespace FireflyGL
{
	public delegate void OnLoadHandler(Stage root);
	public delegate void FrameHandler();

	public class Firefly
	{
		private static bool kill;

		private static Window window;

		private static VertexShader defaultShapeVertexShader, defaultTextureVertexShader;
		private static FragmentShader defaultShapeFragmentShader, defaultTexturedFragmentShader;

		private static LinkedList<IRenderable> renderList, renderRemoveList;
		private static LinkedList<IUpdatable> updateList, updateRemoveList;

		private static bool displayTimesInTitle;
		public static bool DisplayTimesInTitle
		{
			get { return displayTimesInTitle; }
			set
			{
				if (value == false) window.GameWindow.Title = Title;
				displayTimesInTitle = value;
			}
		}

		public static Window Window
		{
			get
			{
				if (window == null) throw new Exception("Window not open, call the Initialize method first");
				return window;
			}
			set { window = value; }
		}

		public static double UpdateTime { get; set; }
		public static double RenderTime { get; set; }
		public static double TotalTime
		{
			get
			{
				return UpdateTime + RenderTime;
			}
		}
		public static long ElapsedMilliseconds { get; set; }

		public static ShaderProgram DefaultTextureProgram { get; set; }
		public static ShaderProgram DefaultShapeProgram { get; set; }

		public static Matrix4 ProjectionMatrix { get; set; }
		public static Matrix4 WindowMatrix { get; set; }

		public static string Title { get; set; }

		public static event FrameHandler OnRender, OnUpdate;

		public static int LatestAvailableVersion { get; set; }

		private static bool depthTest = false;
		public static bool DepthTest
		{
			get
			{
				return depthTest;
			}
			set
			{
				if (depthTest != value)
				{
					depthTest = value;
					if (depthTest == true)
					{
						GL.Enable(EnableCap.DepthTest);
					}
					else
					{
						GL.Disable(EnableCap.DepthTest);
					}
				}
			}
		}

		public static int ViewportWidth { get; private set; }
		public static int ViewportHeight { get; private set; }

		public static Stage Initialize(int width, int height, string title, OnLoadHandler loadHandler,
									  bool useLatestAvailable = false)
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			window = new Window(width, height, title, useLatestAvailable);

			Title = title;

			if (useLatestAvailable)
			{
				Console.WriteLine(@"Using latest available version: " + LatestAvailableVersion / 10F);
			}
			else
			{
				Console.WriteLine(@"Using OpenGL 2.0, latest version available on the system is: " + LatestAvailableVersion / 10F);
			}

			if (useLatestAvailable && LatestAvailableVersion >= 40)
			{
				defaultTextureVertexShader = new VertexShader();
				defaultTextureVertexShader.LoadFromSource(DefaultShaders.OGL4TexturedVert);
				defaultTexturedFragmentShader = new FragmentShader();
				defaultTexturedFragmentShader.LoadFromSource(DefaultShaders.OGL4TexturedFrag);
				defaultShapeVertexShader = new VertexShader();
				defaultShapeVertexShader.LoadFromSource(DefaultShaders.OGL4ShapeVert);
				defaultShapeFragmentShader = new FragmentShader();
				defaultShapeFragmentShader.LoadFromSource(DefaultShaders.OGL4ShapeFrag);
			}
			else
			{
				defaultTextureVertexShader = new VertexShader();
				defaultTextureVertexShader.LoadFromSource(DefaultShaders.TexturedVert);
				defaultTexturedFragmentShader = new FragmentShader();
				defaultTexturedFragmentShader.LoadFromSource(DefaultShaders.TexturedFrag);
				defaultShapeVertexShader = new VertexShader();
				defaultShapeVertexShader.LoadFromSource(DefaultShaders.ShapeVert);
				defaultShapeFragmentShader = new FragmentShader();
				defaultShapeFragmentShader.LoadFromSource(DefaultShaders.ShapeFrag);
			}

			DefaultShapeProgram = new ShaderProgram(defaultShapeVertexShader, defaultShapeFragmentShader);
			DefaultShapeProgram.Link();
			DefaultShapeProgram.AddUniformLocation("window_matrix");
			DefaultShapeProgram.AddUniformLocation("model_matrix");
			DefaultShapeProgram.AddUniformLocation("projection_matrix");
			DefaultShapeProgram.AddUniformLocation("camera_matrix");
			DefaultShapeProgram.AddUniformLocation("alpha");
			DefaultShapeProgram.AddAttribLocation("vertex_coord");
			DefaultShapeProgram.AddAttribLocation("vertex_color");

			DefaultTextureProgram = new ShaderProgram(defaultTextureVertexShader, defaultTexturedFragmentShader);
			DefaultTextureProgram.Link();
			DefaultTextureProgram.AddUniformLocation("window_matrix");
			DefaultTextureProgram.AddUniformLocation("model_matrix");
			DefaultTextureProgram.AddUniformLocation("projection_matrix");
			DefaultTextureProgram.AddUniformLocation("camera_matrix");
			DefaultTextureProgram.AddUniformLocation("texture");
			DefaultTextureProgram.AddUniformLocation("alpha");
			DefaultTextureProgram.AddAttribLocation("vertex_coord");
			DefaultTextureProgram.AddAttribLocation("vertex_texcoord");

			if (LatestAvailableVersion >= 40 && useLatestAvailable)
			{
				GL.BindFragDataLocation(DefaultShapeProgram.Id, 0, "FragColor");
				GL.BindFragDataLocation(DefaultTextureProgram.Id, 0, "FragColor");
			}

			WindowMatrix = Matrix4.Identity;
			ProjectionMatrix = new Matrix4(new Vector4(2F / window.Width, 0, 0, 0),
										   new Vector4(0, -2F / window.Height, 0, 0),
										   new Vector4(0, 0, 1, 0),
										   new Vector4(0, 0, 0, 1)) * Matrix4.CreateTranslation(-1, 1, 0);

			var newCam = new Camera();
			newCam.Activate();

			renderList = new LinkedList<IRenderable>();
			updateList = new LinkedList<IUpdatable>();
			renderRemoveList = new LinkedList<IRenderable>();
			updateRemoveList = new LinkedList<IUpdatable>();

			Utility.Utility.ProcessOGLErrors();

			CreatePrimitives();

			ChangeViewPort(window.Width, window.Height);
			Camera.CurrentCamera.X = ViewportWidth / 2;
			Camera.CurrentCamera.Y = ViewportHeight / 2;

			var stage = new Stage();
			AddEntity(stage);
			window.GameWindow.Load += ((x, y) => loadHandler(stage));
			window.GameWindow.Load += MainLoop;

			window.GameWindow.Run();

			return stage;
		}

		public static void ChangeViewPort(int width, int height)
		{
			ViewportWidth = width;
			ViewportHeight = height;
			GL.Viewport(0, 0, width, height);
			ProjectionMatrix = new Matrix4(new Vector4(2F / width, 0, 0, 0),
										   new Vector4(0, -2F / height, 0, 0),
										   new Vector4(0, 0, 1, 0),
										   new Vector4(-1, 1, 0, 1));
			Camera.CurrentCamera.UpdateToScreenSize();
		}

		public static void ChangeWindowSize(int width, int height, bool updateViewport)
		{
			if (updateViewport) ChangeViewPort(width, height);
			Window.Width = width;
			Window.Height = height;
		}

		public static void Kill()
		{
			kill = true;
		}

		public static void ForceKill()
		{
			window.GameWindow.Close();
		}

		private static void MainLoop(object sender, EventArgs e)
		{
			Utility.Utility.ProcessOGLErrors();
			double updateLock = 1 / 60.0;
			double renderLock = 1 / 500.0;
			long updateOvertime = 0;
			long ticksSinceLastRender = 0;
			var individualTimer = new Stopwatch();
			var totalTimer = new Stopwatch();

			var globalTime = Stopwatch.StartNew();

			SetupOpenGL();
			Input.Initialize();
			window.GameWindow.ProcessEvents();

			totalTimer.Start();
			while (!kill)
			{
				totalTimer.Reset();
				totalTimer.Start();
				if (ticksSinceLastRender / (double)Stopwatch.Frequency > renderLock)
				{
					ticksSinceLastRender = 0;
					individualTimer.Reset();
					individualTimer.Start();
					Render();
					individualTimer.Stop();

					RenderTime = individualTimer.ElapsedTicks / (double)Stopwatch.Frequency * 1000;
				}
				while (updateOvertime / (double)Stopwatch.Frequency > updateLock)
				{
					updateOvertime -= (long)(updateLock * Stopwatch.Frequency);
					individualTimer.Reset();
					individualTimer.Start();
					Input.Update();
					window.GameWindow.ProcessEvents();
					Update();
					individualTimer.Stop();

					UpdateTime = individualTimer.ElapsedTicks / (double)Stopwatch.Frequency * 1000;
					if (UpdateTime > updateLock * 1000) updateOvertime -= (long)((UpdateTime / 1000 - updateLock) * Stopwatch.Frequency);
				}
				ticksSinceLastRender += totalTimer.ElapsedTicks;
				updateOvertime += totalTimer.ElapsedTicks;
			}
		}

		private static void SetupOpenGL()
		{
			Utility.Utility.ProcessOGLErrors();
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			GL.Enable(EnableCap.Multisample);
			GL.Enable(EnableCap.StencilTest);
			Utility.Utility.ProcessOGLErrors();
		}

		private static void Render()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

			if (OnRender != null)
				OnRender.Invoke();

			foreach (IRenderable renderable in renderRemoveList)
			{
				renderList.Remove(renderable);
			}
			renderRemoveList.Clear();

			for (LinkedListNode<IRenderable> node = renderList.First; node != null; node = node.Next)
			{
				node.Value.Render();
			}

			try
			{
				window.GameWindow.SwapBuffers();
			}
			catch (Exception)
			{
				Kill();
			}
		}

		private static void Update()
		{
			if (DisplayTimesInTitle)
				window.GameWindow.Title =
					"UpdateTime( " + updateList.Count + " ): " + (int)(UpdateTime) +
					" RenderTime( " + renderList.Count + " ): " + (int)(RenderTime) +
					" TotalTime: " + (int)TotalTime;

			if (OnUpdate != null)
				OnUpdate.Invoke();

			foreach (IUpdatable updatable in updateRemoveList)
			{
				updateList.Remove(updatable);
			}
			updateRemoveList.Clear();

			for (LinkedListNode<IUpdatable> node = updateList.First; node != null; node = node.Next)
			{
				node.Value.Update();
			}
		}

		public static void AddToUpdateList(IUpdatable updatable)
		{
			updateList.AddLast(updatable);
		}

		public static void RemoveFromUpdateList(IUpdatable updatable)
		{
			updateRemoveList.AddLast(updatable);
		}

		public static void AddToRenderList(IRenderable renderable)
		{
			renderList.AddLast(renderable);
		}

		public static void RemoveFromRenderList(IRenderable renderable)
		{
			renderRemoveList.AddLast(renderable);
		}

		public static bool RenderListContains(IRenderable renderable)
		{
			return renderList.Contains(renderable);
		}

		public static bool UpdateListContains(IUpdatable updatable)
		{
			return updateList.Contains(updatable);
		}

		public static void AddEntity(object entity)
		{
			if (entity is IUpdatable)
			{
				updateList.AddLast(entity as IUpdatable);
			}
			if (entity is IRenderable)
			{
				renderList.AddLast(entity as IRenderable);
			}
		}

		public static void RemoveEntity(object entity)
		{
			if (entity is IUpdatable)
			{
				updateRemoveList.AddLast(entity as IUpdatable);
			}
			if (entity is IRenderable)
			{
				renderRemoveList.AddLast(entity as IRenderable);
			}
		}

		private static void CreatePrimitives()
		{
			var planeData = new float[]
			{
				-1, -1, 0, 1, 1, 1, 1, -1, -1, 0, 0, 0,
				1, -1, 0, 1, 1, 1, 1, 1, -1, 0, 0, 0,
				1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0,
				-1, 1, 0, 1, 1, 1, 1, -1, 1, 0, 0, 0
			};
			var indices = new uint[]
			{
				0, 1, 2,
				2, 3, 0
			};
			Plane.PlaneData = new MeshData(planeData, indices);
			Plane.PlaneData.GenerateNormals();

			var cubeData = new float[]
			{
				-1, -1, -1, 1, 1, 1, 1, -1, -1, 0, 0, 0,
				1, -1, -1, 1, 1, 1, 1, 1, -1, 0, 0, 0,
				1, 1, -1, 1, 1, 1, 1, 1, 1, 0, 0, 0,
				-1, 1, -1, 1, 1, 1, 1, -1, 1, 0, 0, 0,
				-1, -1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0,
				1, -1, 1, 1, 1, 1, 1, -1, 1, 0, 0, 0,
				1, 1, 1, 1, 1, 1, 1, -1, -1, 0, 0, 0,
				-1, 1, 1, 1, 1, 1, 1, 1, -1, 0, 0, 0
			};
			indices = new uint[]
			{
				0, 1, 2,
				2, 3, 0,

				1, 5, 6,
				6, 2, 1,

				5, 4, 7,
				7, 6, 5,

				4, 0, 3,
				3, 7, 4,

				4, 5, 1,
				1, 0, 4,

				3, 2, 6,
				6, 7, 3
			};
			Cube.CubeData = new MeshData(cubeData, indices);
			Cube.CubeData.GenerateNormals();

			var sphereData = new List<float>();
			float angleDelta = (float)Math.PI * 2 / Sphere.DETAIL_LEVEL;
			for (int j = 0; j <= Sphere.DETAIL_LEVEL / 2; ++j)
			{
				float levelRadius = (float)Math.Sin(j * angleDelta);
				for (int i = 0; i <= Sphere.DETAIL_LEVEL; ++i)
				{
					sphereData.Add((float)Math.Cos(i * angleDelta) * levelRadius);
					sphereData.Add((float)Math.Sin(j * angleDelta + (float)Math.PI / 2));
					sphereData.Add((float)Math.Sin(i * angleDelta) * levelRadius);

					sphereData.Add(1);
					sphereData.Add(1);
					sphereData.Add(1);
					sphereData.Add(1);

					sphereData.Add((float)i / Sphere.DETAIL_LEVEL);
					sphereData.Add((float)j / Sphere.DETAIL_LEVEL * 2);

					sphereData.Add(0);
					sphereData.Add(0);
					sphereData.Add(0);
				}
			}
			var sphereIndices = new List<uint>();
			for (int j = 0; j < Sphere.DETAIL_LEVEL / 2; ++j)
			{
				float levelRadius = (float)Math.Sin(j * angleDelta);
				for (int i = 0; i <= Sphere.DETAIL_LEVEL; ++i)
				{
					uint current = (uint)(j * Sphere.DETAIL_LEVEL + i);
					sphereIndices.Add(current);
					sphereIndices.Add(current + 1);
					sphereIndices.Add(current + Sphere.DETAIL_LEVEL + 1);
					sphereIndices.Add(current + Sphere.DETAIL_LEVEL + 1);
					sphereIndices.Add(current + Sphere.DETAIL_LEVEL);
					sphereIndices.Add(current);
				}
			}
			Sphere.SphereData = new MeshData(sphereData.ToArray(), sphereIndices.ToArray());
			Sphere.SphereData.GenerateNormals();
		}

		public static int GetGLInfoInt(GetPName name)
		{
			int res;
			GL.GetInteger(name, out res);
			return res;
		}
		public static double GetGLInfoDouble(GetPName name)
		{
			double res;
			GL.GetDouble(name, out res);
			return res;
		}
		public static string GetGLInfoString(StringName name)
		{
			return GL.GetString(name);
		}
		public static float GetGLInfoFloat(GetPName name)
		{
			float res;
			GL.GetFloat(name, out res);
			return res;
		}
		public static bool GetGLInfoBool(GetPName name)
		{
			bool res;
			GL.GetBoolean(name, out res);
			return res;
		}
	}
}