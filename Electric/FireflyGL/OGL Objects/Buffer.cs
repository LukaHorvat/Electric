using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace FireflyGL
{

	public class Buffer : IDeletable
	{
		private int id;
		public int Id
		{
			get { return id; }
			private set { id = value; }
		}
		public int Length { get; set; }
		public BufferTarget Type { get; set; }

		public Buffer(BufferTarget type)
		{
			int temp;
			GL.GenBuffers(1, out temp);
			Id = temp;
			Type = type;
		}

		public void Bind()
		{
			GL.BindBuffer(Type, Id);
		}

		public void SetDataFloat(BufferUsageHint Hint, float[] Data)
		{
			GL.BindBuffer(Type, Id);
			GL.BufferData(Type, (IntPtr)(Data.Length * sizeof(float)), Data, Hint);
			Length = Data.Length;
		}

		public void SetDataUint(BufferUsageHint Hint, uint[] Data)
		{
			GL.BindBuffer(Type, Id);
			GL.BufferData(Type, (IntPtr)(Data.Length * sizeof(uint)), Data, Hint);
			Length = Data.Length;
		}

		public void SetDataInt(BufferUsageHint hint, int[] data)
		{
			GL.BindBuffer(Type, Id);
			GL.BufferData(Type, (IntPtr)(data.Length * sizeof(int)), data, hint);
			Length = data.Length;
		}
		
		public void Delete()
		{
			try
			{
				GL.DeleteBuffers(1, ref id);
			}
			catch
			{
				Console.WriteLine("what");
			}
		}
	}
}
