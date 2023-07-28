using Axis.Ion.Types;
using System.Collections.Immutable;

namespace Axis.Rhea.Shared.Contract.StateExpressions
{
    public interface IUnaryOperation<out TIonType> : ITypedExpression<TIonType>
    where TIonType : struct, IIonType
    {
        string Name { get; }

        IExpression Subject { get; }
    }

    public interface IBinaryOperation<out TIonType> : ITypedExpression<TIonType>
    where TIonType : struct, IIonType
    {
        string Name { get; }

        IExpression Subject { get; }

        IExpression Object { get; }
    }

    public interface IParametarizedOperation<out TIonType> : ITypedExpression<TIonType>
    where TIonType : struct, IIonType
    {
        string Name { get; }

        IReadOnlyList<IExpression> Arguments { get; }
    }
}
