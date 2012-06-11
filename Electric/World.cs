using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireflyGL;
using FireflyGL.Utility;
using Electric.GridPieces;
using System.Drawing;
using Electric.Tools;

namespace Electric
{
	public partial class World : DisplayObject
	{
		public DebugConsole Console;

		public bool ShowPerformance
		{
			set
			{
				performanceLabel.Active = value;
			}
		}

		private Grid grid;
		private Label performanceLabel;
		private ColoredButton startButton;
		private Layer gameLayer;
		private Layer uiLayer;
		private Layer consoleLayer;
		private Tool currentTool;
		private bool started = false;

		public World()
		{
			gameLayer = new Layer();
			uiLayer = new Layer();
			consoleLayer = new Layer();
			AddChild(gameLayer);
			AddChild(uiLayer);
			AddChild(consoleLayer);

			SetupUI();
			AddChildren();

			currentTool = new RelayPlacer();

			ExposeObjectsToConsole();
		}

		public void AddGrid(Grid grid)
		{
			this.grid = grid;
			gameLayer.AddChild(grid);

			Console.ExposedReferences["grid"] = grid;
		}

		public override void UpdateSelf()
		{
			base.UpdateSelf();

			if (Utility.GetCountdown("performanceRefresh") == 0)
			{
				UpdatePerformanceLabel();			
				Utility.StartCountdown("performanceRefresh", 20);
			}
			if (Input.MouseButtons[MouseButton.Left] == InputState.Press)
			{
				if (grid.IntersectsWithMouse)
				{
					var location = grid.GetPieceLocationAtCoords(Input.MouseX, Input.MouseY);
					currentTool.Click(Input.MouseX, Input.MouseY, location.Item1, location.Item2, grid, grid.GetPieceAtCoords(Input.MouseX, Input.MouseY));
				}
			}
			if (started)
			{
				grid.Step(1F / 60);
			}
		}

		private void SetupUI()
		{
			performanceLabel = new Label("", new System.Drawing.Font("Courier New", 8), System.Drawing.Brushes.White, new FireflyGL.Rectangle(0, 0, 150, 150));
			performanceLabel.X = 650;
			performanceLabel.IgnoresCamera = true;
			performanceLabel.Active = false;

			startButton = new ColoredButton("Start!", new Font("Consolas", 12), Brushes.White, 0.2F, 0.2F, 0.2F, 1);
			startButton.X = Firefly.Window.Width - startButton.Width - 5;
			startButton.Y = Firefly.Window.Height - startButton.Height - 5;
			startButton.OnClick += delegate(Button button)
			{
				started = true;
			};

			Console = new DebugConsole();
		}

		private void AddChildren()
		{
			uiLayer.AddChild(performanceLabel);
			uiLayer.AddChild(startButton);
			consoleLayer.AddChild(Console);
		}

		private void UpdatePerformanceLabel()
		{
			performanceLabel.Text =
				"Update time:\t" + Firefly.UpdateTime + "\n" +
				"Render time:\t" + Firefly.RenderTime + "\n" +
				"Total time:\t" + Firefly.TotalTime + "\n" +
				"Framerate:\t" + (int)(1000 / Firefly.TotalTime);
		}

		private void ExposeObjectsToConsole()
		{
			Console.ExposedReferences["performance_counter"] = performanceLabel;
			Console.ExposedReferences["world"] = this;
			Console.ExposedReferences["setTool"] = new Action<Tool>(SetTool);
		}

		public void SetTool(Tool tool)
		{
			currentTool = tool;
		}
	}
}
