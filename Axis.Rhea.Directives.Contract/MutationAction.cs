namespace Axis.Rhea.Directives.Contract;

public enum MutationAction
{
    /// <summary>
    /// Adds the given data to the site(s) represented by the path selector. If data already exists there, report exception
    /// </summary>
    Append,

    /// <summary>
    /// Adds or updates the data to the site(s) represented by the path selector.
    /// </summary>
    Replace,

    /// <summary>
    /// Delete all data found at the site(s) represented by the path selector.
    /// </summary>
    Delete
}
