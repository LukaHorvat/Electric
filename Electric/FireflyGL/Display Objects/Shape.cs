using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FireflyGL
{
	public abstract class Shape : DisplayObject, IDeletable
	{
		public BufferUsageHint DrawingHint = BufferUsageHint.StaticDraw;

		private bool? shapeFlag = null;
		public bool ShapeFlag
		{
			get
			{
				DisplayObject current = this;
				while (!(current is Shape) || (current as Shape).shapeFlag == null)
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
				if (current is Shape && (current as Shape).shapeFlag != null) return (current as Shape).shapeFlag.Value;
				else return false;
			}
			set { shapeFlag = value; }
		}
		public Rectangle AABB;

		protected LinkedList<Polygon> filledPolygons;
		public LinkedList<Polygon> FilledPolygons
		{
			get { return filledPolygons; }
			set { filledPolygons = value; }
		}

		protected LinkedList<Polygon> outlinePolygons;
		public LinkedList<Polygon> OutlinePolygons
		{
			get { return outlinePolygons; }
			set { outlinePolygons = value; }
		}

		protected float[] fillArray;
		protected float[] outlineArray;

		protected Buffer fillBuffer;
		protected Buffer outlineBuffer;

		protected ShaderProgram program;
		public ShaderProgram Program
		{
			get { return program; }
			set { program = value; }
		}

		protected int floatsPerVertex = 0;
		protected bool notSetting = false;

		public Shape(string path)
		{
			filledPolygons = new LinkedList<Polygon>();
			outlinePolygons = new LinkedList<Polygon>();
			fillArray = new float[0];
			outlineArray = new float[0];
			fillBuffer = new Buffer(BufferTarget.ArrayBuffer);
			outlineBuffer = new Buffer(BufferTarget.ArrayBuffer);
			if (path != "") LoadFromFile(path);
		}

		public Shape()
			: this("") { }

		public abstract void SetPolygons();

		public void GenerateBuffers()
		{
			GenerateAABB();
			fillBuffer.SetDataFloat(BufferUsageHint.StreamDraw, fillArray);
			outlineBuffer.SetDataFloat(BufferUsageHint.StreamDraw, outlineArray);
		}

		public void GenerateAABB()
		{
			float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
			for (var node = filledPolygons.First; node != null; node = node.Next)
			{
				for (int i = 0; i < node.Value.Points.Count; ++i)
				{
					var point = node.Value.Points[i];
					if (point.X < minX) minX = point.X;
					if (point.Y < minY) minY = point.Y;
					if (point.X > maxX) maxX = point.X;
					if (point.Y > maxY) maxY = point.Y;
				}
			}

			for (var node = outlinePolygons.First; node != null; node = node.Next)
			{
				for (int i = 0; i < node.Value.Points.Count; ++i)
				{
					var point = node.Value.Points[i];
					if (point.X < minX) minX = point.X;
					if (point.Y < minY) minY = point.Y;
					if (point.X > maxX) maxX = point.X;
					if (point.Y > maxY) maxY = point.Y;
				}
			}
			AABB = new Rectangle(minX, minY, maxX - minX, maxY - minY);
		}

		public void LoadFromFile(string Path)
		{

			using (StreamReader stream = new StreamReader(Path))
			{
				byte[] length = new byte[4];
				length[0] = (byte)stream.Read(); length[1] = (byte)stream.Read(); length[2] = (byte)stream.Read(); length[3] = (byte)stream.Read(); //Read first 4 bytes
				int intLength = BitConverter.ToInt32(length, 0); //Convert them to int
				fillArray = new float[intLength]; //Resize the array to fir vertices
				for (int i = 0; i < intLength * 4; i += 4)
				{ //Use the int as length
					byte[] readBytes = new byte[4];

					readBytes[0] = (byte)stream.Read();
					readBytes[1] = (byte)stream.Read();
					readBytes[2] = (byte)stream.Read();
					readBytes[3] = (byte)stream.Read(); //Read next 4 bytes

					fillArray[i / 4] = BitConverter.ToSingle(readBytes, 0); //Fill the array with the floats
				}

				length[0] = (byte)stream.Read(); length[1] = (byte)stream.Read(); length[2] = (byte)stream.Read(); length[3] = (byte)stream.Read(); //Read first 4 bytes
				intLength = BitConverter.ToInt32(length, 0); //Convert them to int
				outlineArray = new float[intLength]; //Resize the array to fir vertices
				for (int i = 0; i < intLength * 4; i += 4)
				{ //Use the int as length
					byte[] readBytes = new byte[4];

					readBytes[0] = (byte)stream.Read();
					readBytes[1] = (byte)stream.Read();
					readBytes[2] = (byte)stream.Read();
					readBytes[3] = (byte)stream.Read(); //Read next 4 bytes

					outlineArray[i / 4] = BitConverter.ToSingle(readBytes, 0); //Fill the array with the floats
				}
			}

			GenerateBuffers();
		}

		public void SaveToFile(string Path)
		{

			using (StreamWriter stream = new StreamWriter(Path))
			{
				byte[] length;
				length = BitConverter.GetBytes(fillArray.Length);
				for (int i = 0; i < 4; ++i)
				{
					stream.Write((char)length[i]);
				}
				for (int i = 0; i < fillArray.Length; ++i)
				{
					float current = fillArray[i];
					byte[] array = BitConverter.GetBytes(current);
					for (int j = 0; j < 4; ++j)
					{
						stream.Write((char)array[j]);
					}
				}
				length = BitConverter.GetBytes(outlineArray.Length);
				for (int i = 0; i < 4; ++i)
				{
					stream.Write((char)length[i]);
				}
				for (int i = 0; i < outlineArray.Length; ++i)
				{
					float current = outlineArray[i];
					byte[] array = BitConverter.GetBytes(current);
					for (int j = 0; j < 4; ++j)
					{
						stream.Write((char)array[j]);
					}
				}
			}
		}

		protected override DisplayObject CloneSelf()
		{
			Shape clone = null;
			if (this is ColoredShape)
			{
				clone = new ColoredShape();
			}
			else
			{
				clone = new TexturedShape();
			}

			foreach (var poly in filledPolygons) clone.filledPolygons.AddLast(poly);
			foreach (var poly in outlinePolygons) clone.outlinePolygons.AddLast(poly);

			clone.SetPolygons();

			clone.X = X;
			clone.Y = Y;
			clone.Active = active;
			clone.Visible = visible;
			clone.ScaleX = ScaleX;
			clone.ScaleY = ScaleY;
			clone.Alpha = Alpha;
			clone.IgnoresCamera = ignoresCamera;
			if (interactsWithMouse.HasValue)
				clone.InteractsWithMouse = interactsWithMouse.Value;
			clone.Rotation = Rotation;

			return clone;
		}

		public override void RenderSelf()
		{
			base.RenderSelf();

			program.Use();
		}

		public void Delete()
		{
			fillBuffer.Delete();
			outlineBuffer.Delete();
		}

		public override bool IntersectsGlobalPoint(Point point)
		{
			point = GlobalToLocal(point);

			if (!ShapeFlag)
			{
				return AABB.ContainsPoint(point);
			}
			else
			{
				foreach (var poly in filledPolygons)
				{
					if (PolygonContainsPoint(poly, point)) return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Prevents the buffer from updating until EndMassUpdate is called to avoid redundant updates
		/// </summary>
		public void BeginMassUpdate()
		{
			notSetting = true;
		}

		/// <summary>
		/// Ends the mass update and finally updates the buffer
		/// </summary>
		public void EndMassUpdate()
		{
			if (!notSetting) throw new Exception("No BeginMassUpdate call");
			notSetting = false;
			SetPolygons();
		}

		private bool PolygonContainsPoint(Polygon poly, Point point)
		{
			int count = 0;
			for (int i = 0; i < poly.Points.Count; ++i)
			{
				var first = poly.Points[i];
				var second = poly.Points[(i + 1) % poly.Points.Count];
				if (LineToRight(first, second, point)) count++;
			}
			return (count % 2) == 1;
		}

		private bool LineToRight(Vector4 a, Vector4 b, Point x)
		{
			if (a.Y < x.Y && b.Y < x.Y) return false;
			if (a.Y > x.Y && b.Y > x.Y) return false;
			if (a.X < x.X && b.X < x.X) return false;
			if (a.X > x.X && b.X > x.X) return true;
			float deltaX = b.X - a.X;
			float deltaY = b.Y - a.Y;
			float intersectX = deltaX / deltaY * (x.Y - a.Y) + a.X;
			if (intersectX < x.X) return false;
			return true;
		}

		protected override void DrawSelfToShape(Shape target)
		{
			base.DrawSelfToShape(target);

			if (!Visible || !Active) return;
			var filled = GetFilledPolygonsGlobal();
			foreach (var poly in filled) target.FilledPolygons.AddLast(poly);
			var outline = GetOutlinePolygonsGlobal();
			foreach (var poly in outline) target.OutlinePolygons.AddLast(poly);
		}

		protected override Rectangle GetSelfBoundingBox()
		{
			var polys = GetFilledPolygonsGlobal();
			if (polys.Count == 0) return Rectangle.Zero;

			float left = polys.First.Value.Points[0].X;
			float right = polys.First.Value.Points[0].X;
			float top = polys.First.Value.Points[0].Y;
			float bottom = polys.First.Value.Points[0].Y;
			foreach (var poly in polys)
			{
				foreach (var point in poly.Points)
				{
					if (point.X < left) left = point.X;
					if (point.X > right) right = point.X;
					if (point.Y < top) top = point.Y;
					if (point.Y > bottom) bottom = point.Y;
				}
			}

			return new Rectangle(left, top, right - left, bottom - top);
		}

		/// <summary>
		/// Get the polygon data with all transformations applied including the parent's model matrix
		/// </summary>
		/// <returns></returns>
		public LinkedList<Polygon> GetFilledPolygonsGlobal()
		{
			var list = new LinkedList<Polygon>();
			float[] temp;
			bool textured = this is TexturedShape;
			foreach (var poly in filledPolygons)
			{
				int stride = 2 + (textured ? 2 : 4); //2 for x and y + (2 for u and v or 4 for r, g, b and a)
				temp = new float[poly.Points.Count * stride];
				for (int i = 0; i < poly.Points.Count; ++i)
				{
					var transformed = Vector4.Transform(poly.Points[i], modelMatrix);
					temp[i * stride + 0] = transformed.X; //X
					temp[i * stride + 1] = transformed.Y; //Y
					if (textured)
					{
						temp[i * stride + 2] = poly.Texcoords[i].X; //U
						temp[i * stride + 3] = poly.Texcoords[i].Y; //V
					}
					else
					{
						temp[i * stride + 2] = poly.Colors[i].X; //R
						temp[i * stride + 3] = poly.Colors[i].Y; //G
						temp[i * stride + 4] = poly.Colors[i].Z; //B
						temp[i * stride + 5] = poly.Colors[i].W * Alpha; //A
					}
				}
				list.AddLast(new Polygon(textured, temp));
			}
			return list;
		}

		/// <summary>
		/// Get the polygon data with all transformations applied not including the parent's model matrix
		/// </summary>
		/// <returns></returns>
		public LinkedList<Polygon> GetFilledPolygonsLocal()
		{
			var temp = Parent;
			if (Parent != null)
			{
				Parent.RemoveChild(this);
			}
			var list = GetFilledPolygonsGlobal();
			if (temp != null)
			{
				temp.AddChild(this);
			}
			return list;
		}

		/// <summary>
		/// Get the polygon data with all transformations applied including the parent's model matrix
		/// </summary>
		/// <returns></returns>
		public LinkedList<Polygon> GetOutlinePolygonsGlobal()
		{
			var list = new LinkedList<Polygon>();
			float[] temp;
			bool textured = this is TexturedShape;
			foreach (var poly in outlinePolygons)
			{
				int stride = 2 + (textured ? 2 : 4); //2 for x and y + (2 for u and v or 4 for r, g, b and a)
				temp = new float[poly.Points.Count * stride];
				for (int i = 0; i < poly.Points.Count; ++i)
				{
					var transformed = Vector4.Transform(poly.Points[i], modelMatrix);
					temp[i * stride + 0] = transformed.X; //X
					temp[i * stride + 1] = transformed.Y; //Y
					if (textured)
					{
						temp[i * stride + 2] = poly.Texcoords[i].X; //U
						temp[i * stride + 3] = poly.Texcoords[i].Y; //V
					}
					else
					{
						temp[i * stride + 2] = poly.Colors[i].X; //R
						temp[i * stride + 3] = poly.Colors[i].Y; //G
						temp[i * stride + 4] = poly.Colors[i].Z; //B
						temp[i * stride + 5] = poly.Colors[i].W * Alpha; //A
					}
				}
				list.AddLast(new Polygon(textured, temp));
			}
			return list;
		}

		/// <summary>
		/// Get the polygon data with all transformations applied not including the parent's model matrix
		/// </summary>
		/// <returns></returns>
		public LinkedList<Polygon> GetOutlinePolygonsLocal()
		{
			var temp = Parent;
			if (Parent != null)
			{
				Parent.RemoveChild(this);
			}
			var list = GetOutlinePolygonsGlobal();
			if (temp != null)
			{
				temp.AddChild(this);
			}
			return list;
		}
	}
}
