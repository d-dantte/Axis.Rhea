using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.Directives.PolicyTriggers.Groups;
using Axis.Rhea.Shared.Contract.Utils;

namespace Axis.Rhea.Shared.Contract.Directives.PolicyTriggers;

public static class PolicyTriggerParser
{
    internal const string TriggerConditionSymbol = "trigger-condition";
    internal const string ConditionGroupSymbol = "condition-group";
    internal const string MatchExpressionSymbol = "match-expression";
    internal const string AndSymbol = "and";
    internal const string OrSymbol = "or";
    internal const string XorSymbol = "xor";
    internal const string NotSymbol = "not";
    internal const string StatusCodeExpSymbol = "status-code-exp";
    internal const string ResponseCodeExpSymbol = "response-code-exp";
    internal const string ErrorExpSymbol = "error-exp";
    internal const string FaultExpSymbol = "fault-exp";
    internal const string IntegerSymbol = "positive-integer";
    internal const string IdentifierSymbol = "identifier";
    internal const string TitleTextSymbol = "title-text";

    public static IResult<IRetryPolicyTriggerCondition> Parse(string text)
    {
        _ = TryParse(text, out var result);
        return result;
    }

    public static bool TryParse(string text, out IResult<IRetryPolicyTriggerCondition> result)
    {
        var recognitionResult = PulsarUtil.PolicyTriggerConditionGrammar
            .GetRecognizer(TriggerConditionSymbol)
            .Recognize(text);

        return recognitionResult switch
        {
            SuccessResult success => TryParse(success.Symbol, out result),

            ErrorResult error =>
                (result = Result.Of<IRetryPolicyTriggerCondition>(error.Exception)) != null,

            FailureResult failure =>
                (result = Result.Of<IRetryPolicyTriggerCondition>(new RecognitionException(failure))) != null,

            _ =>
                (result = Result.Of<IRetryPolicyTriggerCondition>(
                    new InvalidOperationException($"Invalid recognizer result: {recognitionResult}"))) != null
        };
    }

    private static bool TryParse(CSTNode node, out IResult<IRetryPolicyTriggerCondition> result)
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        try
        {
            result = Result.Of(ExtractTriggerCondition(node));
            return true;
        }
        catch(Exception e)
        {
            result = Result.Of<IRetryPolicyTriggerCondition>(e);
            return false;
        }
    }

    private static IRetryPolicyTriggerCondition ExtractTriggerCondition(CSTNode triggerConditionNode)
    {
        var query = $"{ConditionGroupSymbol}|{MatchExpressionSymbol}";
        var conditionNode = triggerConditionNode.FindNode(query);
        return conditionNode.SymbolName switch
        {
            ConditionGroupSymbol => ExtractConditionGroup(conditionNode),
            MatchExpressionSymbol => ExtractMatchExpression(conditionNode),
            _ => throw new InvalidOperationException($"Invalid symbol encountered: {conditionNode.SymbolName}")
        };
    }

    private static IRetryPolicyTriggerCondition ExtractConditionGroup(CSTNode conditionNode)
    {
        var group = conditionNode.FirstNode();
        return group.SymbolName switch
        {
            AndSymbol => group
                .FindNodes(TriggerConditionSymbol)
                .Select(ExtractTriggerCondition)
                .ToArray()
                .ApplyTo(conditions => new And(conditions)),

            OrSymbol => group
                .FindNodes(TriggerConditionSymbol)
                .Select(ExtractTriggerCondition)
                .ToArray()
                .ApplyTo(conditions => new Or(conditions)),

            XorSymbol => group
                .FindNodes(TriggerConditionSymbol)
                .Select(ExtractTriggerCondition)
                .ToArray()
                .ApplyTo(conditions => new Xor(conditions[0], conditions[1])),

            NotSymbol => group
                .FindNode(TriggerConditionSymbol)
                .ApplyTo(ExtractTriggerCondition)
                .ApplyTo(condition => new NotTrigger(condition)),

            _ => throw new InvalidOperationException($"Invalid symbol encountered: {conditionNode.SymbolName}")
        };
    }

    private static IRetryPolicyTriggerCondition ExtractMatchExpression(CSTNode conditionNode)
    {
        var group = conditionNode.FirstNode();
        return group.SymbolName switch
        {
            StatusCodeExpSymbol => group
                .FindNode(IntegerSymbol)
                .ApplyTo(node => node.TokenValue())
                .ApplyTo(int.Parse)
                .ApplyTo(code => new StatusCodeTrigger(code)),

            ResponseCodeExpSymbol => group
                .FindNode(IdentifierSymbol)
                .ApplyTo(node => node.TokenValue())
                .ApplyTo(code => new ResponseCodeTrigger(code)),

            ErrorExpSymbol => group
                .FindNode(TitleTextSymbol)
                .ApplyTo(node => node?.TokenValue().UnwrapFrom("'"))
                .ApplyTo(title => title is null
                    ? (IRetryPolicyTriggerCondition)ErrorPayloadTrigger.Instance
                    : new ErrorTitleTrigger(title)),

            FaultExpSymbol => group
                .FindNode(IdentifierSymbol)
                .ApplyTo(node => node?.TokenValue())
                .ApplyTo(type => type is null
                    ? (IRetryPolicyTriggerCondition)FaultPayloadTrigger.Instance
                    : new FaultTypeTrigger(type)),

            _ => throw new InvalidOperationException($"Invalid symbol encountered: {conditionNode.SymbolName}")
        };
    }
}
