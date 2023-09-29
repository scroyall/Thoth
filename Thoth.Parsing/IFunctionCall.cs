namespace Thoth.Parsing;

/// <summary>
/// Common interface for function call statements and expressions.
/// </summary>
public interface IFunctionCall
{
    /// <summary>
    /// Name of the function to call.
    /// </summary>
    public string Name { get; }
}