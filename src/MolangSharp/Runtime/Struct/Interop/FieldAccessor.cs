using System;
using System.Reflection;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Runtime.Struct.Interop
{
	public class FieldAccessor : ValueAccessor
	{
		private readonly FieldInfo _fieldInfo;
		private readonly Func<object, IMoValue> _getter;
		private readonly Action<object, object> _setter;

		public FieldAccessor(FieldInfo fieldInfo)
		{
			_fieldInfo = fieldInfo;
			_getter = AccessorCompiler.CompileGetter(fieldInfo, fieldInfo.FieldType);

			if (!fieldInfo.IsInitOnly)
				_setter = AccessorCompiler.CompileSetter(fieldInfo, fieldInfo.FieldType);
		}

		public override bool CanRead => true;
		public override bool CanWrite => !_fieldInfo.IsInitOnly;

		/// <inheritdoc />
		public override IMoValue Get(object instance)
		{
			if (_getter != null)
				return _getter(instance);

			var value = _fieldInfo.GetValue(instance);

			return value is IMoValue moValue ? moValue : MoValue.FromObject(value);
		}

		/// <inheritdoc />
		public override void Set(object instance, IMoValue value)
		{
			if (_setter != null)
				_setter(instance, value);
			else
				_fieldInfo.SetValue(instance, value);

			InvokeChanged();
		}
	}
}
