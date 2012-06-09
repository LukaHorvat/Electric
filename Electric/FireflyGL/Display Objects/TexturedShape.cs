using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FireflyGL
{

	public class TexturedShape : Shape
	{
		public Texture Texture { get; set; }

		public TexturedShape(string Path)
			: base(Path)
		{
			program = Firefly.DefaultTextureProgram;
		}

		public TexturedShape()
			: base()
		{
			program = Firefly.DefaultTextureProgram;
			floatsPerVertex = 6;
		}
		public override void SetPolygons()
		{
			if (notSetting) return;
			int size = 0;
			foreach (var poly in filledPolygons)
			{
				size += (poly.Points.Count - 2) * 3 * 6;
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
					fillArray[count++] = poly.Texcoords[0].X;
					fillArray[count++] = poly.Texcoords[0].Y;

					fillArray[count++] = poly.Points[i - 1].X;
					fillArray[count++] = poly.Points[i - 1].Y;
					fillArray[count++] = -1;
					fillArray[count++] = 1;
					fillArray[count++] = poly.Texcoords[i - 1].X;
					fillArray[count++] = poly.Texcoords[i - 1].Y;

					fillArray[count++] = poly.Points[i].X;
					fillArray[count++] = poly.Points[i].Y;
					fillArray[count++] = -1;
					fillArray[count++] = 1;
					fillArray[count++] = poly.Texcoords[i].X;
					fillArray[count++] = poly.Texcoords[i].Y;
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
					outlineArray[count++] = poly.Texcoords[i - 1].X;
					outlineArray[count++] = poly.Texcoords[i - 1].Y;

					outlineArray[count++] = poly.Points[i].X;
					outlineArray[count++] = poly.Points[i].Y;
					outlineArray[count++] = -1;
					outlineArray[count++] = 1;
					outlineArray[count++] = poly.Texcoords[i].X;
					outlineArray[count++] = poly.Texcoords[i].Y;
				}
			}

			GenerateBuffers();
		}

		public override void RenderSelf()
		{
			base.RenderSelf();

			((Uniform)program.Locations["texture"]).LoadTexture(Texture);
			((Uniform)program.Locations["window_matrix"]).LoadMatrix(Firefly.WindowMatrix);
			((Uniform)program.Locations["projection_matrix"]).LoadMatrix(Firefly.ProjectionMatrix);
			if (!IgnoresCamera) ((Uniform)program.Locations["camera_matrix"]).LoadMatrix(Camera.CurrentCamera.Matrix);
			else ((Uniform)program.Locations["camera_matrix"]).LoadMatrix(Matrix4.Identity);
			((Uniform)program.Locations["model_matrix"]).LoadMatrix(modelMatrix);
			((Uniform)program.Locations["alpha"]).LoadFloat(realAlpha);
			((Uniform)program.Locations["tintR"]).LoadFloat(realTintR);
			((Uniform)program.Locations["tintG"]).LoadFloat(realTintG);
			((Uniform)program.Locations["tintB"]).LoadFloat(realTintB);
			((Uniform)program.Locations["tintA"]).LoadFloat(realTintA);

			GL.EnableVertexAttribArray(program.Locations["vertex_coord"].Location);
			GL.EnableVertexAttribArray(program.Locations["vertex_texcoord"].Location);

			fillBuffer.Bind();
			((Attribute)program.Locations["vertex_coord"]).AttributePointerFloat(4, 6, 0);
			((Attribute)program.Locations["vertex_texcoord"]).AttributePointerFloat(2, 6, 4);
			GL.DrawArrays(BeginMode.Triangles, 0, fillArray.Length / floatsPerVertex);

			outlineBuffer.Bind();
			((Attribute)program.Locations["vertex_coord"]).AttributePointerFloat(4, 6, 0);
			((Attribute)program.Locations["vertex_texcoord"]).AttributePointerFloat(2, 6, 4);
			GL.DrawArrays(BeginMode.LineStrip, 0, outlineArray.Length / floatsPerVertex);

			GL.DisableVertexAttribArray(program.Locations["vertex_coord"].Location);
			GL.DisableVertexAttribArray(program.Locations["vertex_texcoord"].Location);

			Utility.Utility.ProcessOGLErrors();
		}
	}
}
