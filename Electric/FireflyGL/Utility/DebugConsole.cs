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
using System.Runtime.CompilerServices;
using System.Threading;

namespace FireflyGL.Utility
{
	public class DebugConsole : DisplayObject
	{
		public bool Open = false;
		public Dictionary<string, object> ExposedReferences;

		private TextField input;
		private Label consoleText;
		private string output;

		private int currentHistory = 0;
		private List<string> history;
		private DisplayObject visuals;

		public DebugConsole()
		{
			ExposedReferences = new Dictionary<string, object>();

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

			input = new TextField(new System.Drawing.Font("Consolas", 10), System.Drawing.Brushes.White, 0x0, Firefly.Window.Width, 20);
			input.Y = Firefly.Window.Height / 2 - 17;
			input.IllegalChars.AppendMany('\r', '\n');
			visuals.AddChild(input);

			consoleText = new Label("", new Font("Consolas", 10), Brushes.White);
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
				var builder = new StringBuilder();
				foreach (var pair in ExposedReferences)
				{
					if (pair.Value == null) throw new Exception("Console object " + pair.Key + " is bound to null");

					builder.Append(GetFriendlyTypeName(pair.Value.GetType()) + " " + pair.Key + ", ");
				}
				var code = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FireflyGL;
using FireflyGL.Utility;

namespace FireflyGL
{
	class DebugConsole
	{
		static void Run(" + builder.ToString() + @"StringWriter writer)
		{
			var stdOut = Console.Out;
			Console.SetOut(writer);
			" + input.Text + @"
			Console.SetOut(stdOut);
		}
	}
}";
				var compiler = new CodeCompiler();
				compiler.Compile(code);
				var results = compiler.Results;
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
						var writer = new StringWriter();

						compiler.Run(ExposedReferences.Values.ToArray(), writer);

						PushText(writer.ToString());
						history.Add(input.Text);
						currentHistory = history.Count;
						input.Text = "";
						writer.Dispose();
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
					input.CursorPosition = input.Text.Length;
				}
			}
			if (Input.Keys[Key.Down] == InputState.Release)
			{
				if (currentHistory < history.Count - 1)
				{
					currentHistory++;
					input.Text = history[currentHistory];
					input.CursorPosition = input.Text.Length;
				}
			}
		}

		private string GetFriendlyTypeName(Type type)
		{
			if (type.IsGenericParameter)
			{
				return type.Name;
			}

			if (!type.IsGenericType)
			{
				return type.FullName;
			}

			var builder = new System.Text.StringBuilder();
			var name = type.Name;
			var index = name.IndexOf("`");
			builder.AppendFormat("{0}.{1}", type.Namespace, name.Substring(0, index));
			builder.Append('<');
			var first = true;
			foreach (var arg in type.GetGenericArguments())
			{
				if (!first)
				{
					builder.Append(',');
				}
				builder.Append(GetFriendlyTypeName(arg));
				first = false;
			}
			builder.Append('>');
			return builder.ToString();
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
			consoleText = new Label(output, new Font("Consolas", 10), Brushes.White, new global::FireflyGL.Rectangle(0, 0, Firefly.Window.Width, 0));
			visuals.AddChild(consoleText);
			consoleText.Y = Firefly.Window.Height / 2 - 20 - consoleText.Height - 3;
		}

		public static void CheckForNonPublicTypes()
		{
			var types = Assembly.GetExecutingAssembly().GetTypes().Where(type =>
				!type.Attributes.HasFlag(TypeAttributes.Public) &&
				type.Name.IndexOf("PrivateImplementationDetails") == -1);
			if (types.Count() > 0) Console.WriteLine("The following types are not public:");
			foreach (var type in types)
			{
				if (type.Attributes.HasFlag(TypeAttributes.NotPublic))
				{
					Console.WriteLine("\t" + type.FullName);
				}
			}
		}
	}

	public class CodeCompiler
	{
		public CompilerResults Results;

		public void Compile(string code)
		{
			var provider = new CSharpCodeProvider();
			var param = new CompilerParameters();
			param.GenerateInMemory = false;
			param.GenerateExecutable = false;
			param.ReferencedAssemblies.Add("System.dll");
			param.ReferencedAssemblies.Add("System.Core.dll");
			param.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
			Results = provider.CompileAssemblyFromSource(param, code);
		}

		public void Run(object[] arguments, StringWriter writer)
		{
			Results.CompiledAssembly.GetType("FireflyGL.DebugConsole").GetMethod("Run", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, arguments.Concat(new object[] { writer }).ToArray());
		}
	}
}
