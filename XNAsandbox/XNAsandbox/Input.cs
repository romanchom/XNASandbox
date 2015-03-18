using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAsandbox {
	class Input {
		private static KeyboardState lastKeyboardState = new KeyboardState();
		private static KeyboardState currentKeyboardState = new KeyboardState();

		private static MouseState lastMouseState = new MouseState();
		private static MouseState currentMouseState = new MouseState();

		public static void Update(){
			lastKeyboardState = currentKeyboardState;
			currentKeyboardState = Keyboard.GetState();

			lastMouseState = currentMouseState;
			currentMouseState = Mouse.GetState();
		}

		public static bool GetKey(Keys key) {
			return currentKeyboardState.IsKeyDown(key);
		}

		public static bool GetKeyDown(Keys key) {
			return currentKeyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key);
		}

		public static bool GetKeyUp(Keys key) {
			return currentKeyboardState.IsKeyUp(key) && lastKeyboardState.IsKeyDown(key);
		}

		public static bool GetMouseButton(int id) {
			return GetMouseFromState(ref currentMouseState, id);
		}

		public static bool GetMouseButtonDown(int id) {
			return GetMouseFromState(ref currentMouseState, id) && !GetMouseFromState(ref lastMouseState, id);
		}

		public static bool GetMouseButtonUp(int id) {
			return !GetMouseFromState(ref currentMouseState, id) && GetMouseFromState(ref lastMouseState, id);
		}

		public static Vector2 mouseDelta{
			get {
				Vector2 ret;
				ret.X = currentMouseState.X - lastMouseState.X;
				ret.Y = currentMouseState.Y - lastMouseState.Y;
				return ret;
			}
		}

		public static Vector2 mousePosition {
			get {
				Vector2 ret;
				ret.X = currentMouseState.X;
				ret.Y = currentMouseState.Y;
				return ret;
			}
		}

		public static float mouseScroll {
			get {
				return currentMouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue;
			}
		}
		
		
		private static bool GetMouseFromState(ref MouseState mouse, int id) {
			switch (id) {
				case 0:
					return mouse.LeftButton == ButtonState.Pressed;
				case 1:
					return mouse.RightButton == ButtonState.Pressed;
				case 2:
					return mouse.MiddleButton == ButtonState.Pressed;
				default:
					throw new Exception("Invalid mouse button");
			}
		}
	}
}
