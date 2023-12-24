namespace DungeonCrawler.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class HashAsAttribute : Attribute {
	public string Value { get; }

	public HashAsAttribute(string value) {
		this.Value = value;
	}

	public HashAsAttribute(Type type) {
		this.Value = type.FullName;
	}
}