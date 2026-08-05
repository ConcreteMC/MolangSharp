using System;
using System.Collections.Generic;
using ConcreteMC.MolangSharp.Attributes;
using ConcreteMC.MolangSharp.Runtime.Struct;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Runtime
{
	/// <summary>
	///		The default Math implementations for the MoLang runtime
	/// </summary>
	/// <remarks>
	///		Mirrors the official Molang math function reference. Trigonometry works in degrees.
	/// </remarks>
	public static class MoLangMath
	{
		/// <summary>
		///		Conversion factor from degrees to radians
		/// </summary>
		private const double DegreesToRadians = Math.PI / 180d;

		/// <summary>
		///		Conversion factor from radians to degrees
		/// </summary>
		private const double RadiansToDegrees = 180d / Math.PI;

		/// <summary>
		///		The <c>ease_*</c> functions documented in the math function reference, by their Molang name
		/// </summary>
		/// <remarks>
		///		Each curve maps an interpolant in <c>[0, 1]</c> onto a weight; use
		///		<see cref="MoLangEasings.Interpolate"/> to apply one to a start/end pair.
		/// </remarks>
		public static readonly IReadOnlyDictionary<string, Func<double, double>> Easings =
			new Dictionary<string, Func<double, double>>(StringComparer.OrdinalIgnoreCase)
			{
				{"ease_in_sine", MoLangEasings.InSine},
				{"ease_out_sine", MoLangEasings.OutSine},
				{"ease_in_out_sine", MoLangEasings.InOutSine},
				{"ease_in_quad", MoLangEasings.InQuad},
				{"ease_out_quad", MoLangEasings.OutQuad},
				{"ease_in_out_quad", MoLangEasings.InOutQuad},
				{"ease_in_cubic", MoLangEasings.InCubic},
				{"ease_out_cubic", MoLangEasings.OutCubic},
				{"ease_in_out_cubic", MoLangEasings.InOutCubic},
				{"ease_in_quart", MoLangEasings.InQuart},
				{"ease_out_quart", MoLangEasings.OutQuart},
				{"ease_in_out_quart", MoLangEasings.InOutQuart},
				{"ease_in_quint", MoLangEasings.InQuint},
				{"ease_out_quint", MoLangEasings.OutQuint},
				{"ease_in_out_quint", MoLangEasings.InOutQuint},
				{"ease_in_expo", MoLangEasings.InExpo},
				{"ease_out_expo", MoLangEasings.OutExpo},
				{"ease_in_out_expo", MoLangEasings.InOutExpo},
				{"ease_in_circ", MoLangEasings.InCirc},
				{"ease_out_circ", MoLangEasings.OutCirc},
				{"ease_in_out_circ", MoLangEasings.InOutCirc},
				{"ease_in_back", MoLangEasings.InBack},
				{"ease_out_back", MoLangEasings.OutBack},
				{"ease_in_out_back", MoLangEasings.InOutBack},
				{"ease_in_elastic", MoLangEasings.InElastic},
				{"ease_out_elastic", MoLangEasings.OutElastic},
				{"ease_in_out_elastic", MoLangEasings.InOutElastic},
				{"ease_in_bounce", MoLangEasings.InBounce},
				{"ease_out_bounce", MoLangEasings.OutBounce},
				{"ease_in_out_bounce", MoLangEasings.InOutBounce}
			};

		/// <summary>
		///		The Math library
		/// </summary>
		public static readonly QueryStruct Library = CreateLibrary();

		private static QueryStruct CreateLibrary()
		{
			var functions = new Dictionary<string, Func<MoParams, object>>(StringComparer.OrdinalIgnoreCase)
			{
				{"abs", param => Math.Abs(param.GetDouble(0))},
				{"acos", param => Acos(param.GetDouble(0))},
				{"asin", param => Asin(param.GetDouble(0))},
				{"atan", param => Atan(param.GetDouble(0))},
				{"atan2", param => Atan2(param.GetDouble(0), param.GetDouble(1))},
				{"ceil", param => Math.Ceiling(param.GetDouble(0))},
				{"clamp", param => Clamp(param.GetDouble(0), param.GetDouble(1), param.GetDouble(2))},
				{"copy_sign", param => CopySign(param.GetDouble(0), param.GetDouble(1))},
				{"cos", param => Cos(param.GetDouble(0))},
				{"die_roll", param => DieRoll(param.GetDouble(0), param.GetDouble(1), param.GetDouble(2))},
				{"die_roll_integer", param => DieRollInt(param.GetInt(0), param.GetInt(1), param.GetInt(2))},
				{"exp", param => Math.Exp(param.GetDouble(0))},
				{"floor", param => Math.Floor(param.GetDouble(0))},
				{"hermite_blend", param => HermiteBlend(param.GetDouble(0))},
				{"inverse_lerp", param => InverseLerp(param.GetDouble(0), param.GetDouble(1), param.GetDouble(2))},
				{"lerp", param => Lerp(param.GetDouble(0), param.GetDouble(1), param.GetDouble(2))},
				{"lerprotate", param => LerpRotate(param.GetDouble(0), param.GetDouble(1), param.GetDouble(2))},
				{"lerp_rotate", param => LerpRotate(param.GetDouble(0), param.GetDouble(1), param.GetDouble(2))},
				{"ln", param => Math.Log(param.GetDouble(0))},
				{"max", param => Math.Max(param.GetDouble(0), param.GetDouble(1))},
				{"min", param => Math.Min(param.GetDouble(0), param.GetDouble(1))},
				{"min_angle", param => MinAngle(param.GetDouble(0))},
				{"mod", param => param.GetDouble(0) % param.GetDouble(1)},
				{"pi", param => Math.PI},
				{"pow", param => Math.Pow(param.GetDouble(0), param.GetDouble(1))},
				{"random", param => Random(param.GetDouble(0), param.GetDouble(1))},
				{"random_integer", param => RandomInt(param.GetInt(0), param.GetInt(1))},
				{"round", param => Round(param.GetDouble(0))},
				{"sign", param => Sign(param.GetDouble(0))},
				{"sin", param => Sin(param.GetDouble(0))},
				{"sqrt", param => Math.Sqrt(param.GetDouble(0))},
				{"trunc", param => Math.Truncate(param.GetDouble(0))}
			};

			foreach (var easing in Easings)
			{
				var curve = easing.Value;

				functions.Add(
					easing.Key,
					param => MoLangEasings.Interpolate(
						curve, param.GetDouble(0), param.GetDouble(1), param.GetDouble(2)));
			}

			return new QueryStruct(functions);
		}

		/// <summary>
		///		Sine (in degrees) of value
		/// </summary>
		public static double Sin(double value)
		{
			return Math.Sin(value * DegreesToRadians);
		}

		/// <summary>
		///		Cosine (in degrees) of value
		/// </summary>
		public static double Cos(double value)
		{
			return Math.Cos(value * DegreesToRadians);
		}

		/// <summary>
		///		Arcsine of value, in degrees
		/// </summary>
		public static double Asin(double value)
		{
			return Math.Asin(value) * RadiansToDegrees;
		}

		/// <summary>
		///		Arccosine of value, in degrees
		/// </summary>
		public static double Acos(double value)
		{
			return Math.Acos(value) * RadiansToDegrees;
		}

		/// <summary>
		///		Arctangent of value, in degrees
		/// </summary>
		public static double Atan(double value)
		{
			return Math.Atan(value) * RadiansToDegrees;
		}

		/// <summary>
		///		Arctangent of y/x, in degrees
		/// </summary>
		public static double Atan2(double y, double x)
		{
			return Math.Atan2(y, x) * RadiansToDegrees;
		}

		/// <summary>
		///		Clamp value to between min and max inclusive
		/// </summary>
		public static double Clamp(double value, double min, double max)
		{
			return Math.Min(max, Math.Max(value, min));
		}

		/// <summary>
		///		Compose a floating-point value with the magnitude of <paramref name="magnitude"/> and the sign of
		///		<paramref name="sign"/>
		/// </summary>
		public static double CopySign(double magnitude, double sign)
		{
			// Math.CopySign is not available on every target framework. Inspecting the raw bits keeps -0.0 negative.
			var result = Math.Abs(magnitude);

			return BitConverter.DoubleToInt64Bits(sign) < 0 ? -result : result;
		}

		/// <summary>
		///		Random value between low and high inclusive
		/// </summary>
		public static double Random(double low, double high)
		{
			return low + _random.NextDouble() * (high - low);
		}

		private static Random _random = new Random();

		/// <summary>
		///		Random integer value between low and high inclusive
		/// </summary>
		public static int RandomInt(int low, int high)
		{
			if (high < low)
				(low, high) = (high, low);

			// Random.Next excludes its upper bound, Molang's math.random_integer includes it.
			return _random.Next(low, high == int.MaxValue ? high : high + 1);
		}

		/// <summary>
		///		Returns the sum of 'num' random numbers, each with a value from low to high
		/// </summary>
		public static double DieRoll(double num, double low, double high)
		{
			int i = 0;
			double total = 0;
			while (i++ < num) total += Random(low, high);

			return total;
		}

		/// <summary>
		///		Returns the sum of 'num' random integer numbers, each with a value from low to high
		/// </summary>
		public static int DieRollInt(int num, int low, int high)
		{
			int i = 0;
			int total = 0;
			while (i++ < num) total += RandomInt(low, high);

			return total;
		}

		/// <summary>
		///		Interpolate points on a smooth curve using the Hermite basis function <c>3t^2 - 2t^3</c>
		/// </summary>
		public static double HermiteBlend(double value)
		{
			return 3d * value * value - 2d * value * value * value;
		}

		/// <summary>
		///		Round value to the nearest integral number, rounding halves away from zero
		/// </summary>
		public static double Round(double value)
		{
			return Math.Round(value, MidpointRounding.AwayFromZero);
		}

		/// <summary>
		///		Returns 1 if value is positive, -1 otherwise
		/// </summary>
		public static double Sign(double value)
		{
			return value > 0d ? 1d : -1d;
		}

		/// <summary>
		///		Minimize angle magnitude (in degrees) into the range [-180, 180)
		/// </summary>
		public static double MinAngle(double value)
		{
			value = (value + 180d) % 360d;

			if (value < 0d)
				value += 360d;

			return value - 180d;
		}

		/// <summary>
		///		Computes the linear interpolation between start and end with weighting factor amount in [0, 1]
		/// </summary>
		public static double Lerp(double start, double end, double amount)
		{
			amount = Math.Max(0, Math.Min(1, amount));

			return start + (end - start) * amount;
		}

		/// <summary>
		///		Returns the normalized progress between start and end given value
		/// </summary>
		public static double InverseLerp(double start, double end, double value)
		{
			if (end == start)
				return 0d;

			return (value - start) / (end - start);
		}

		/// <summary>
		///		Computes a linear interpolation around a circle in the shortest direction from start to end degrees
		/// </summary>
		public static double LerpRotate(double start, double end, double amount)
		{
			start = Radify(start);
			end = Radify(end);

			if (start > end)
			{
				(start, end) = (end, start);
			}

			if (end - start > 180)
			{
				return Radify(end + amount * (360 - (end - start)));
			}

			return start + amount * (end - start);
		}

		/// <summary>
		///		Folds an angle in degrees into the range [0, 360)
		/// </summary>
		public static double Radify(double num)
		{
			return (((num + 180) % 360) + 180) % 360;
		}
	}

	/// <summary>
	///		The default Math implementations for the MoLang runtime
	/// </summary>
	public sealed class MoLangMathImpl
	{
		private static IMoStruct _instance;
		public static IMoStruct Library => _instance ??= new InteropStruct(new MoLangMathImpl());

		private MoLangMathImpl() { }

		[MoFunction("abs")]
		public double Abs(double value) => Math.Abs(value);

		[MoFunction("sin")]
		public double Sin(double value) => MoLangMath.Sin(value);

		[MoFunction("asin")]
		public double Asin(double value) => MoLangMath.Asin(value);

		[MoFunction("cos")]
		public double Cos(double value) => MoLangMath.Cos(value);

		[MoFunction("acos")]
		public double Acos(double value) => MoLangMath.Acos(value);

		[MoFunction("atan")]
		public double Atan(double value) => MoLangMath.Atan(value);

		[MoFunction("atan2")]
		public double Atan2(double y, double x) => MoLangMath.Atan2(y, x);

		[MoFunction("ceil")]
		public double Ceiling(double value) => Math.Ceiling(value);

		[MoFunction("clamp")]
		public double Clamp(double value, double min, double max) => MoLangMath.Clamp(value, min, max);

		[MoFunction("copy_sign")]
		public double CopySign(double magnitude, double sign) => MoLangMath.CopySign(magnitude, sign);

		[MoFunction("die_roll")]
		public double DieRoll(double num, double low, double high) => MoLangMath.DieRoll(num, low, high);

		[MoFunction("die_roll_integer")]
		public int DieRollInt(int num, int low, int high) => MoLangMath.DieRollInt(num, low, high);

		[MoFunction("exp")]
		public double Exp(double value) => Math.Exp(value);

		[MoFunction("mod")]
		public double Modulus(double x, double y) => x % y;

		[MoFunction("floor")]
		public double Floor(double value) => Math.Floor(value);

		[MoFunction("hermite_blend")]
		public double HermiteBlend(double value) => MoLangMath.HermiteBlend(value);

		[MoFunction("inverse_lerp")]
		public double InverseLerp(double start, double end, double value) =>
			MoLangMath.InverseLerp(start, end, value);

		[MoFunction("lerp")]
		public double Lerp(double start, double end, double amount) => MoLangMath.Lerp(start, end, amount);

		[MoFunction("lerprotate", "lerp_rotate")]
		public double LerpRotate(double start, double end, double amount) =>
			MoLangMath.LerpRotate(start, end, amount);

		[MoFunction("ln")]
		public double Log(double value) => Math.Log(value);

		[MoFunction("max")]
		public double Max(double value1, double value2) => Math.Max(value1, value2);

		[MoFunction("min")]
		public double Min(double value1, double value2) => Math.Min(value1, value2);

		[MoFunction("min_angle")]
		public double MinAngle(double value) => MoLangMath.MinAngle(value);

		[MoFunction("pi")]
		public double PiFunc() => Math.PI;

		[MoProperty("pi")] public double PI => Math.PI;

		[MoFunction("pow")]
		public double Pow(double x, double y) => Math.Pow(x, y);

		[MoFunction("random")]
		public double Random(double low, double high) => MoLangMath.Random(low, high);

		[MoFunction("random_integer")]
		public int RandomInt(int low, int high) => MoLangMath.RandomInt(low, high);

		[MoFunction("round")]
		public double Round(double value) => MoLangMath.Round(value);

		[MoFunction("sign")]
		public double Sign(double value) => MoLangMath.Sign(value);

		[MoFunction("sqrt")]
		public double Sqrt(double value) => Math.Sqrt(value);

		[MoFunction("trunc")]
		public double Truncate(double value) => Math.Truncate(value);

		[MoFunction("ease_in_sine")]
		public double EaseInSine(double s, double e, double t) => Ease(MoLangEasings.InSine, s, e, t);

		[MoFunction("ease_out_sine")]
		public double EaseOutSine(double s, double e, double t) => Ease(MoLangEasings.OutSine, s, e, t);

		[MoFunction("ease_in_out_sine")]
		public double EaseInOutSine(double s, double e, double t) => Ease(MoLangEasings.InOutSine, s, e, t);

		[MoFunction("ease_in_quad")]
		public double EaseInQuad(double s, double e, double t) => Ease(MoLangEasings.InQuad, s, e, t);

		[MoFunction("ease_out_quad")]
		public double EaseOutQuad(double s, double e, double t) => Ease(MoLangEasings.OutQuad, s, e, t);

		[MoFunction("ease_in_out_quad")]
		public double EaseInOutQuad(double s, double e, double t) => Ease(MoLangEasings.InOutQuad, s, e, t);

		[MoFunction("ease_in_cubic")]
		public double EaseInCubic(double s, double e, double t) => Ease(MoLangEasings.InCubic, s, e, t);

		[MoFunction("ease_out_cubic")]
		public double EaseOutCubic(double s, double e, double t) => Ease(MoLangEasings.OutCubic, s, e, t);

		[MoFunction("ease_in_out_cubic")]
		public double EaseInOutCubic(double s, double e, double t) => Ease(MoLangEasings.InOutCubic, s, e, t);

		[MoFunction("ease_in_quart")]
		public double EaseInQuart(double s, double e, double t) => Ease(MoLangEasings.InQuart, s, e, t);

		[MoFunction("ease_out_quart")]
		public double EaseOutQuart(double s, double e, double t) => Ease(MoLangEasings.OutQuart, s, e, t);

		[MoFunction("ease_in_out_quart")]
		public double EaseInOutQuart(double s, double e, double t) => Ease(MoLangEasings.InOutQuart, s, e, t);

		[MoFunction("ease_in_quint")]
		public double EaseInQuint(double s, double e, double t) => Ease(MoLangEasings.InQuint, s, e, t);

		[MoFunction("ease_out_quint")]
		public double EaseOutQuint(double s, double e, double t) => Ease(MoLangEasings.OutQuint, s, e, t);

		[MoFunction("ease_in_out_quint")]
		public double EaseInOutQuint(double s, double e, double t) => Ease(MoLangEasings.InOutQuint, s, e, t);

		[MoFunction("ease_in_expo")]
		public double EaseInExpo(double s, double e, double t) => Ease(MoLangEasings.InExpo, s, e, t);

		[MoFunction("ease_out_expo")]
		public double EaseOutExpo(double s, double e, double t) => Ease(MoLangEasings.OutExpo, s, e, t);

		[MoFunction("ease_in_out_expo")]
		public double EaseInOutExpo(double s, double e, double t) => Ease(MoLangEasings.InOutExpo, s, e, t);

		[MoFunction("ease_in_circ")]
		public double EaseInCirc(double s, double e, double t) => Ease(MoLangEasings.InCirc, s, e, t);

		[MoFunction("ease_out_circ")]
		public double EaseOutCirc(double s, double e, double t) => Ease(MoLangEasings.OutCirc, s, e, t);

		[MoFunction("ease_in_out_circ")]
		public double EaseInOutCirc(double s, double e, double t) => Ease(MoLangEasings.InOutCirc, s, e, t);

		[MoFunction("ease_in_back")]
		public double EaseInBack(double s, double e, double t) => Ease(MoLangEasings.InBack, s, e, t);

		[MoFunction("ease_out_back")]
		public double EaseOutBack(double s, double e, double t) => Ease(MoLangEasings.OutBack, s, e, t);

		[MoFunction("ease_in_out_back")]
		public double EaseInOutBack(double s, double e, double t) => Ease(MoLangEasings.InOutBack, s, e, t);

		[MoFunction("ease_in_elastic")]
		public double EaseInElastic(double s, double e, double t) => Ease(MoLangEasings.InElastic, s, e, t);

		[MoFunction("ease_out_elastic")]
		public double EaseOutElastic(double s, double e, double t) => Ease(MoLangEasings.OutElastic, s, e, t);

		[MoFunction("ease_in_out_elastic")]
		public double EaseInOutElastic(double s, double e, double t) => Ease(MoLangEasings.InOutElastic, s, e, t);

		[MoFunction("ease_in_bounce")]
		public double EaseInBounce(double s, double e, double t) => Ease(MoLangEasings.InBounce, s, e, t);

		[MoFunction("ease_out_bounce")]
		public double EaseOutBounce(double s, double e, double t) => Ease(MoLangEasings.OutBounce, s, e, t);

		[MoFunction("ease_in_out_bounce")]
		public double EaseInOutBounce(double s, double e, double t) => Ease(MoLangEasings.InOutBounce, s, e, t);

		private static double Ease(Func<double, double> curve, double start, double end, double amount)
		{
			return MoLangEasings.Interpolate(curve, start, end, amount);
		}
	}
}
