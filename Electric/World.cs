using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireflyGL;
using FireflyGL.Utility;

namespace Electric
{
	class World : DisplayObject
	{
		public bool ShowPerformance
		{
			set
			{
				performanceLabel.Active = value;
			}
		}

		private Grid grid;
		private Label performanceLabel;
		private Layer gameLayer;
		private Layer uiLayer;

		public World()
		{
			gameLayer = new Layer();
			uiLayer = new Layer();
			AddChild(gameLayer);
			AddChild(uiLayer);

			performanceLabel = new Label("", new System.Drawing.Font("Courier New", 8), System.Drawing.Brushes.White, new Rectangle(0, 0, 150, 150));
			uiLayer.AddChild(performanceLabel);
			performanceLabel.X = 650;
			performanceLabel.IgnoresCamera = true;
		}

		public void AddGrid(Grid grid)
		{
			this.grid = grid;
			gameLayer.AddChild(grid);

			new GridPiece(PieceType.Relay);
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
