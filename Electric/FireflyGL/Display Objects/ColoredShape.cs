using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FireflyGL
{

	public class ColoredShape : Shape
	{
		public ColoredShape(string Path)
			: base(Path)
		{
			program = Firefly.DefaultShapeProgram;
		}

		public ColoredShape()
			: base()
		{
			program = Firefly.DefaultShapeProgram;
			floatsPerVertex = 8;
		}

		public override void SetPolygons()
		{
			if (notSetting) return;
			int size = 0;
			foreach (var poly in filledPolygons)
			{
				size += (poly.Points.Count - 2) * 3 * 8;
			}
			fillArray = new float[size];
			int count = 0;
			foreach (Polygon poly in filledPolygons)
			{
				for (int i = 2; i < poly.Points.Count; ++i)
				{
					fillArray[count++] = poly.Points[0].X;
					fillArray[count++] = poly.Points[0].Y;
					fillArray[count++] = -1;
					fillArray[count++] = 1;
					fillArray[count++] = poly.Colors[0].X;
					fillArray[count++] = poly.Colors[0].Y;
					fillArray[count++] = poly.Colors[0].Z;
					fillArray[count++] = poly.Colors[0].W;

					fillArray[count++] = poly.Points[i - 1].X;
					fillArray[count++] = poly.Points[i - 1].Y;
					fillArray[count++] = -1;
					fillArray[count++] = 1;
					fillArray[count++] = poly.Colors[i - 1].X;
					fillArray[count++] = poly.Colors[i - 1].Y;
					fillArray[count++] = poly.Colors[i - 1].Z;
					fillArray[count++] = poly.Colors[i - 1].W;

					fillArray[count++] = poly.Points[i].X;
					fillArray[count++] = poly.Points[i].Y;
					fillArray[count++] = -1;
					fillArray[count++] = 1;
					fillArray[count++] = poly.Colors[i].X;
					fillArray[count++] = poly.Colors[i].Y;
					fillArray[count++] = poly.Colors[i].Z;
					fillArray[count++] = poly.Colors[i].W;
				}
			}

			size = 0;
			count = 0;
			foreach (var poly in outlinePolygons)
			{
				size += (poly.Points.Count - 1) * 2 * 8;
			}
			outlineArray = new float[size];
			foreach (Polygon poly in outlinePolygons)
			{
				if (poly.Points.Count <= 1) continue;
				for (int i = 1; i < poly.Points.Count; ++i)
				{
					outlineArray[count++] = poly.Points[i - 1].X;
					outlineArray[count++] = poly.Points[i - 1].Y;
					outlineArray[count++] = -1;
					outlineArray[count++] = 1;
					outlineArray[count++] = poly.Colors[i - 1].X;
					outlineArray[count++] = poly.Colors[i - 1].Y;
					outlineArray[count++] = poly.Colors[i - 1].Z;
					outlineArray[count++] = poly.Colors[i - 1].W;

					outlineArray[count++] = poly.Points[i].X;
					outlineArray[count++] = poly.Points[i].Y;
					outlineArray[count++] = -1;
					outlineArray[count++] = 1;
					outlineArray[count++] = poly.Colors[i].X;
					outlineArray[count++] = poly.Colors[i].Y;
					outlineArray[count++] = poly.Colors[i].Z;
					outlineArray[count++] = poly.Colors[i].W;
				}
			}

			GenerateBuffers();
		}

		public void AddOutline(float r, float g, float b, float a)
		{
			var list = new List<float>();
			foreach (var filledPoly in filledPolygons)
			{
				list.Clear();
				foreach (var point in filledPoly.Points)
				{
					list.AppendMany(point.X, point.Y, r, g, b, a);
				}
				list.AppendMany(filledPoly.Points[0].X, filledPoly.Points[0].Y, r, g, b, a);
				outlinePolygons.AddLast(new Polygon(false,
					list.ToArray()));
			}
			SetPolygons();
		}

		public override void RenderSelf()
		{
			base.RenderSelf();

			loadUniforms();
			enableVertexAttribArrays();
			if (fillArray.Length > 0) drawFillBuffer();
			if (outlineArray.Length > 0) drawOutlineBuffer();
			diableVertexArrays();
			Utility.Utility.ProcessOGLErrors();
		}

		void loadUniforms()
		{
			(program.Locations["window_matrix"] as Uniform).LoadMatrix(Firefly.WindowMatrix);
			(program.Locations["projection_matrix"] as Uniform).LoadMatrix(Firefly.ProjectionMatrix);
			if (!IgnoresCamera) (program.Locations["camera_matrix"] as Uniform).LoadMatrix(Camera.CurrentCamera.Matrix);
			else (program.Locations["camera_matrix"] as Uniform).LoadMatrix(Matrix4.Identity);
			(program.Locations["model_matrix"] as Uniform).LoadMatrix(modelMatrix);
			(program.Locations["alpha"] as Uniform).LoadFloat(Alpha);
		}

		void enableVertexAttribArrays()
		{
			GL.EnableVertexAttribArray(program.Locations["vertex_coord"].Location);
			GL.EnableVertexAttribArray(program.Locations["vertex_color"].Location);
		}

		void drawFillBuffer()
		{
			fillBuffer.Bind();
			(program.Locations["vertex_coord"] as Attribute).AttributePointerFloat(4, 8, 0);
			(program.Locations["vertex_color"] as Attribute).AttributePointerFloat(4, 8, 4);
			GL.DrawArrays(BeginMode.Triangles, 0, fillArray.Length / floatsPerVertex);
		}

		void drawOutlineBuffer()
		{
			outlineBuffer.Bind();
			(program.Locations["vertex_coord"] as Attribute).AttributePointerFloat(4, 8, 0);
			(program.Locations["vertex_color"] as Attribute).AttributePointerFloat(4, 8, 4);
			GL.DrawArrays(BeginMode.Lines, 0, outlineArray.Length / floatsPerVertex);
		}

		void diableVertexArrays()
		{
			GL.DisableVertexAttribArray(program.Locations["vertex_coord"].Location);
			GL.DisableVertexAttribArray(program.Locations["vertex_color"].Location);
		}
	}
}
