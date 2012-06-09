using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	public class Plane : Mesh
	{
		public static MeshData PlaneData { get; set; }
		public Vertex TopLeft { get; set; }
		public Vertex BottomLeft { get; set; }
		public Vertex TopRight { get; set; }
		public Vertex BottomRight { get; set; }

		public Plane()
			: base(PlaneData.Copy())
		{
			TopLeft = Data[0];
			BottomLeft = Data[3];
			TopRight = Data[1];
			BottomRight = Data[2];
			Firefly.AddToRenderList(this);
		}
	}
}
