using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireflyGL;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Drawing;
using System.IO;

namespace Electric.FireflyGL.Utility
{
	class DebugConsole : DisplayObject
	{
		public bool Open = false;
		public DisplayObject visuals;

		private TextField input;
		private Label consoleText;
		private string output;

		private int currentHistory = 0;
		private List<string> history;

		public DebugConsole()
		{
			visuals = new DisplayObject();
			AddChild(visuals);

			var background = new ColoredRectangle(0, 0, Firefly.Window.Width, Firefly.Window.Height / 2, 0, 0, 0, 0.85F);
			visuals.AddChild(background);
			var line = new ColoredShape();
			line.OutlinePolygons.AddLast(new Polygon(false,
				4, 0, 1, 1, 1, 1,
				Firefly.Window.Width - 4, 0, 1, 1, 1, 1));
			line.SetPolygons();
			line.Y = Firefly.Window.Height / 2 - 20;
			visuals.AddChild(line);

			input = new TextField(new System.Drawing.Font("Courier New", 8), System.Drawing.Brushes.White, 0x0, Firefly.Window.Width, 20);
			input.Y = Firefly.Window.Height / 2 - 17;
			visuals.AddChild(input);

			consoleText = new Label("", new Font("Courier New", 8), Brushes.White);
			visuals.AddChild(consoleText);

			history = new List<string>();

			CloseConsole();
		}

		public override void UpdateSelf()
		{
			base.UpdateSelf();
			if (Input.Keys[Key.Tilde] == InputState.Release)
			{
				if (!Open)
				{
					OpenConsole();
				}
				else
				{
					CloseConsole();
				}
			}
			if (Input.Keys[Key.Enter] == InputState.Release)
			{
				CSharpCodeProvider provider = new CSharpCodeProvider();
				var param = new CompilerParameters();
				param.GenerateInMemory = false;
				param.GenerateExecutable = false;
				param.ReferencedAssemblies.Add("System.dll");
				param.ReferencedAssemblies.Add("System.Core.dll");
				param.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
				var results = provider.CompileAssemblyFromSource(param,
					@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FireflyGL
{
	class Program
	{
		static void Main(StringWriter writer)
		{
			Console.SetOut(writer);
			" + input.Text + @"
		}
	}
}");
				if (results.Errors.HasErrors)
				{
					for (int i = 0; i < results.Errors.Count; ++i)
					{
						PushText(results.Errors[i].ErrorText);
					}
				}
				else
				{
					try
					{
						var writter = new StringWriter();
						results.CompiledAssembly.GetType("FireflyGL.Program").GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { writter });
						PushText(writter.ToString());
						history.Add(input.Text);
						currentHistory = history.Count;
						input.Text = "";
					}
					catch (Exception e)
					{

					}
				}
			}
			if (Input.Keys[Key.Up] == InputState.Release)
			{
				if (currentHistory > 0)
				{
					currentHistory--;
					input.Text = history[currentHistory];
				}
			}
			if (Input.Keys[Key.Down] == InputState.Release)
			{
				if (currentHistory < history.Count - 1)
				{
					currentHistory++;
					input.Text = history[currentHistory];
				}
			}
		}

		public void OpenConsole()
		{
			Open = true;
			Visible = true;
			X = 0;
			input.Active = true;
			input.Focus();
		}

		public void CloseConsole()
		{
			Open = false;
			Visible = false;
			X = -Firefly.Window.Height / 2;
			input.Active = false;
		}

		private void PushText(string text)
		{
			output = output + text + "\n";

			visuals.RemoveChild(consoleText);
			consoleText = new Label(output, new Font("Courier New", 8), Brushes.White);
			visuals.AddChild(consoleText);
			consoleText.Y = Firefly.Window.Height / 2 - 20 - consoleText.Height - 3;
		}
	}
}
