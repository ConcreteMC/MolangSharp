using System;

namespace ConcreteMC.MolangSharp.Runtime
{
	/// <summary>
	///		The easing curves behind the <c>math.ease_*</c> functions
	/// </summary>
	/// <remarks>
	///		Every documented easing function has the signature <c>math.ease_x(start, end, 0_to_1)</c> and "Output goes
	///		from start to end via 0_to_1", so each curve below maps an interpolant in <c>[0, 1]</c> onto a weight that
	///		is 0 at t=0 and 1 at t=1. <see cref="Interpolate"/> applies that weight to the start/end pair.
	/// </remarks>
	public static class MoLangEasings
	{
		private const double BackC1 = 1.70158d;
		private const double BackC2 = BackC1 * 1.525d;
		private const double BackC3 = BackC1 + 1d;

		private const double ElasticC4 = 2d * Math.PI / 3d;
		private const double ElasticC5 = 2d * Math.PI / 4.5d;

		private const double BounceN1 = 7.5625d;
		private const double BounceD1 = 2.75d;

		/// <summary>
		///		Applies an easing curve to a start/end pair
		/// </summary>
		/// <param name="curve">The easing curve to apply</param>
		/// <param name="start">The value returned at <paramref name="amount"/> 0</param>
		/// <param name="end">The value returned at <paramref name="amount"/> 1</param>
		/// <param name="amount">The interpolant, clamped to [0, 1]</param>
		public static double Interpolate(Func<double, double> curve, double start, double end, double amount)
		{
			amount = Math.Max(0d, Math.Min(1d, amount));

			if (amount <= 0d)
				return start;

			if (amount >= 1d)
				return end;

			return start + (end - start) * curve(amount);
		}

		public static double InSine(double t) => 1d - Math.Cos(t * Math.PI / 2d);

		public static double OutSine(double t) => Math.Sin(t * Math.PI / 2d);

		public static double InOutSine(double t) => -(Math.Cos(Math.PI * t) - 1d) / 2d;

		public static double InQuad(double t) => t * t;

		public static double OutQuad(double t) => 1d - (1d - t) * (1d - t);

		public static double InOutQuad(double t) =>
			t < 0.5d ? 2d * t * t : 1d - Math.Pow(-2d * t + 2d, 2d) / 2d;

		public static double InCubic(double t) => t * t * t;

		public static double OutCubic(double t) => 1d - Math.Pow(1d - t, 3d);

		public static double InOutCubic(double t) =>
			t < 0.5d ? 4d * t * t * t : 1d - Math.Pow(-2d * t + 2d, 3d) / 2d;

		public static double InQuart(double t) => t * t * t * t;

		public static double OutQuart(double t) => 1d - Math.Pow(1d - t, 4d);

		public static double InOutQuart(double t) =>
			t < 0.5d ? 8d * t * t * t * t : 1d - Math.Pow(-2d * t + 2d, 4d) / 2d;

		public static double InQuint(double t) => t * t * t * t * t;

		public static double OutQuint(double t) => 1d - Math.Pow(1d - t, 5d);

		public static double InOutQuint(double t) =>
			t < 0.5d ? 16d * t * t * t * t * t : 1d - Math.Pow(-2d * t + 2d, 5d) / 2d;

		public static double InExpo(double t) => t <= 0d ? 0d : Math.Pow(2d, 10d * t - 10d);

		public static double OutExpo(double t) => t >= 1d ? 1d : 1d - Math.Pow(2d, -10d * t);

		public static double InOutExpo(double t)
		{
			if (t <= 0d)
				return 0d;

			if (t >= 1d)
				return 1d;

			return t < 0.5d
				? Math.Pow(2d, 20d * t - 10d) / 2d
				: (2d - Math.Pow(2d, -20d * t + 10d)) / 2d;
		}

		public static double InCirc(double t) => 1d - Math.Sqrt(1d - t * t);

		public static double OutCirc(double t) => Math.Sqrt(1d - (t - 1d) * (t - 1d));

		public static double InOutCirc(double t) =>
			t < 0.5d
				? (1d - Math.Sqrt(1d - Math.Pow(2d * t, 2d))) / 2d
				: (Math.Sqrt(1d - Math.Pow(-2d * t + 2d, 2d)) + 1d) / 2d;

		public static double InBack(double t) => BackC3 * t * t * t - BackC1 * t * t;

		public static double OutBack(double t) =>
			1d + BackC3 * Math.Pow(t - 1d, 3d) + BackC1 * Math.Pow(t - 1d, 2d);

		public static double InOutBack(double t) =>
			t < 0.5d
				? Math.Pow(2d * t, 2d) * ((BackC2 + 1d) * 2d * t - BackC2) / 2d
				: (Math.Pow(2d * t - 2d, 2d) * ((BackC2 + 1d) * (t * 2d - 2d) + BackC2) + 2d) / 2d;

		public static double InElastic(double t)
		{
			if (t <= 0d)
				return 0d;

			if (t >= 1d)
				return 1d;

			return -Math.Pow(2d, 10d * t - 10d) * Math.Sin((t * 10d - 10.75d) * ElasticC4);
		}

		public static double OutElastic(double t)
		{
			if (t <= 0d)
				return 0d;

			if (t >= 1d)
				return 1d;

			return Math.Pow(2d, -10d * t) * Math.Sin((t * 10d - 0.75d) * ElasticC4) + 1d;
		}

		public static double InOutElastic(double t)
		{
			if (t <= 0d)
				return 0d;

			if (t >= 1d)
				return 1d;

			return t < 0.5d
				? -(Math.Pow(2d, 20d * t - 10d) * Math.Sin((20d * t - 11.125d) * ElasticC5)) / 2d
				: Math.Pow(2d, -20d * t + 10d) * Math.Sin((20d * t - 11.125d) * ElasticC5) / 2d + 1d;
		}

		public static double InBounce(double t) => 1d - OutBounce(1d - t);

		public static double OutBounce(double t)
		{
			if (t < 1d / BounceD1)
				return BounceN1 * t * t;

			if (t < 2d / BounceD1)
			{
				t -= 1.5d / BounceD1;

				return BounceN1 * t * t + 0.75d;
			}

			if (t < 2.5d / BounceD1)
			{
				t -= 2.25d / BounceD1;

				return BounceN1 * t * t + 0.9375d;
			}

			t -= 2.625d / BounceD1;

			return BounceN1 * t * t + 0.984375d;
		}

		public static double InOutBounce(double t) =>
			t < 0.5d ? (1d - OutBounce(1d - 2d * t)) / 2d : (1d + OutBounce(2d * t - 1d)) / 2d;
	}
}
