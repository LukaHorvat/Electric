using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;

namespace FireflyGL
{

	public delegate void MouseHandler(MouseEventArgs Args);

	public struct MouseEvenArguments
	{

	}

	public enum Key : int
	{
		Unknown = 0,

		// Modifiers
		ShiftLeft,
		LShift = ShiftLeft,
		ShiftRight,
		RShift = ShiftRight,
		ControlLeft,
		LControl = ControlLeft,
		ControlRight,
		RControl = ControlRight,
		AltLeft,
		LAlt = AltLeft,
		AltRight,
		RAlt = AltRight,
		WinLeft,
		LWin = WinLeft,
		WinRight,
		RWin = WinRight,
		Menu,

		// Function keys (hopefully enough for most keyboards - mine has 26)
		// <keysymdef.h> on X11 reports up to 35 function keys.
		F1,
		F2,
		F3,
		F4,
		F5,
		F6,
		F7,
		F8,
		F9,
		F10,
		F11,
		F12,
		F13,
		F14,
		F15,
		F16,
		F17,
		F18,
		F19,
		F20,
		F21,
		F22,
		F23,
		F24,
		F25,
		F26,
		F27,
		F28,
		F29,
		F30,
		F31,
		F32,
		F33,
		F34,
		F35,

		// Direction arrows
		Up,
		Down,
		Left,
		Right,

		Enter,
		Escape,
		Space,
		Tab,
		BackSpace,
		Back = BackSpace,
		Insert,
		Delete,
		PageUp,
		PageDown,
		Home,
		End,
		CapsLock,
		ScrollLock,
		PrintScreen,
		Pause,
		NumLock,

		// Special keys
		Clear,
		Sleep,
		/*LogOff,
		Help,
		Undo,
		Redo,
		New,
		Open,
		Close,
		Reply,
		Forward,
		Send,
		Spell,
		Save,
		Calculator,
         
		// Folders and applications
		Documents,
		Pictures,
		Music,
		MediaPlayer,
		Mail,
		Browser,
		Messenger,
         
		// Multimedia keys
		Mute,
		PlayPause,
		Stop,
		VolumeUp,
		VolumeDown,
		TrackPrevious,
		TrackNext,*/

		// Keypad keys
		Keypad0,
		Keypad1,
		Keypad2,
		Keypad3,
		Keypad4,
		Keypad5,
		Keypad6,
		Keypad7,
		Keypad8,
		Keypad9,
		KeypadDivide,
		KeypadMultiply,
		KeypadSubtract,
		KeypadMinus = KeypadSubtract,
		KeypadAdd,
		KeypadPlus = KeypadAdd,
		KeypadDecimal,
		KeypadEnter,

		// Letters
		A,
		B,
		C,
		D,
		E,
		F,
		G,
		H,
		I,
		J,
		K,
		L,
		M,
		N,
		O,
		P,
		Q,
		R,
		S,
		T,
		U,
		V,
		W,
		X,
		Y,
		Z,

		// Numbers
		Number0,
		Number1,
		Number2,
		Number3,
		Number4,
		Number5,
		Number6,
		Number7,
		Number8,
		Number9,

		// Symbols
		Tilde,
		Minus,
		//Equal,
		Plus,
		BracketLeft,
		LBracket = BracketLeft,
		BracketRight,
		RBracket = BracketRight,
		Semicolon,
		Quote,
		Comma,
		Period,
		Slash,
		BackSlash,
		LastKey
	}
	public enum MouseButton : int
	{
		Left = 0,
		Middle,
		Right,
		Button1,
		Button2,
		Button3,
		Button4,
		Button5,
		Button6,
		Button7,
		Button8,
		Button9,
		LastButton
	}
	public enum InputState : int
	{
		Up = 0,
		Down,
		Release,
		Press
	}


	class Input
	{
		public static event MouseHandler MouseClick;
		public static event MouseHandler MouseDown;
		public static event MouseHandler MousePress;
		public static event MouseHandler MouseRelease;
		public static event MouseHandler MouseMove;

