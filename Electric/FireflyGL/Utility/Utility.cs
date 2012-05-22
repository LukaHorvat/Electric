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
	delegate void RepeatAction(int index);

	class Utility
	{
		static Stopwatch timer = new Stopwatch();
		static Random rand = new Random();
		static Graphics measureGfx;
		static Dictionary<string, int> countdowns = new Dictionary<string, int>();

		static Utility()
		{
			var bmp = new Bitmap(1, 1);
			measureGfx = Graphics.FromImage(bmp);
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

		public static Bitmap MakeTextBitmap(string text, Font font, Brush brush, TextRenderingHint hint = TextRenderingHint.AntiAliasGridFit, Rectangle? wordWrap = null)
		{
			Bitmap toReturn;
			if (wordWrap.HasValue)
			{
				toReturn = new Bitmap((int)wordWrap.Value.Width, (int)wordWrap.Value.Height);
			}
			else
			{
				var size = measureGfx.MeasureString(text, font);
				if (size == SizeF.Empty) size = measureGfx.MeasureString(" ", font);
				toReturn = new Bitmap((int)size.Width, (int)size.Height);
			}
			Graphics gfx = Graphics.FromImage(toReturn);
			gfx.TextRenderingHint = hint;

			if (wordWrap.HasValue)
			{
				gfx.DrawString(text, font, brush, new RectangleF(wordWrap.Value.X, wordWrap.Value.Y, wordWrap.Value.Width, wordWrap.Value.Height));
			}
			else
			{
				gfx.DrawString(text, font, brush, 0, 0);
			}

			gfx.Dispose();

			return toReturn;
		}

		public static Texture MakeTextureFromText(string text, Font font, Brush brush, TextRenderingHint hint = TextRenderingHint.AntiAliasGridFit, Rectangle? wordWrap = null)
		{
			var bmp = MakeTextBitmap(text, font, brush, hint, wordWrap: wordWrap);
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
	}
}
