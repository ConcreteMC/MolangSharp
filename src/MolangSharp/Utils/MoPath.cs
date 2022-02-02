namespace ConcreteMC.MolangSharp.Utils
{
	public class MoPath
	{
		public MoPath Root { get; }
		public MoPath Next { get; private set; }

		public string Path { get; }
		public string Value { get; private set; }

		public bool HasChildren => Next != null;

		public MoPath(string path)
		{
			Next = null;
			Root = this;
			Path = path;

			var segments = path.Split('.');
			Value = segments[0];

			MoPath current = this;
			if (segments.Length > 1)
			{
				string currentPath = $"{Value}";

				for (int i = 1; i < segments.Length; i++)
				{
					var value = segments[i];

					if (string.IsNullOrWhiteSpace(value))
						break;

					currentPath += $".{value}";

					var moPath = new MoPath(Root, currentPath, value);
					current.Next = moPath;
					current = moPath;
				}
			}
		}

		internal MoPath(MoPath root,string path, string value)
		{
			Root = root;
			Path = path;
			Value = value;
		}

		internal void SetValue(string value)
		{
			Value = value;
		}
		
		/// <inheritdoc />
		public override string ToString()
		{
			return Value;
		}
	}
}