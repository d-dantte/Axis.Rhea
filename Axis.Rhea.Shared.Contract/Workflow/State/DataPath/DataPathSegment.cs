using Axis.Ion.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.Exceptions;
using Axis.Rhea.Shared.Contract.Utils;

namespace Axis.Rhea.Shared.Contract.Workflow.State.DataPath;

/// <summary>
/// This represents a single, non-branching path through the ion data that yields a single ion value.
/// takes the form
/// <code>
/// /abc/xyz/[5]/other-stuff
/// </code>
/// </summary>
public abstract record DataPathSegment : Pulsar.Grammar.Utils.IParsable<DataPathSegment?>
{
    internal const string PathSymbol = "path";
    internal const string PathSegmentSymbol = "path-segment";
    private const string PropertySymbol = "property";
    private const string IndexSymbol = "index";

    /// <summary>
    /// The next node in the path, or null
    /// </summary>
    public DataPathSegment? Next { get; }

    protected DataPathSegment(DataPathSegment? next = null)
    {
        Next = next;
    }

    /// <summary>
    /// Given an <see cref="IIonType"/>, select the element that matches the selector of the current segment
    /// </summary>
    /// <param name="ion"></param>
    /// <returns></returns>
    public abstract IDataPathSelection Select(IIonType ion);

    public IEnumerable<DataPathSegment> Segments()
    {
        var segment = this;
        do
        {
            yield return segment;
        }
        while ((segment = segment.Next) != null);
    }

    #region Pulsar.Grammar.Utils.IParsable<DataPathNode>
    public static DataPathSegment Parse(CSTNode node)
    {
        _ = TryParse(node, out IResult<DataPathSegment> result);

        return result switch
        {
            IResult<DataPathSegment>.DataResult data => data.Data,
            IResult<DataPathSegment>.ErrorResult error => error.Cause().Throw<DataPathSegment>(),
            _ => throw new InvalidOperationException($"Invalid result: {result}")
        };
    }

    public static bool TryParse(CSTNode node, out DataPathSegment? value)
    {
        _ = TryParse(node, out IResult<DataPathSegment> result);

        value = result switch
        {
            IResult<DataPathSegment?>.DataResult data => data.Data,
            _ => default
        };

        return !value.IsDefault();
    }

    public static DataPathSegment Parse(string text, IFormatProvider? provider = null)
    {
        var recognitionResult = PulsarUtil.DataPathGrammar
            .GetRecognizer(PathSymbol)
            .Recognize(text);

        return recognitionResult switch
        {
            SuccessResult success => Parse(success.Symbol),
            ErrorResult error => error.Exception.Throw<DataPathSegment>(),
            FailureResult failure => throw new RecognitionException(failure),
            _ => throw new InvalidOperationException($"Invalid recognizer result: {recognitionResult}")
        };
    }

    public static bool TryParse(
        string? text,
        IFormatProvider? provider,
        out DataPathSegment? result)
    {
        var parseResult = PulsarUtil.DataPathGrammar
            .GetRecognizer(PathSymbol)
            .Recognize(text);

        result = parseResult switch
        {
            SuccessResult success => Parse(success.Symbol),
            _ => default
        };

        return !result.IsDefault();
    }

    public static bool TryParse(CSTNode pathDefinitionNode, out IResult<DataPathSegment> result)
    {
        if (pathDefinitionNode is null)
            throw new ArgumentNullException(nameof(pathDefinitionNode));

        try
        {
            var selectors = $"{PathSegmentSymbol}.{PropertySymbol}|{IndexSymbol}";
            var pathNode = pathDefinitionNode
                .FindNodes(selectors)
                .Reverse()
                .Aggregate(default(DataPathSegment?), (child, cstnode) =>
                {
                    return cstnode.TokenValue().ApplyTo(tv => cstnode.SymbolName switch
                    {
                        PropertySymbol => new PropertySegment(
                            next: child,
                            isRequired: !tv.EndsWith("?"),
                            property: tv
                                .TrimEnd("?")
                                .UnwrapFrom("'")
                                .ApplyTo(selector => selector.Equals("*") ? null : selector)),

                        IndexSymbol => new ItemSegment(
                            next: child,
                            isRequired: !tv.EndsWith("?"),
                            index: tv
                                .TrimEnd("?")
                                .UnwrapFrom("[", "]")
                                .ApplyTo(selector => selector.Equals("*") ? default(int?) : int.Parse(selector))),

                        _ => $"Invalid Selector symbol: {cstnode.SymbolName}"
                            .ApplyTo(message => new DataPathParserException(message))
                            .Throw<DataPathSegment>()
                    });
                })
                ?? throw new DataPathParserException("No paths found");

            result = Result.Of(pathNode);
            return true;
        }
        catch(Exception e)
        {
            result = Result.Of<DataPathSegment>(e);
            return false;
        }
    }
    #endregion
}
