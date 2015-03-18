using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAsandbox {
	class Time {
		private static GameTime gameTime;

		public static float time {
			get {
				return (float) gameTime.TotalGameTime.TotalSeconds;
			}
		}

		public static float deltaTime {
			get {
				return (float) gameTime.ElapsedGameTime.TotalSeconds;
			}
		}
	}
}
