using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FireflyGL
{

	public class Attribute : IShaderObject
	{

		private string identifier;
		public string Identifier
		{
			get { return identifier; }
			set { identifier = value; }
		}

		private int location;
		public int Location
		{
			get { return location; }
			set { location = value; }
		}

		public Attribute(string identifier, ShaderProgram program)
		{
			this.identifier = identifier;
			location = GL.GetAttribLocation(program.Id, identifier);
		}

		public void AttributePointerFloat(int Size, int Stride, int Offset)
		{
			GL.VertexAttribPointer(location, Size, VertexAttribPointerType.Float, false, Stride * sizeof(float), Offset * sizeof(float));
		}

		public void AttributePointerInt(int Size, int Stride, int Offset)
		{
			GL.VertexAttribPointer(location, Size, VertexAttribPointerType.Int, false, Stride * sizeof(int), Offset * sizeof(int));
		}
	}
}
