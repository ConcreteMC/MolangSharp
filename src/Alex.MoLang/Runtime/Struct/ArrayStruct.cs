using System;
using System.Collections.Generic;
using System.Linq;
using Alex.MoLang.Runtime.Exceptions;
using Alex.MoLang.Runtime.Value;
using Alex.MoLang.Utils;

namespace Alex.MoLang.Runtime.Struct
{
	/// <summary>
	///		Represents an array of values
	/// </summary>
	public class ArrayStruct : IMoStruct
	{
		private IMoValue[] _array;

		public ArrayStruct()
		{
			_array = new IMoValue[0];
		}

		public ArrayStruct(IEnumerable<IMoValue> values)
		{
			_array = values.ToArray();
			//_array.AddRange(values);
		}

		private void Resize(int size)
		{
			if (size >= _array.Length)
				Array.Resize(ref _array, size + 1);
		}
		
		public IMoValue this[int index]
		{
			get
			{
				if (index >= _array.Length)
					return DoubleValue.Zero;
				//Resize(index);
				return _array[index % _array.Length];
			}
			set
			{
				if (_array.Length == 0)
					return;
				//Resize(index);
				
				_array[index % _array.Length] = value;
			}
		}

		public void Set(MoPath key, IMoValue value)
		{
			if (int.TryParse(key.Value, out int index))
			{
				this[index] = value;
			}
		}

		public IMoValue Get(MoPath key, MoParams parameters)
		{
			if (int.TryParse(key.Value, out int index))
			{
				return this[index];
			}

			throw new MoLangRuntimeException($"Invalid path for array access: {key.Path.ToString()}", null);
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public object Value => _array;
	}

	public class VariableArrayStruct : VariableStruct
	{
		protected override IMoStruct CreateNew()
		{
			return new ArrayStruct();
		}
	}
}