using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	public class Comparer : IComparer<Tuple<int, int>>
	{

		public int Compare(Tuple<int, int> x, Tuple<int, int> y)
		{
			return x.Item1 - y.Item1;
		}
	}

	public class TimelineLayer : IEntity
	{

		public int Depth;

		private SortedSet<Tuple<int, int>> beginnings, ends;
		private Dictionary<int, IEntity> entities;
		private int newId = 0;
		private LinkedList<IEntity> currentEntities;
		private int currentFrame = 0;
		private int lastEnd;

		public TimelineLayer(int depth)
		{
			Depth = depth;
			beginnings = new SortedSet<Tuple<int, int>>(new Comparer());
			ends = new SortedSet<Tuple<int, int>>(new Comparer());
			entities = new Dictionary<int, IEntity>();
			currentEntities = new LinkedList<IEntity>();
		}

		public void AddFlag(int start, int length, IEntity target)
		{
			if (start + length > lastEnd) lastEnd = start + length;

			beginnings.Add(new Tuple<int, int>(start, newId));
			ends.Add(new Tuple<int, int>(start + length, newId));
			entities[newId] = target;

			if (start <= currentFrame && start + length >= currentFrame)
			{
				currentEntities.AddLast(target);
			}

			++newId;
		}

		public void JumpTo(int frame)
		{
			if (frame > lastEnd)
			{
				currentEntities.Clear();
				return;
			}
			currentFrame = frame;
			var first = beginnings.GetViewBetween(beginnings.First(), beginnings.Last(x => x.Item1 <= frame));
			var second = ends.GetViewBetween(ends.First(x => x.Item1 >= frame), ends.Last());
			first.RemoveWhere(x => !second.Any(y => y.Item2 == x.Item2));

			currentEntities = new LinkedList<IEntity>(first.Select(x => entities[x.Item2]));
		}

		public void NextFrame()
		{
			foreach (var end in ends.Where(x => x.Item1 == currentFrame)) currentEntities.Remove(entities[end.Item2]); //Remove all entities that end on this frame
			currentFrame++;
			foreach (var start in beginnings.Where(x => x.Item1 == currentFrame)) currentEntities.AddLast(entities[start.Item2]);
		}

		public void Update()
		{
			foreach (var ent in currentEntities) ent.Update();
		}

		public void Render()
		{
			foreach (var ent in currentEntities) ent.Render();
		}
	}

	public class Timeline : IEntity
	{
		public Dictionary<string, int> JumpTable;
		public Dictionary<string, TimelineLayer> Layers;
		public bool Play;

		private List<TimelineLayer> layersByDepth;
		private bool first = true;

		public Timeline()
		{
			JumpTable = new Dictionary<string, int>();
			Layers = new Dictionary<string, TimelineLayer>();
			layersByDepth = new List<TimelineLayer>();
		}

		public void AddLayer(string key, int depth)
		{
			var newLayer = new TimelineLayer(depth);
			Layers[key] = newLayer;

			if (depth >= layersByDepth.Count)
			{
				layersByDepth.Add(newLayer);
				newLayer.Depth = layersByDepth.Count - 1;
			}
			else if (depth < 0)
			{
				layersByDepth.Insert(0, newLayer);
				newLayer.Depth = 0;
			}
			else layersByDepth.Insert(depth, newLayer);
		}

		public void Update()
		{
			if (!first && Play)
			{
				for (int i = layersByDepth.Count - 1; i >= 0; --i)
				{
					layersByDepth[i].NextFrame();
				}
			}
			else first = false;
			for (int i = layersByDepth.Count - 1; i >= 0; --i)
			{
				layersByDepth[i].Update();
			}
		}

		public void Render()
		{
			foreach (var layer in layersByDepth)
			{
				layer.Render();
			}
		}

		public void JumpTo(int frame)
		{
			foreach (var layer in layersByDepth)
			{
				layer.JumpTo(frame);
			}
		}

		public void JumpTo(string name)
		{
			if (JumpTable.ContainsKey(name))
			{
				JumpTo(JumpTable[name]);
			}
		}

		public void NextFrame()
		{
			foreach (var layer in layersByDepth)
			{
				layer.NextFrame();
			}
		}
	}
}
