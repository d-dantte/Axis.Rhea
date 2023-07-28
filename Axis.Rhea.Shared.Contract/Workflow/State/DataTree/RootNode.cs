using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.Utils;
using Axis.Rhea.Shared.Contract.Workflow.State.DataPath;

namespace Axis.Rhea.Shared.Contract.Workflow.State.DataTree;

/// <summary>
/// Represents the root of the DataTree, ergo, the root <see cref="Ion.Types.IIonType"/> value to be pruned.
/// </summary>
public record RootNode : DataTreeNode, Pulsar.Grammar.Utils.IParsable<RootNode?>
{
    public RootNode(bool isRequired, params DataTreeNode[] children)
    : base(isRequired, children)
    {
    }

    public RootNode(params DataTreeNode[] children)
        : this(true, children)
    {
    }


    #region Pulsar.Grammar.Utils.IParsable<DataTreeNode>
    public static RootNode Parse(CSTNode node)
    {
        _ = TryParse(node, out IResult<RootNode> result);

        return result switch
        {
            IResult<RootNode>.DataResult data => data.Data,
            IResult<RootNode>.ErrorResult error => error.Cause().Throw<RootNode>(),
            _ => throw new InvalidOperationException($"Invalid result: {result}")
        };
    }

    public static bool TryParse(CSTNode node, out RootNode? value)
    {
        _ = TryParse(node, out IResult<RootNode> result);

        value = result switch
        {
            IResult<RootNode>.DataResult data => data.Data,
            _ => default
        };

        return !value.IsDefault();
    }

    public static RootNode Parse(string text, IFormatProvider? provider = null)
    {
        var recognitionResult = PulsarUtil.DataPathGrammar
            .GetRecognizer(PathsSymbol)
            .Recognize(text);

        return recognitionResult switch
        {
            SuccessResult success => Parse(success.Symbol),
            ErrorResult error => error.Exception.Throw<RootNode>(),
            FailureResult failure => throw new RecognitionException(failure),
            _ => throw new InvalidOperationException($"Invalid recognizer result: {recognitionResult}")
        };
    }

    public static bool TryParse(
        string? text,
        IFormatProvider? provider,
        out RootNode? result)
    {
        var parseResult = PulsarUtil.DataPathGrammar
            .GetRecognizer(PathsSymbol)
            .Recognize(text);

        result = parseResult switch
        {
            SuccessResult success => Parse(success.Symbol),
            _ => default
        };

        return !result.IsDefault();
    }

    public static bool TryParse(CSTNode pathsNode, out IResult<RootNode> result)
    {
        if (pathsNode is null)
            throw new ArgumentNullException(nameof(pathsNode));

        try
        {
            var node = pathsNode
                .FindNodes(PathSymbol)
                .Select(DataPathSegment.Parse)
                .Aggregate(DataTreeBuilder.Create(), (treeBuilder, segment) => treeBuilder.Combine(segment))
                .Build();

            result = Result.Of(node);
            return true;
        }
        catch(Exception e)
        {
            result = Result.Of<RootNode>(e);
            return false;
        }
    }
    #endregion
}
