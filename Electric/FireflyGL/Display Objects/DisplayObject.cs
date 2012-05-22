using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace FireflyGL
{
	public class DisplayObject : IEntity
	{
		int tweenFramesLeft;
		float tweenDeltaX, tweenDeltaY, tweenDeltaRotation;

		protected bool? interactsWithMouse = null;
		public bool InteractsWithMouse
		{
			get
			{
				DisplayObject current = this;
				while (current.interactsWithMouse == null)
				{
					if (current.Parent != null)
					{
						current = current.Parent;
					}
					else
					{
						break;
					}
				}
				if (current.interactsWithMouse == null) return false;
				else
				{
					return current.interactsWithMouse.Value;
				}
			}
			set { interactsWithMouse = value; }
		}
		public bool IntersectsWithMouse { get; private set; }

		private DisplayObject parent;
		public DisplayObject Parent
		{
			get { return parent; }
			set
			{
				parent = value;
				UpdateMatrices();
			}
		}
		protected Matrix4 scaleMatrix, rotationMatrix, translationMatrix, modelMatrix, parentMatrix; //Camera and window matrices are stored in separate classes because they are common to all display objects
		protected Matrix4 parentlessModelMatrix;

		protected LinkedList<DisplayObject> children = new LinkedList<DisplayObject>();

		float scaleX = 1, scaleY = 1, x, y, rotation, alpha = 1;
		protected bool ignoresCamera;

		public bool IgnoresCamera
		{
			get { return ignoresCamera; }
			set { ignoresCamera = value; UpdateMatrices(); }
		}

		public float ScaleX
		{
			get { return scaleX; }
			set { scaleX = value; UpdateMatrices(); }
		}

		public float ScaleY
		{
			get { return scaleY; }
			set { scaleY = value; UpdateMatrices(); }
		}

		public float Scale
		{
			set { scaleX = value; scaleY = value; UpdateMatrices(); }
		}

		public float X
		{
			get { return x; }
			set { x = value; UpdateMatrices(); }
		}

		public float Y
		{
			get { return y; }
			set { y = value; UpdateMatrices(); }
		}

		public float Rotation
		{
			get { return rotation; }
			set { rotation = value; Geometry.MakeAngle(ref rotation); UpdateMatrices(); }
		}

		public float Alpha
		{
			get { return alpha; }
			set { alpha = value; }
		}

		protected bool visible = true;
		public bool Visible
		{
			get { return visible; }
			set { visible = value; }
		}

		protected bool active = true;
		public bool Active
		{
			get { return active; }
			set { active = value; }
		}

		public DisplayObject()
		{
			UpdateMatrices();
		}

		protected void UpdateMatrices()
		{
			scaleMatrix = Matrix4.Scale(scaleX, scaleY, 1); //0,0 = scaleX, 1,1 = scaleY
			rotationMatrix = Matrix4.CreateRotationZ(rotation);
			translationMatrix = Matrix4.CreateTranslation(x, y, 0); //0,3 = x, 1,3 = y
			if (Parent != null)
			{
				parentMatrix = Parent.modelMatrix;
				if (ignoresCamera == false && Parent.ignoresCamera) ignoresCamera = true;
			}
			else parentMatrix = Matrix4.Identity;

			parentlessModelMatrix = scaleMatrix * rotationMatrix * translationMatrix;
			modelMatrix = parentlessModelMatrix * parentMatrix;
			foreach (var child in children) child.UpdateMatrices();
		}

		public virtual void Render()
		{
			if (!Active || !visible) return;
			RenderSelf();

			for (var node = children.First; node != null; node = node.Next)
			{
				node.Value.Render();
			}
		}

		public virtual void RenderSelf() { }

		public virtual void Update()
		{
			if (!Active) return;
			for (var node = children.First; node != null; node = node.Next)
			{
				node.Value.Update();
			}

			UpdateSelf();
		}

		public virtual void UpdateSelf()
		{
			if (tweenFramesLeft > 0)
			{
				tweenFramesLeft--;
				X += tweenDeltaX;
				Y += tweenDeltaY;
				Rotation += tweenDeltaRotation;
			}
			if (InteractsWithMouse)
			{
				IntersectsWithMouse = children.Any(child => child.IntersectsWithMouse); //If any child intersecrts with mouse, parent intersects with mouse
				if (!IntersectsWithMouse)//If not, check if the mouse intersects with the parent's shape
				{
					if (!IgnoresCamera) IntersectsWithMouse = IntersectsGlobalPoint(new Point(Input.MouseX, Input.MouseY));
					else IntersectsWithMouse = IntersectsGlobalPoint(new Point(Input.RelativeMouseX, Input.RelativeMouseY));
				}
			}
		}

		public void Tween(float endX, float endY, float endRotation, int frames)
		{
			Tween(x, y, endX, endY, rotation, endRotation, frames);
		}

		public void Tween(float startX, float startY, float endX, float endY, float endRotation, int frames)
		{
			Tween(startX, startY, endX, endY, rotation, endRotation, frames);
		}

		public void Tween(float endX, float endY, int frames)
		{
			Tween(x, y, endX, endY, rotation, rotation, frames);
		}

		public void Tween(float startX, float startY, float endX, float endY, float startRotation, float endRotation, int frames)
		{
			if (!Firefly.UpdateListContains(this))
			{
				Firefly.AddToUpdateList(this);
			}
			tweenDeltaX = (endX - startX) / frames;
			tweenDeltaY = (endY - startY) / frames;
			tweenDeltaRotation = Geometry.AngleDifference(startRotation, endRotation) / frames;
			tweenFramesLeft = frames;
		}

		public void StopTween()
		{
			tweenFramesLeft = 0;
		}

		public void AddChild(DisplayObject child)
		{
			children.AddLast(child);
			child.Parent = this;
		}

		public void RemoveChild(DisplayObject child)
		{
			children.Remove(child);
			child.Parent = null;
		}

		public virtual bool IntersectsGlobalPoint(Point point)
		{
			return false;
		}

		public Point GlobalToLocal(Point global)
		{
			if (scaleX == 0 || scaleY == 0) return Point.Max;
			var inverse = modelMatrix;
			inverse.Invert();
			var temp = new Vector4(global.X, global.Y, 1, 1);
			temp = Vector4.Transform(temp, inverse);
			global.X = temp.X;
			global.Y = temp.Y;

			return global;
		}

		public Point LocalToGlobal(Point local)
		{
			var temp = new Vector4(local.X, local.Y, 1, 1);
			temp = Vector4.Transform(temp, modelMatrix);
			local.X = temp.X;
			local.Y = temp.Y;
			return local;
		}

		public Point GetMouseAsLocal()
		{
			Point mouse;
			if (IgnoresCamera)
			{
				mouse = new Point(Input.RelativeMouseX, Input.RelativeMouseY);
			}
			else
			{
				mouse = new Point(Input.MouseX, Input.MouseY);
			}
			return GlobalToLocal(mouse);
		}

		/// <summary>
		/// Inserts the object into another one including the transformations from this object's parent.
		/// </summary>
		/// <param name="target"></param>
		public void DrawToShapeGlobal(Shape target)
		{
			DrawSelfToShape(target);
			foreach (var child in children) DrawToShapeGlobal(target);

			target.SetPolygons();
		}

		/// <summary>
		/// Inserts the object into another one not including the transformations from this object's parent.
		/// </summary>
		/// <param name="target"></param>
		public void DrawToShapeLocal(Shape target)
		{
			var temp = parent;
			if (parent != null)
			{
				parent.RemoveChild(this);
			} //We need to draw this object as if it doesn't have a parent
			DrawToShapeGlobal(target);
			if (temp != null)
			{
				temp.AddChild(this);
			} //Finally we attach it to the parent again
		}

		protected virtual void DrawSelfToShape(Shape target)
		{

		}

		public DisplayObject Clone()
		{
			var clone = CloneSelf();
			foreach (var child in children)
			{
				clone.AddChild(child.Clone());
			}

			return clone;
		}

		protected virtual DisplayObject CloneSelf()
		{
			var clone = new DisplayObject();
			clone.X = x;
			clone.Y = y;
			clone.Active = active;
			clone.Visible = visible;
			clone.ScaleX = scaleX;
			clone.ScaleY = scaleY;
			clone.Alpha = alpha;
			clone.IgnoresCamera = ignoresCamera;
			if (interactsWithMouse.HasValue)
				clone.InteractsWithMouse = interactsWithMouse.Value;
			clone.Rotation = rotation;

			return clone;
		}

		public Rectangle GetGlobalBoundingBox()
		{
			var rect = GetSelfBoundingBox();
			foreach (var child in children) rect = Combine(rect, child.GetGlobalBoundingBox());
			return rect;
		}

		private Rectangle Combine(Rectangle first, Rectangle second)
		{
			if (first == Rectangle.Zero && second == Rectangle.Zero) return Rectangle.Zero;
			if (first == Rectangle.Zero) return second;
			if (second == Rectangle.Zero) return first;

			float left = first.X;
			float top = first.Y;
			float right = first.X + first.Width;
			float bottom = first.Y + first.Height;
			
			if (second.X < left) left = second.X;
			if (second.Y < top) top = second.Y;
			if (second.X + second.Width > right) right = second.X + second.Width;
			if (second.Y + second.Height > bottom) bottom = second.Y + second.Height;

			return new Rectangle(left, top, right - left, bottom - top);
		}

		protected virtual Rectangle GetSelfBoundingBox()
		{
			return Rectangle.Zero;
		}
	}
}
