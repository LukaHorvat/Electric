using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace FireflyGL
{
	public class Camera3D
	{
		public static Camera3D ActiveCamera { get; set; }

		public static event Action CameraChanged;

		static Camera3D()
		{
			ActiveCamera = new Camera3D();
		}

		public Matrix4 Matrix { get; set; }

		public Camera3D()
		{
			Matrix = Matrix4.Identity;
			rotation = Matrix4.Identity;
			translation = Matrix4.Identity;
			scale = Matrix4.Identity;
		}

		public void Activate()
		{
			ActiveCamera = this;
		}

		private Matrix4 rotation;
		private Matrix4 translation;
		private Matrix4 scale;

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

		private void UpdateTranslation()
		{
			translation = Matrix4.CreateTranslation(x, y, z);
			UpdateCameraMatrix();
		}
		private void UpdateScale()
		{
			scale = Matrix4.Scale(scaleX, scaleY, scaleZ);
			UpdateCameraMatrix();
		}
		private void UpdateRotation()
		{
			rotation = Matrix4.CreateRotationY(yaw) * Matrix4.CreateRotationX(pitch) * Matrix4.CreateRotationZ(roll);
			UpdateCameraMatrix();
		}
		private void UpdateCameraMatrix()
		{
			Matrix = Matrix4.Mult(Matrix4.Mult(translation, rotation), scale);
			if (CameraChanged != null) CameraChanged.Invoke();
		}
		public void MoveInDirection(float forward, float right, float up)
		{
			var vec = new Vector4(-right, -up, -forward, 1);
			var temp = rotation;
			temp.Invert();
			vec = Vector4.Transform(vec, temp);
			X += vec.X;
			Y += vec.Y;
			Z += vec.Z;
		}
	}
}
