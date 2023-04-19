using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using System;
using System.Linq;

namespace Axis.Rhea.Core.Workflow.State
{
    internal static class ExpressionTransformer
    {
        internal const string RelationalPredicate = "relational-predicate";

        internal const string LogicalXorExpression = "logical-xor-expression";
        internal const string LogicalNorExpression = "logical-nor-expression";
        internal const string LogicalAndExpression = "logical-and-expression";

        internal const string RelationalExpression = "relational-expression";
        internal const string RelationalOperator = "relational-operator";
        internal const string RelationalRhs = "relational-rhs";

        internal const string RangeExpression = "range-expression";
        internal const string ListExpression = "list-expression";
        internal const string TypeExpression = "type-expression";

        internal const string ArithmeticExpression = "arithmetic-expression";

        internal const string TransformationExpression = "transformation-expression";
        internal const string Transformation = "transformation";

        internal const string VariableExpression = "variable-expression";
        internal const string ConstantExpression = "constant-expression";
        internal const string ExpressionSymbol = "expression";

        internal const string NowConstant = "now";


        internal static CSTNode TransformExpression(CSTNode expression)
        {
            return expression.SymbolName switch
            {
                LogicalNorExpression => TransformLogicalNor(expression),
                LogicalXorExpression => TransformLogicalXor(expression),
                RelationalExpression => TransformRelationalExpression(expression),
                TransformationExpression => TransformTransformationExpression(expression),
                VariableExpression => TransformVariableExpression(expression),
                NowConstant => TransformNowConstant(),
                _ => expression switch
                {
                    CSTNode.LeafNode leaf => CSTNode.Of(leaf.SymbolName, leaf.Tokens),
                    CSTNode.BranchNode branch => CSTNode.Of(
                        branch.SymbolName,
                        branch.Nodes.Select(TransformExpression).ToArray()),
                    _ => throw new ArgumentException($"Invalid expression node: {expression}")
                }
            };
        }

        internal static CSTNode TransformLogicalXor(CSTNode logicalXorExpression)
        {
            var operand1 = logicalXorExpression
                .ThrowIfNull(new ArgumentNullException(nameof(logicalXorExpression)))
                .FindNode(LogicalNorExpression);

            var operand2 = logicalXorExpression.FindNode(ExpressionSymbol);

            if (operand2 is null)
                return TransformExpression(operand1);

            return CSTNode.Of(
                "transformed-nor-expression",
                CSTNode.Of("open-bracket", "("),
                TransformExpression(operand1),
                CSTNode.Of("nor-method", ").Xor("),
                TransformExpression(operand2),
                CSTNode.Of("close-bracket", ")"));
        }

        internal static CSTNode TransformLogicalNor(CSTNode logicalNorExpression)
        {
            var operand1 = logicalNorExpression
                .ThrowIfNull(new ArgumentNullException(nameof(logicalNorExpression)))
                .FindNode(LogicalAndExpression);

            var operand2 = logicalNorExpression.FindNode(ExpressionSymbol);

            if (operand2 is null)
                return TransformExpression(operand1);

            return CSTNode.Of(
                "transformed-nor-expression",
                CSTNode.Of("open-bracket", "("),
                TransformExpression(operand1),
                CSTNode.Of("nor-method", ").Nor("),
                TransformExpression(operand2),
                CSTNode.Of("close-bracket", ")"));
        }

