using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAsandbox {
	class Random {
		private static System.Random generator = new System.Random();
		public static float value {
			get {
				return (float) generator.NextDouble();
			}
		}

		public static float integer {
			get {
				return generator.Next();
			}
		}

		public static float Range(float a, float b) {
			return (float) (generator.NextDouble() * (b - a) + a);
		}

		public static int Range(int a, int b) {
			return generator.Next() % (b - a) + a;
		}
	}
}
