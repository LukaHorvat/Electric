//#define ProcessErrors
//#define CrashOnError
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Text;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using OpenTK;

namespace FireflyGL.Utility
{
	public delegate void RepeatAction(int index);

	public class Utility
	{
		public static Graphics MeasureGfx;

		static Stopwatch timer = new Stopwatch();
		static Random rand = new Random();
		static Dictionary<string, int> countdowns = new Dictionary<string, int>();

		static Utility()
		{
			var bmp = new Bitmap(1, 1);
			MeasureGfx = Graphics.FromImage(bmp);
		}

		public static double GetRandom()
		{
			return rand.NextDouble();
		}

		public static float GetRandomF()
		{
			return (float)rand.NextDouble();
		}

		public static int GetRandomInt(int max)
		{
			return rand.Next(max);
		}

		public static string LoadTextFromFile(string Path)
		{
			using (StreamReader Stream = new StreamReader(Path))
			{
				string ret = Stream.ReadToEnd();
				return ret;
			}
		}

		public static void StartTimer()
		{
			timer.Reset();
			timer.Start();
		}

		public static double StopTimer()
		{
			timer.Stop();
			return timer.ElapsedTicks / (double)Stopwatch.Frequency * 1000;
		}

		public static Bitmap MakeTextBitmap(string text, Font font, Brush brush, TextRenderingHint hint = TextRenderingHint.AntiAliasGridFit, Rectangle? wordWrap = null, LabelSizeMethod method = LabelSizeMethod.FixedSize)
		{
			Bitmap toReturn;
			SizeF size;
			StringFormat format = new StringFormat { FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.MeasureTrailingSpaces, Trimming = StringTrimming.None };
			StringFormat onlyTrailingSpaces = new StringFormat { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };
			if (wordWrap.HasValue)
			{
				if (wordWrap.Value.Height == 0)
					size = MeasureGfx.MeasureString(text, font,
						(int)wordWrap.Value.Width,
						format);
				else if (wordWrap.Value.Width == 0)
					size = MeasureGfx.MeasureString(text, font,
						new SizeF(float.MaxValue, wordWrap.Value.Height),
						onlyTrailingSpaces);
				else if (method == LabelSizeMethod.SmallestPossible)
					size = MeasureGfx.MeasureString(text, font,
						new SizeF(wordWrap.Value.Width, wordWrap.Value.Height),
						onlyTrailingSpaces);
				else
					size = new SizeF(wordWrap.Value.Width, wordWrap.Value.Height);
			}
			else
			{
				size = MeasureGfx.MeasureString(text, font, int.MaxValue, onlyTrailingSpaces);
			}

			if (size == SizeF.Empty) size = MeasureGfx.MeasureString(" ", font);
			toReturn = new Bitmap((int)size.Width, (int)size.Height);
			Graphics gfx = Graphics.FromImage(toReturn);

			gfx.TextRenderingHint = hint;

			gfx.DrawString(text, font, brush, new RectangleF(0, 0, size.Width, size.Height), format);

			gfx.Dispose();

			return toReturn;
		}

		public static Texture MakeTextureFromText(string text, Font font, Brush brush, TextRenderingHint hint = TextRenderingHint.AntiAliasGridFit, Rectangle? wordWrap = null, LabelSizeMethod method = LabelSizeMethod.FixedSize)
		{
			var bmp = MakeTextBitmap(text, font, brush, hint, wordWrap, method);
			return new Texture(bmp);
		}

		public static void ProcessOGLErrors()
		{
#if ProcessErrors
			var errCode = OpenTK.Graphics.OpenGL.GL.GetError();
			if (errCode != OpenTK.Graphics.OpenGL.ErrorCode.NoError)
			{
#if CrashOnError
				throw new Exception(errCode.ToString());
#else
				Console.WriteLine("OpenGL error: " + errCode);
#endif
			}
#endif
		}

		public static void RepeatFor(int times, RepeatAction action)
		{
			for (int i = 0; i < times; i++) action.Invoke(i);
		}

		public static Rectangle GetUVFromAtlas(int atlasWidth, int atlasHeight, int tilesInRow, int tilesInColumn, int num)
		{
			var atlX = num % tilesInRow;
			var atlY = num / tilesInRow;
			var x = (float)atlX / tilesInRow;
			var y = (float)atlY / tilesInColumn;
			return new Rectangle(x, y, 1.0F / tilesInRow, 1.0F / tilesInColumn);
		}

		public static void StartCountdown(string name, int ticks)
		{
			if (countdowns.Count == 0)
			{
				Firefly.OnUpdate += Update;
			}
			countdowns[name] = ticks;
		}

		public static int GetCountdown(string name)
		{
			if (countdowns.ContainsKey(name))
			{
				return countdowns[name];
			}
			else
				return 0;
		}

		public static void Update()
		{
			var list = new LinkedList<string>();
			foreach (var key in countdowns.Keys)
			{
				if (countdowns[key] > 0)
				{
					list.AddLast(key);
				}
			}
			foreach (var key in list)
			{
				countdowns[key]--;
			}
		}

		public static int Mod(int first, int second)
		{
			if (second <= 0) throw new Exception("The second number must be greater than 0");
			if (first >= 0)
			{
				return first % second;
			}
			else
			{
				return second - (-first % second);
			}
		}

		public static void InitializeList<T1>(List<T1> list, int elements)
		{
			if (list == null) list = new List<T1>(elements);
			for (int i = 0; i < elements; ++i) list.Add(default(T1));
		}

		public static void InitializeList<T1>(List<List<T1>> list, int width, int height)
		{
			if (list == null) list = new List<List<T1>>(width);
			for (int i = 0; i < width; ++i)
			{
				list.Add(new List<T1>(height));
				for (int j = 0; j < height; ++j)
				{
					list[i].Add(default(T1));
				}
			}
		}

		public static void InitializeList<T1>(List<List<List<T1>>> list, int width, int height, int depth)
		{
			if (list == null) list = new List<List<List<T1>>>(width);
			for (int i = 0; i < width; ++i)
			{
				list.Add(new List<List<T1>>(height));
				for (int j = 0; j < height; ++j)
				{
					list[i].Add(new List<T1>(depth));
					for (int k = 0; k < depth; ++k)
					{
						list[i][j].Add(default(T1));
					}
				}
			}
		}
	}
}
