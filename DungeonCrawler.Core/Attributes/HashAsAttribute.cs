namespace DungeonCrawler.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class HashAsAttribute : Attribute {
	public String Value { get; }

	public HashAsAttribute(String value) {
		this.Value = value;
	}

	public HashAsAttribute(Type type) {
		this.Value = type.FullName;
	}
}