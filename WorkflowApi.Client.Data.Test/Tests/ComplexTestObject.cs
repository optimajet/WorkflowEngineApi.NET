namespace WorkflowApi.Client.Data.Test.Tests;

public class ComplexTestObject
{
    public string Prop1 { get; set; } = string.Empty;
    public int Prop2 { get; set; }
    public NestedObject Nested { get; set; } = new();
}

public class NestedObject
{
    public bool SubProp1 { get; set; }
    public List<int> SubProp2 { get; set; } = new();
}