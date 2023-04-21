using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Core.Workflow.State.DataTree;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Rhea.Core.Workflow.State.DataPath
{
    /// <summary>
    /// This represents a single, non-branching path through the ion data that yields a single ion value.
    /// takes the form
    /// <code>
    /// /abc/xyz/[5]/other-stuff
    /// </code>
    /// </summary>
    public abstract record DataPathNode : Pulsar.Grammar.Utils.IParsable<DataPathNode>
    {
        internal static readonly string PathDefinitionSymbol = "path-definition";

        /// <summary>
        /// The next node in the path, or null
        /// </summary>
        public DataPathNode Next { get; }

        protected DataPathNode(DataPathNode next = null)
        {
            Next = next;
        }

        /// <summary>
        /// Given an <see cref="IIonType"/>, select the element that matches the selector of the given node
        /// </summary>
        /// <param name="ion"></param>
        /// <returns></returns>
        public abstract IIonType Select(IIonType ion);

        #region Pulsar.Grammar.Utils.IParsable<DataPathNode>
        public static DataPathNode Parse(CSTNode node)
        {
            _ = TryParse(node, out IResult<DataPathNode> result);

            return result switch
            {
                IResult<DataPathNode>.DataResult data => data.Data,
                IResult<DataPathNode>.ErrorResult error => error.Cause().Throw<DataPathNode>(),
                _ => throw new InvalidOperationException($"Invalid result: {result}")
            };
        }

        public static bool TryParse(CSTNode node, out DataPathNode value)
        {
            _ = TryParse(node, out IResult<DataPathNode> result);

            value = result switch
            {
                IResult<DataPathNode>.DataResult data => data.Data,
                _ => default
            };

            return !value.IsDefault();
        }

        public static DataPathNode Parse(string text, IFormatProvider provider = null)
        {
            var recognitionResult = PulsarUtil.DataTreeGrammar
                .GetRecognizer(PathDefinitionSymbol)
                .Recognize(text);

            return recognitionResult switch
            {
                SuccessResult success => Parse(success.Symbol),
                ErrorResult error => error.Exception.Throw<DataPathNode>(),
                FailureResult failure => throw new RecognitionException(failure),
                _ => throw new InvalidOperationException($"Invalid recognizer result: {recognitionResult}")
            };
        }

        public static bool TryParse(
            [NotNullWhen(true)] string text,
            IFormatProvider provider,
            [MaybeNullWhen(false)] out DataPathNode result)
        {
            var parseResult = PulsarUtil.DataTreeGrammar
                .GetRecognizer(PathDefinitionSymbol)
                .Recognize(text);

            result = parseResult switch
            {
                SuccessResult success => Parse(success.Symbol),
                _ => default
            };

            return !result.IsDefault();
        }

        public static bool TryParse(CSTNode node, out IResult<DataPathNode> result)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    /// <summary>
    /// Interface that presents a property used in selecting elements from <see cref="IIonType"/>s.
    /// </summary>
    /// <typeparam name="TSelectorType"></typeparam>
    public interface IIonSelector<TSelectorType>
    {
        /// <summary>
        /// The selector value
        /// </summary>
        TSelectorType Selector { get; }
    }
}
