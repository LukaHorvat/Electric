using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireflyGL;

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
		private int performanceRefreshCountdown = 0;

		public World()
		{
			performanceLabel = new Label("", new System.Drawing.Font("Courier New", 8), System.Drawing.Brushes.White, new Rectangle(0, 0, 300, 300));
			AddChild(performanceLabel);
			performanceLabel.X = 500;
			performanceLabel.IgnoresCamera = true;
		}

		public void AddGrid(Grid grid)
		{
			this.grid = grid;
			AddChild(grid);
		}

		public override void UpdateSelf()
		{
			base.UpdateSelf();

			if (performanceRefreshCountdown == 0)
			{
				performanceLabel.Text =
					"Update time: " + Firefly.UpdateTime + "\n" +
					"Render time: " + Firefly.RenderTime + "\n" +
					"Total time: " + Firefly.TotalTime + "\n" +
					"Framerate (60 cap): " + (int)(1000 / Firefly.TotalTime);
				performanceRefreshCountdown = 20;
			}
			else
			{
				performanceRefreshCountdown--;
			}
		}
	}
}
