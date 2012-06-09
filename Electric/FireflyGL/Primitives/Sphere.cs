using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	public class Sphere : Mesh
	{
		public const int DETAIL_LEVEL = 128;
		public static MeshData SphereData { get; set; }

		public Sphere()
			: base(SphereData.Copy())
		{
			Firefly.AddToRenderList(this);
		}
	}
}