		private static OpenTK.Vector2 absoluteMouse;
		public static bool MouseMoved { get; set; }
		public static bool MouseHandled;
		public static int RelativeMouseX, RelativeMouseY;

		public static int DeltaMouseX, DeltaMouseY;
		private static int lastMouseX, lastMouseY;

		public static int MouseX
		{
			get
			{
				if (MouseMoved) UpdateMouse();
				return (int)absoluteMouse.X;
			}
		}
		public static int MouseY
		{
			get
			{
				if (MouseMoved) UpdateMouse();
				return (int)absoluteMouse.Y;
			}
		}

		public static float WheelDelta { get; set; }
		public static Dictionary<Key, InputState> Keys { get; set; }
		public static Dictionary<MouseButton, InputState> MouseButtons { get; set; }

		public static Stack<char> TypedSymbols;

		private static LinkedList<Key> keysToRelease, keysToPress;
		private static LinkedList<MouseButton> mouseButtonsToRelease, mouseButtonsToPress;
		private static float absoluteWheel;

		public static void Initialize()
		{
			MouseMoved = true;

			MouseDown = new MouseHandler(DownHandler);
			MouseClick = new MouseHandler(ClickHandler);
			MousePress = new MouseHandler(PressHandler);
			MouseRelease = new MouseHandler(ReleaseHandler);
			MouseMove = new MouseHandler(MoveHandler);

			keysToRelease = new LinkedList<Key>();
			keysToPress = new LinkedList<Key>();
			mouseButtonsToRelease = new LinkedList<MouseButton>();
			mouseButtonsToPress = new LinkedList<MouseButton>();
			Keys = new Dictionary<Key, InputState>();
			MouseButtons = new Dictionary<MouseButton, InputState>();

			string[] names = Enum.GetNames(typeof(Key));
			for (int i = 0; i < names.Length; ++i)
			{
				try
				{
					Keys.Add((Key)Enum.Parse(typeof(Key), names[i]), InputState.Up);
				}
				catch (Exception e)
				{
					string stupingWarnings = e.Message;
				}
			}

			names = Enum.GetNames(typeof(MouseButton));
			for (int i = 0; i < names.Length; ++i)
			{
				try
				{
					MouseButtons.Add((MouseButton)Enum.Parse(typeof(MouseButton), names[i]), InputState.Up);
				}
				catch (Exception e)
				{
					string stupidWarnings = e.Message;
				}
			}

			Firefly.Window.GameWindow.Mouse.Move += new EventHandler<MouseMoveEventArgs>(OpentkMove);
			Firefly.Window.GameWindow.Mouse.ButtonDown += new EventHandler<MouseButtonEventArgs>(OpentkMouseDown);
			Firefly.Window.GameWindow.Mouse.ButtonUp += new EventHandler<MouseButtonEventArgs>(OpentkMouseUp);
			Firefly.Window.GameWindow.Keyboard.KeyDown += new EventHandler<KeyboardKeyEventArgs>(OpentkKeyDown);
			Firefly.Window.GameWindow.Keyboard.KeyUp += new EventHandler<KeyboardKeyEventArgs>(OpentkKeyUp);
			Firefly.Window.GameWindow.Mouse.WheelChanged += new EventHandler<MouseWheelEventArgs>(OpentkWheelChange);
			Firefly.Window.GameWindow.KeyPress += new EventHandler<OpenTK.KeyPressEventArgs>(OpenTKKeyPress);

			TypedSymbols = new Stack<char>();
		}

		public static void Update()
		{
			MouseHandled = false;
			WheelDelta = 0;
			foreach (Key key in keysToPress)
			{
				Keys[key] = InputState.Down;
			}
			foreach (MouseButton button in mouseButtonsToPress)
			{
				MouseButtons[button] = InputState.Down;
			}
			foreach (Key key in keysToRelease)
			{
				Keys[key] = InputState.Up;
			}
			foreach (MouseButton button in mouseButtonsToRelease)
			{
				MouseButtons[button] = InputState.Up;
			}
			keysToRelease.Clear();
			mouseButtonsToRelease.Clear();
			keysToPress.Clear();
			mouseButtonsToPress.Clear();
			TypedSymbols.Clear();

			DeltaMouseX = MouseX - lastMouseX;
			DeltaMouseY = MouseY - lastMouseY;
			lastMouseX = MouseX;
			lastMouseY = MouseY;
		}

