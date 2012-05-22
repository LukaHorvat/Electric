using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FireflyGL
{
	public class MeshData
	{
		public const int VERTEX_SIZE = 12;

		public float[] Data { get; set; }
		public uint[] IndexArray { get; set; }
		public Buffer Buffer { get; set; }
		public bool Empty { get; private set; }
		public int Size { get { return Data.Length / VERTEX_SIZE; } }

		public void GenerateBuffer()
		{
			Buffer = new Buffer(BufferTarget.ArrayBuffer);
			Buffer.SetDataFloat(BufferUsageHint.StaticDraw, Data);
			if (IndexArray == null || IndexArray.Length == 0)
			{
				IndexArray = new uint[Data.Length / VERTEX_SIZE];
				Utility.Utility.RepeatFor(IndexArray.Length, x => IndexArray[x] = (uint)x);
			}
			Empty = false;
		}

		public MeshData()
		{
			Data = new float[0];
			Buffer = new Buffer(BufferTarget.ArrayBuffer);
			Empty = true;
		}

		public MeshData(float[] data, uint[] indices = null)
		{
			IndexArray = indices;
			Data = data;
			GenerateBuffer();
		}

		public void GenerateNormals()
		{
			var avg = new Vector3[Size];
			for (int i = 0; i < IndexArray.Length; i += 3)
			{
				Vertex a = this[IndexArray[i]], b = this[IndexArray[i + 1]], c = this[IndexArray[i + 2]];
				var normal = GetFaceNormals(a, b, c);
				for (int j = 0; j < 3; ++j)
				{
					avg[IndexArray[i + j]] += normal;
				}
			}
			for (int i = 0; i < Size; ++i)
			{
				avg[i].Normalize();
				this[i].NormalX = avg[i].X;
				this[i].NormalY = avg[i].Y;
				this[i].NormalZ = avg[i].Z;
			}
		}

		private static Vector3 GetFaceNormals(Vertex a, Vertex b, Vertex c)
		{
			var vec = Vector3.Cross(new Vector3(b.X - a.X, b.Y - a.Y, b.Z - a.Z), new Vector3(c.X - a.X, c.Y - a.Y, c.Z - a.Z));
			if (vec.Length <= 0.001F)
			{
				vec = new Vector3(1, 0, 0);
			}
			vec.Normalize();
			return vec;
		}

		public void SubData(int start, int length, float[] data)
		{
			Buffer.Bind();
			GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(start * sizeof(float)), new IntPtr(length * sizeof(float)), data);
		}

		public Vertex this[uint index]
		{
			get { return this[(int)index]; }
			set { this[(int)index] = value; }
		}

		public Vertex this[int index]
		{
			get
			{
				if (index * VERTEX_SIZE >= Data.Length) throw new IndexOutOfRangeException("Cannot return the vertex because the specivied index is out of bounds of the array");
				return new Vertex(this, index);
			}
			set
			{
				int adjustedIndex = index * VERTEX_SIZE;
				if (Empty || adjustedIndex >= Data.Length) return;
				Data[adjustedIndex + 0] = value.X;
				Data[adjustedIndex + 1] = value.Y;
				Data[adjustedIndex + 2] = value.Z;
				Data[adjustedIndex + 3] = value.R;
				Data[adjustedIndex + 4] = value.G;
				Data[adjustedIndex + 5] = value.B;
				Data[adjustedIndex + 6] = value.A;
				Data[adjustedIndex + 7] = value.U;
				Data[adjustedIndex + 8] = value.V;

				SubData(adjustedIndex, VERTEX_SIZE, new[]{
					value.X,
					value.Y,
					value.Z,
					value.R,
					value.G,
					value.B,
					value.A,
					value.U,
					value.V
				});
			}
		}

		public MeshData Copy()
		{
			return new MeshData((float[])Data.Clone(), (uint[])IndexArray.Clone());
		}
	}
}
