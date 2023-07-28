using Axis.Ion.Types;

namespace Axis.Rhea.Shared.Contract.StateExpressions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IExpression
    {
        IAtom Evaluate();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TIonType"></typeparam>
    public interface ITypedExpression<out TIonType>: IExpression
    where TIonType: struct, IIonType
    {
        new ITypedAtom<TIonType> Evaluate();
    }
}
