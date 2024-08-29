namespace WorkflowApi.Exceptions;

/// <summary>
/// An exception thrown when an instance of a class is not initialized.
/// </summary>
public class NotInitializedException :  NullReferenceException
{
    public NotInitializedException(string name) : base($"Instance of class {name} not initialized.") {}
}