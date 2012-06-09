using System;
using System.Collections.Generic;
using FireflyGL;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FireflyGL
{
	public class Mesh : IRenderable
	{
		public MeshData Data { get; set; }

		private VertexShader vertexShader;
		private FragmentShader fragmentShader;
		private ShaderProgram shaderProgram;
		private Matrix4 rotation;
		private Matrix4 shaderRotationMatrix;
		private Matrix4 translation;
		private Matrix4 scale;
		private Matrix4 projection;
		private Matrix4 window;
		private Matrix4 shaderModelMatrix;
		public Matrix4 ModelMatrix { get; set; }
		private Matrix4 ParentMatrix
		{
			get
			{
				if (Parent != null) return Parent.ModelMatrix;
				return Matrix4.Identity;
			}
		}

		private float x, y, z, pitch, yaw, roll, scaleX = 1, scaleY = 1, scaleZ = 1;
		public float X
		{
			get { return x; }
			set
			{
				x = value;
				UpdateTranslation();
			}
		}
		public float Y
		{
			get { return y; }
			set
			{
				y = value;
				UpdateTranslation();
			}
		}
		public float Z
		{
			get { return z; }
			set
			{
				z = value;
				UpdateTranslation();
			}
		}

		public float Pitch
		{
			get { return pitch; }
			set
			{
				pitch = value;
				UpdateRotation();
			}
		}
		public float Yaw
		{
			get { return yaw; }
			set
			{
				yaw = value;
				UpdateRotation();
			}
		}
		public float Roll
		{
			get { return roll; }
			set
			{
				roll = value;
				UpdateRotation();
			}
		}

		public float ScaleX
		{
			get { return scaleX; }
			set
			{
				scaleX = value;
				UpdateScale();
			}
		}
		public float ScaleY
		{
			get { return scaleY; }
			set
			{
				scaleY = value;
				UpdateScale();
			}
		}
		public float ScaleZ
		{
			get { return scaleZ; }
			set
			{
				scaleZ = value;
				UpdateScale();
			}
		}
		public float Scale
		{
			set
			{
				scaleX = value;
				scaleY = value;
				scaleZ = value;
				UpdateScale();
			}
		}

		public Mesh Parent { get; set; }
		private LinkedList<Mesh> children;

		private void UpdateTranslation()
		{
			translation = Matrix4.CreateTranslation(x, y, z);
			UpdateShaderMatrix();
		}
		private void UpdateScale()
		{
			scale = Matrix4.Scale(scaleX, scaleY, scaleZ);
			UpdateShaderMatrix();
		}
		private void UpdateRotation()
		{
			rotation = Matrix4.CreateRotationY(yaw) * Matrix4.CreateRotationX(pitch) * Matrix4.CreateRotationZ(roll);
			if (Parent != null) rotation = Matrix4.Mult(rotation, Parent.rotation);
			shaderRotationMatrix = rotation;
			UpdateShaderMatrix();
		}
		private void UpdateShaderMatrix()
		{
			ModelMatrix = Matrix4.Mult(Matrix4.Mult(Matrix4.Mult(scale, rotation), translation), ParentMatrix);
			shaderModelMatrix = Matrix4.Mult(Matrix4.Mult(Matrix4.Mult(Matrix4.Mult(Matrix4.Mult(Matrix4.Mult(
				scale,
				rotation), 
				translation), 
				ParentMatrix), 
				Camera3D.ActiveCamera.Matrix), 
				projection), 
				window);
			foreach (var child in children) child.UpdateRotation();
		}

		#region Shader sources
		private readonly string vertexShaderSource = @"
#version 110

attribute vec3 position;
attribute vec4 color;
attribute vec3 normal;

uniform mat4 matrix;
uniform mat4 rotation;

varying vec4 frag_color;
varying vec3 frag_normal;

void main ()
{
	vec3 newNormal = (rotation * vec4(normal, 1.0)).xyz;
	frag_color = color;
	frag_normal = newNormal;
	gl_Position = matrix * vec4(position, 1.0);
}
";

		private readonly string fragmentShaderSource = @"
#version 110

varying vec4 frag_color;
varying vec3 frag_normal;

void main ()
{
	vec3 newNormal = normalize(frag_normal);
	gl_FragColor = vec4(frag_color.xyz * ((dot(vec3(1.0, 0.0, 0.0), newNormal) + 1.0) / 2.0), 1.0);
	//gl_FragColor = vec4( frag_normal, 1.0 );
}
";
		#endregion

		public Mesh(MeshData data)
		{
			Camera3D.CameraChanged += UpdateShaderMatrix;
			children = new LinkedList<Mesh>();

			Data = data;

			vertexShader = new VertexShader();
			vertexShader.LoadFromSource(vertexShaderSource);
			fragmentShader = new FragmentShader();
			fragmentShader.LoadFromSource(fragmentShaderSource);

			shaderProgram = new ShaderProgram(vertexShader, fragmentShader);
			shaderProgram.Link();
			shaderProgram.Use();

			shaderProgram.AddAttribLocation("position");
			shaderProgram.AddAttribLocation("color");
			shaderProgram.AddAttribLocation("normal");
			shaderProgram.AddUniformLocation("matrix");
			shaderProgram.AddUniformLocation("rotation");

			rotation = Matrix4.Identity;
			scale = Matrix4.Identity;
			translation = Matrix4.Identity;
			projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 2.5F, 1, 0.1F, 100F);
			shaderModelMatrix = Matrix4.Identity;
			shaderRotationMatrix = Matrix4.Identity;

			window = Matrix4.Scale(Firefly.Window.Height / (float)Firefly.Window.Width, 1, 1);

			UpdateShaderMatrix();
		}

		public void AddChild(Mesh child)
		{
			children.AddLast(child);
			child.Parent = this;
		}

		public void RemoveChild(Mesh child)
		{
			children.Remove(child);
			child.Parent = null;
		}

		public void Render()
		{
			if (Data.Empty) return;

			shaderProgram.Use();
			((Uniform)shaderProgram.Locations["matrix"]).LoadMatrix(shaderModelMatrix);
			((Uniform)shaderProgram.Locations["rotation"]).LoadMatrix(shaderRotationMatrix);

			GL.EnableVertexAttribArray(shaderProgram.Locations["position"].Location);
			GL.EnableVertexAttribArray(shaderProgram.Locations["color"].Location);
			GL.EnableVertexAttribArray(shaderProgram.Locations["normal"].Location);

			Data.Buffer.Bind();
			((Attribute)shaderProgram.Locations["position"]).AttributePointerFloat(3, MeshData.VERTEX_SIZE, 0);
			((Attribute)shaderProgram.Locations["color"]).AttributePointerFloat(4, MeshData.VERTEX_SIZE, 3);
			((Attribute)shaderProgram.Locations["normal"]).AttributePointerFloat(3, MeshData.VERTEX_SIZE, 9);

			GL.DrawElements(BeginMode.Triangles, Data.IndexArray.Length, DrawElementsType.UnsignedInt, Data.IndexArray);
		}
	}
}
