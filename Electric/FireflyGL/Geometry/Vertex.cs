namespace FireflyGL
{
	public class Vertex
	{
		public float X
		{
			get { return data.Data[index + 0]; }
			set
			{
				int offset = index + 0;
				data.Data[offset] = value;
				data.SubData(offset, 1, new[] { value });
			}
		}
		public float Y
		{
			get { return data.Data[index + 1]; }
			set
			{
				int offset = index + 1;
				data.Data[offset] = value;
				data.SubData(offset, 1, new[] { value });
			}
		}
		public float Z
		{
			get { return data.Data[index + 2]; }
			set
			{
				int offset = index + 2;
				data.Data[offset] = value;
				data.SubData(offset, 1, new[] { value });
			}
		}
		public float R
		{
			get { return data.Data[index + 3]; }
			set
			{
				int offset = index + 3;
				data.Data[offset] = value;
				data.SubData(offset, 1, new[] { value });
			}
		}
		public float G
		{
			get { return data.Data[index + 4]; }
			set
			{
				int offset = index + 4;
				data.Data[offset] = value;
				data.SubData(offset, 1, new[] { value });
			}
		}
		public float B
		{
			get { return data.Data[index + 5]; }
			set
			{
				int offset = index + 5;
				data.Data[offset] = value;
				data.SubData(offset, 1, new[] { value });
			}
		}
		public float A
		{
			get { return data.Data[index + 6]; }
			set
			{
				int offset = index + 6;
				data.Data[offset] = value;
				data.SubData(offset, 1, new[] { value });
			}
		}
		public float U
		{
			get { return data.Data[index + 7]; }
			set
			{
				int offset = index + 7;
				data.Data[offset] = value;
				data.SubData(offset, 1, new[] { value });
			}
		}
		public float V
		{
			get { return data.Data[index + 8]; }
			set
			{
				int offset = index + 8;
				data.Data[offset] = value;
				data.SubData(offset, 1, new[] { value });
			}
		}
		public float NormalX
		{
			get { return data.Data[index + 9]; }
			set
			{
				int offset = index + 9;
				data.Data[offset] = value;
				data.SubData(offset, 1, new[] { value });
			}
		}
		public float NormalY
		{
			get { return data.Data[index + 10]; }
			set
			{
				int offset = index + 10;
				data.Data[offset] = value;
				data.SubData(offset, 1, new[] { value });
			}
		}
		public float NormalZ
		{
			get { return data.Data[index + 11]; }
			set
			{
				int offset = index + 11;
				data.Data[offset] = value;
				data.SubData(offset, 1, new[] { value });
			}
		}
		public bool NoNormals
		{
			get { return NormalX == 0F && NormalY == 0F && NormalZ == 0F; }
		}

		readonly MeshData data;
		readonly int index;

		public Vertex(MeshData link, int index)
		{
			data = link;
			this.index = index * MeshData.VERTEX_SIZE;
		}
	}
}
