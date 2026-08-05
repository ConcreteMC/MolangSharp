using System;
using System.Reflection;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Runtime.Struct.Interop
{
	public class PropertyAccessor : ValueAccessor
	{
		private readonly PropertyInfo _propertyInfo;
		private readonly Func<object, IMoValue> _getter;
		private readonly Action<object, object> _setter;

		public PropertyAccessor(PropertyInfo propertyInfo)
		{
			_propertyInfo = propertyInfo;

			if (propertyInfo.CanRead)
				_getter = AccessorCompiler.CompileGetter(propertyInfo, propertyInfo.PropertyType);

			if (propertyInfo.CanWrite)
				_setter = AccessorCompiler.CompileSetter(propertyInfo, propertyInfo.PropertyType);
		}

		public override bool CanRead => _propertyInfo.CanRead;
		public override bool CanWrite => _propertyInfo.CanWrite;

		/// <inheritdoc />
		public override IMoValue Get(object instance)
		{
			if (_getter != null)
				return _getter(instance);

			var value = _propertyInfo.GetValue(instance);

			return value is IMoValue moValue ? moValue : MoValue.FromObject(value);
		}

		/// <inheritdoc />
		public override void Set(object instance, IMoValue value)
		{
			var propType = _propertyInfo.PropertyType;

			if (propType == typeof(double))
			{
				Write(instance, value.AsDouble());

				return;
			}
			else if (propType == typeof(float))
			{
				Write(instance, value.AsFloat());

				return;
			}
			else if (propType == typeof(bool))
			{
				Write(instance, value.AsBool());

				return;
			}
			else if (propType == typeof(string))
			{
				Write(instance, value.AsString());

				return;
			}

			Write(instance, value);
			InvokeChanged();
		}

		private void Write(object instance, object value)
		{
			if (_setter != null)
			{
				_setter(instance, value);

				return;
			}

			_propertyInfo.SetValue(instance, value);
		}
	}
}
