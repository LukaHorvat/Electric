using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	public class Frame : IEntity
	{
		public LinkedList<IRenderable> RenderList { get; set; }
		public LinkedList<IUpdatable> UpdateList { get; set; }

		public bool Active { get; set; }

		public Frame()
		{
			RenderList = new LinkedList<IRenderable>();
			UpdateList = new LinkedList<IUpdatable>();
		}

		public void AddEntity(IEntity item)
		{
			RenderList.AddLast(item);
			UpdateList.AddLast(item);
		}

		public void RemoveEntity(IEntity item)
		{
			if (RenderList.Contains(item)) RenderList.Remove(item);
			if (UpdateList.Contains(item)) UpdateList.Remove(item);
		}

		public virtual void Render()
		{
			if (Active)
			{
				RenderSelf();
				foreach (var item in RenderList) item.Render();
			}
		}

		public virtual void RenderSelf()
		{

		}

		public virtual void Update()
		{
			if (Active)
			{
				UpdateSelf();
				foreach (var item in UpdateList) item.Update();
			}
		}

		public virtual void UpdateSelf()
		{

		}
	}
}
