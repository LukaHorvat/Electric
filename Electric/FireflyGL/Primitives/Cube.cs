namespace FireflyGL
{
	class Cube : Mesh
	{
		public static MeshData CubeData { get; set; }
		public Vertex FrontTopLeft { get; set; }
		public Vertex FrontTopRight { get; set; }
		public Vertex FrontBottomLeft { get; set; }
		public Vertex FrontBottomRight { get; set; }
		public Vertex BackTopLeft { get; set; }
		public Vertex BackTopRight { get; set; }
		public Vertex BackBottomLeft { get; set; }
		public Vertex BackBottomRight { get; set; }

		public Cube()
			: base(CubeData.Copy())
		{
			FrontTopLeft = Data[0];
			FrontTopRight = Data[1];
			FrontBottomLeft = Data[3];
			FrontBottomRight = Data[2];
			BackTopLeft = Data[4];
			BackTopRight = Data[5];
			BackBottomLeft = Data[7];
			BackBottomRight = Data[6];
			Firefly.AddToRenderList(this);
		}
	}
}