		private static void UpdateMouse()
		{
			MouseMoved = false;
			absoluteMouse = Camera.CurrentCamera.GetApsoluteMouse(RelativeMouseX, RelativeMouseY);
		}

		private static void OpentkWheelChange(object sender, MouseWheelEventArgs e)
		{
			WheelDelta = e.ValuePrecise - absoluteWheel;
			absoluteWheel = e.ValuePrecise;
		}
		private static void OpentkMouseUp(object sender, MouseButtonEventArgs e)
		{
			MouseButton temp = (MouseButton)Enum.Parse(
				typeof(MouseButton),
				Enum.GetName(typeof(MouseButton), (int)e.Button)
				);
			MouseButtons[temp] = InputState.Release;
			mouseButtonsToRelease.AddLast(temp);
		}
		private static void OpentkMouseDown(object sender, MouseButtonEventArgs e)
		{
			MouseButton temp = (MouseButton)Enum.Parse(
				typeof(MouseButton),
				Enum.GetName(typeof(MouseButton), (int)e.Button)
				);
			MouseButtons[temp] = InputState.Press;
			mouseButtonsToPress.AddLast(temp);
		}
		private static void OpentkKeyUp(object sender, KeyboardKeyEventArgs e)
		{
			Key temp = (Key)Enum.Parse(
				typeof(Key),
				Enum.GetName(typeof(Key), (int)e.Key)
				);
			Keys[temp] = InputState.Release;
			keysToRelease.AddLast(temp);
		}
		private static void OpentkKeyDown(object sender, KeyboardKeyEventArgs e)
		{
			Key temp = (Key)Enum.Parse(
				typeof(Key),
				Enum.GetName(typeof(Key), (int)e.Key)
				);
			Keys[temp] = InputState.Press;
			keysToPress.AddLast(temp);
		}
		private static void OpenTKKeyPress(object sender, OpenTK.KeyPressEventArgs e)
		{
			TypedSymbols.Push(e.KeyChar);
		}
		private static void OpentkMove(object sender, MouseMoveEventArgs e)
		{
			RelativeMouseX = e.X;
			RelativeMouseY = e.Y;
			MouseMoved = true;
		}

		private static void PressHandler(MouseEventArgs Args)
		{

		}
		private static void DownHandler(MouseEventArgs Args)
		{

		}
		private static void ClickHandler(MouseEventArgs Args)
		{

		}
		private static void ReleaseHandler(MouseEventArgs Args)
		{

		}
		private static void MoveHandler(MouseEventArgs Args)
		{

		}

		public static char GetChar()
		{
			if (!Keys.ContainsValue(InputState.Release)) return (char)0;
			var clickedKey = Keys.First(x => x.Value == InputState.Release).Key;
			return GetCharFromKey(clickedKey);
		}

		private static char GetCharFromKey(Key clickedKey)
		{
			if ((int)clickedKey >= (int)Key.A && (int)clickedKey <= (int)Key.Z)
			{
				if (Keys[Key.ShiftLeft] == InputState.Down || Keys[Key.ShiftRight] == InputState.Down)
				{
					return clickedKey.ToString()[0];
				}
				else
				{
					return clickedKey.ToString().ToLower()[0];
				}
			}
			if ((int)clickedKey >= (int)Key.Number0 && (int)clickedKey <= (int)Key.Number9)
			{
				return clickedKey.ToString().Substring(6)[0];
			}
			if ((int)clickedKey >= (int)Key.Keypad0 && (int)clickedKey <= (int)Key.Keypad9)
			{
				return clickedKey.ToString().Substring(6)[0];
			}
			switch (clickedKey)
			{
				case Key.Space:
					return ' ';
				case Key.Enter:
					return '\n';
				case Key.Period:
					return '.';
				case Key.Comma:
					return ',';
				case Key.Minus:
					return '-';
				case Key.Semicolon:
					return ';';
			}
			return (char)0;
		}
	}
}