        internal static CSTNode TransformRelationalExpression(CSTNode relationalExpression)
        {
            var rhs = relationalExpression
                .ThrowIfNull(new ArgumentNullException(nameof(relationalExpression)))
                .FindNode($"{RelationalRhs}.{RangeExpression}|{ListExpression}|{TypeExpression}");

            var @operator = relationalExpression.FindNode(RelationalOperator);

            if (rhs is null)
            {
                return @operator?.TokenValue() switch
                {
                    "=" => CSTNode.Of(
                        "transformed-equality-expression",
                        TransformExpression(relationalExpression.FindNode(ArithmeticExpression)),
                        CSTNode.Of("transformed-equality-operator", " == "),
                        TransformExpression(relationalExpression.FindNode($"{RelationalRhs}.{ExpressionSymbol}"))),

                    "matches" => CSTNode.Of(
                        "transformed-matches-expression",
                        CSTNode.Of("open-bracket", "("),
                        TransformExpression(relationalExpression.FindNode(ArithmeticExpression)),
                        CSTNode.Of("matches-method", ").Matches("),
                        TransformExpression(relationalExpression.FindNode($"{RelationalRhs}.{ExpressionSymbol}")),
                        CSTNode.Of("close-bracket", ")")),

                    "starts with" => CSTNode.Of(
                        "transformed-starts-with-expression",
                        CSTNode.Of("open-bracket", "("),
                        TransformExpression(relationalExpression.FindNode(ArithmeticExpression)),
                        CSTNode.Of("starts-with-method", $").{GetRelationalFunction(@operator)}("),
                        TransformExpression(relationalExpression.FindNode($"{RelationalRhs}.{ExpressionSymbol}")),
                        CSTNode.Of("close-bracket", ")")),

                    "ends with" => CSTNode.Of(
                        "transformed-ends-with-expression",
                        CSTNode.Of("open-bracket", "("),
                        TransformExpression(relationalExpression.FindNode(ArithmeticExpression)),
                        CSTNode.Of("ends-with-method", $").{GetRelationalFunction(@operator)}("),
                        TransformExpression(relationalExpression.FindNode($"{RelationalRhs}.{ExpressionSymbol}")),
                        CSTNode.Of("close-bracket", ")")),

                    _ => relationalExpression
                        .As<CSTNode.BranchNode>().Nodes
                        .Select(TransformExpression)
                        .ApplyTo(nodes => CSTNode.Of("transformed-relational-expression", nodes.ToArray()))
                };
            }

            return rhs.SymbolName switch
            {
                RangeExpression => CSTNode.Of(
                    "transformed-range-expression",
                    CSTNode.Of("open-bracket", "("),
                    TransformExpression(relationalExpression.FindNode(ArithmeticExpression)),
                    CSTNode.Of("between-method", $").{GetRelationalFunction(@operator)}("),
                    TransformExpression(rhs.FindNodes(ExpressionSymbol).First()),
                    CSTNode.Of("comma", ", "),
                    TransformExpression(rhs.FindNodes(ExpressionSymbol).Last()),
                    CSTNode.Of("close-bracket", ")")),

                ListExpression =>  CSTNode.Of(
                    "transformed-list-expression",
                    CSTNode.Of("open-bracket", "("),
                    TransformExpression(relationalExpression.FindNode(ArithmeticExpression)),
                    CSTNode.Of("in-method", $").{GetRelationalFunction(@operator)}("),
                    CSTNode.Of("param-list", rhs
                        .FindNodes(ExpressionSymbol)
                        .Select(TransformExpression)
                        .StuffUsing(CSTNode.Of("comma", ", "))
                        .ToArray()),
                    CSTNode.Of("close-bracket", ")")),

                TypeExpression => CSTNode.Of(
                    "transformed-type-expression",
                    CSTNode.Of("open-bracket", "("),
                    TransformExpression(relationalExpression.FindNode(ArithmeticExpression)),
                    CSTNode.Of("type-method", $").{GetRelationalFunction(@operator)}(typeof(Ion{rhs.TokenValue()}))")),

                _ => throw new ArgumentException($"Invalid relation-expression rhs symbol: {rhs.SymbolName}")
            };
        }

        internal static CSTNode TransformTransformationExpression(CSTNode transformationExpression)
        {
            if (transformationExpression is null)
                throw new ArgumentNullException(nameof(transformationExpression));

            var transformations = transformationExpression
                .FindNodes(Transformation)
                .Select(node => $"{GetTransformFunction(node)}()")
                .JoinUsing(".");

            var lhsSymbols = $"{VariableExpression}|{ConstantExpression}|{ExpressionSymbol}";
            var exps = transformationExpression
                .FindNodes(lhsSymbols)
                .Select(TransformExpression);

            if (!string.IsNullOrEmpty(transformations))
                exps = exps.Append(CSTNode.Of("transforms", $".{transformations}"));

            return CSTNode.Of(
                "transformed-transform-expression",
                exps.ToArray());
        }

        internal static CSTNode TransformTimeSpanExpression(CSTNode timespanExpression)
        {
            if (timespanExpression is null)
                throw new ArgumentNullException(nameof(timespanExpression));

            return CSTNode.Of(
                "transformed-timespan-expression",
                CSTNode.Of("timespan-parse", "TimeSpan.Parse(\""),
                CSTNode.Of("timespan", timespanExpression.TokenValue()),
                CSTNode.Of("timespan-parse-close", "\")"));
        }

        internal static CSTNode TransformVariableExpression(CSTNode variableExpression)
        {
            if (variableExpression is null)
                throw new ArgumentNullException(nameof(variableExpression));

            return CSTNode.Of("transformed-now-constant", variableExpression.TokenValue() switch
            {
                "#" => "Key",
                "$" => "Value",
                _ => throw new ArgumentException($"Invalid variable token: {variableExpression.TokenValue()}")
            });
        }

        internal static CSTNode TransformNowConstant()
        {
            return CSTNode.Of("transformed-now-constant", "DateTimeOffset.Now");
        }

        private static string GetRelationalFunction(CSTNode node)
        {
            return node.TokenValue() switch
            {
                "in" => "In",
                "not in" => "NotIn",
                "between" => "Between",
                "not between" => "NotBetween",
                "is" => "IsType",
                "is not" => "IsNotType",
                "starts with" => "StartsWith",
                "ends with" => "EndsWith",
                "matches" => "Matches",
                _ => throw new ArgumentException($"Invalid range operator: {node.TokenValue()}")
            };
        }

        private static string GetTransformFunction(CSTNode node)
        {
            return node.TokenValue() switch
            {
                "to-lower" => "ToLower",
                "to-upper" => "ToUpper",
                "negation" => "Negation",
                "reverse" => "Reverse",
                "count" => "Count",
                "is-null" => "IsNull",
                "is-not-null" => "IsNotNull",
                _ => throw new ArgumentException($"Invalid transform operator: {node.TokenValue()}")
            };
        }

    }
}
