using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAsandbox {
	class Mathf {
		public static float Sin(float a) {
			return (float) Math.Sin((double) a);
		}

		public static float Cos(float a) {
			return (float) Math.Cos((double) a);
		}

		public static float Pow(float a, float b) {
			return (float) Math.Pow((double) a, (double) b);
		}
	}
}
