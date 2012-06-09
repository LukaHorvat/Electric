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
	public class World : DisplayObject
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

		public World()
		{
			gameLayer = new Layer();
			uiLayer = new Layer();
			consoleLayer = new Layer();
			AddChild(gameLayer);
			AddChild(uiLayer);
			AddChild(consoleLayer);

			performanceLabel = new Label("", new System.Drawing.Font("Courier New", 8), System.Drawing.Brushes.White, new FireflyGL.Rectangle(0, 0, 150, 150));
			uiLayer.AddChild(performanceLabel);
			performanceLabel.X = 650;
			performanceLabel.IgnoresCamera = true;
			performanceLabel.Active = false;

			Console = new DebugConsole();
			consoleLayer.AddChild(Console);
			Console.ExposedReferences["performance_counter"] = performanceLabel;

			startButton = new ColoredButton("Start!", new Font("Consolas", 12), Brushes.White, 0.2F, 0.2F, 0.2F, 1);
			uiLayer.AddChild(startButton);
			startButton.X = Firefly.Window.Width - startButton.Width - 5;
			startButton.Y = Firefly.Window.Height - startButton.Height - 5;

			currentTool = new RelayPlacer();
		}

		public void AddGrid(Grid grid)
		{
			this.grid = grid;
			gameLayer.AddChild(grid);

			grid.AddPiece(new Battery(new ChargeColor(1, 0, 0)), 5, 5);
		}

		public override void UpdateSelf()
		{
			base.UpdateSelf();

			if (Utility.GetCountdown("performanceRefresh") == 0)
			{
				performanceLabel.Text =
					"Update time:\t" + Firefly.UpdateTime + "\n" +
					"Render time:\t" + Firefly.RenderTime + "\n" +
					"Total time:\t" + Firefly.TotalTime + "\n" +
					"Framerate:\t" + (int)(1000 / Firefly.TotalTime);
				
				Utility.StartCountdown("performanceRefresh", 20);
			}
		}
	}
}
