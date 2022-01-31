using System.Reflection;
using Alex.MoLang.Runtime.Value;

namespace Alex.MoLang.Runtime.Struct
{
	public class PropertyAccessor : ValueAccessor
	{
		private PropertyInfo _propertyInfo;

		public PropertyAccessor(PropertyInfo propertyInfo)
		{
			_propertyInfo = propertyInfo;
		}

		/// <inheritdoc />
		public override IMoValue Get(object instance)
		{
			var value = _propertyInfo.GetValue(instance);

			return value is IMoValue moValue ? moValue : MoValue.FromObject(value);
		}

		/// <inheritdoc />
		public override void Set(object instance, IMoValue value)
		{
			if (!_propertyInfo.CanWrite)
				return;

			var propType = _propertyInfo.PropertyType;

			if (propType == typeof(double))
			{
				_propertyInfo.SetValue(instance, value.AsDouble());
				return;
			}
			else if (propType == typeof(float))
			{
				_propertyInfo.SetValue(instance, value.AsFloat());
				return;
			}
			else if (propType == typeof(bool))
			{
				_propertyInfo.SetValue(instance, value.AsBool());
				return;
			}
			else if (propType == typeof(string))
			{
				_propertyInfo.SetValue(instance, value.AsString());
				return;
			}
			
			_propertyInfo.SetValue(instance, value);
		}
	}
}