using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace FireflyGL
{

	public class Camera
	{
		private static Camera currentCamera;
		public static Camera CurrentCamera
		{
			get { return Camera.currentCamera; }
			set { Camera.currentCamera = value; }
		}

		private Matrix4 translationMatrix, rotationMatrix, offsetMatrix, scaleMatrix, finalMatrix;
		public bool RequiresUpdate { get; set; }

		float rotation, x, y, zoom = 1;
		public float Rotation
		{
			get { return -rotation; }
			set
			{
				rotation = value;
				rotationMatrix = Matrix4.CreateRotationZ(-rotation);
				RequiresUpdate = true;
				Input.MouseMoved = true;
			}
		}
		public float X
		{
			get { return -x; }
			set
			{
				x = -value;
				RequiresUpdate = true;
				translationMatrix.Row3.X = x;
				Input.MouseMoved = true;
			}
		}
		public float Y
		{
			get { return -y; }
			set
			{
				y = -value;
				RequiresUpdate = true;
				translationMatrix.Row3.Y = y;
				Input.MouseMoved = true;
			}
		}
		public float Zoom
		{
			get { return zoom; }
			set
			{
				zoom = value;
				scaleMatrix.Row0.X = zoom;
				scaleMatrix.Row1.Y = zoom;
				RequiresUpdate = true;
				Input.MouseMoved = true;
			}
		}
		public Matrix4 Matrix
		{
			get
			{
				if (RequiresUpdate)
					updateMatrices();
				return finalMatrix;
			}
		}

		public Camera()
		{
			RequiresUpdate = true;
			finalMatrix = Matrix4.Identity;
			scaleMatrix = Matrix4.Scale(zoom, zoom, 1);
			offsetMatrix = Matrix4.CreateTranslation(
				Firefly.ViewportWidth / 2,
				Firefly.ViewportHeight / 2,
				0);
			rotationMatrix = Matrix4.CreateRotationZ(rotation);
			translationMatrix = Matrix4.CreateTranslation(x, y, 0);
		}

		public void UpdateToScreenSize()
		{
			finalMatrix = Matrix4.Identity;
			scaleMatrix = Matrix4.Scale(zoom, zoom, 1);
			offsetMatrix = Matrix4.CreateTranslation(
				Firefly.ViewportWidth / 2,
				Firefly.ViewportHeight / 2,
				0);
			rotationMatrix = Matrix4.CreateRotationZ(rotation);
		}

		void updateMatrices()
		{
			finalMatrix = translationMatrix * scaleMatrix * rotationMatrix * offsetMatrix;
			RequiresUpdate = false;
		}

		public void Activate()
		{
			currentCamera = this;
		}

		public Vector2 GetApsoluteMouse(int X, int Y)
		{
			float x, y;
			x = X;
			y = Y;
			x -= Firefly.ViewportWidth / 2;
			y -= Firefly.ViewportHeight / 2;
			x /= zoom;
			y /= zoom;
			float angle = (float)Math.Atan2(y, x);
			angle -= rotation;
			float distance = (float)Math.Sqrt(x * x + y * y);
			x = (float)Math.Cos(angle) * distance;
			y = (float)Math.Sin(angle) * distance;
			x -= this.x;
			y -= this.y;

			return new Vector2(x, y);
		}
	}
}
