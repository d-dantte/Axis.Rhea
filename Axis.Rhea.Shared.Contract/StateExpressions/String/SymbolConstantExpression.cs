using Axis.Ion.Types;

namespace Axis.Rhea.Shared.Contract.StateExpressions.String
{
    public record SymbolConstantExpression : IConstantValueExpression<IonIdentifier>
    {
        public IonIdentifier Ion => throw new NotImplementedException();

        public ITypedAtom<IonIdentifier> Evaluate()
        {
            throw new NotImplementedException();
        }

        IAtom IExpression.Evaluate()
        {
            throw new NotImplementedException();
        }
    }
}
