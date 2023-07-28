using Axis.Ion.Types;
using Axis.Luna.Common.Results;

namespace Axis.Rhea.Shared.Contract.StateExpressions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TIonType"></typeparam>
    public interface IConstantValueExpression<TIonType> : ITypedExpression<TIonType>
    where TIonType : struct, IIonType
    {
        TIonType Ion { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TIonType"></typeparam>
    public interface IValueSelectionExpression<TIonType> : IConstantValueExpression<TIonType>
    where TIonType : struct, IIonType
    {
        IResult<TIonType> Selection { get; }
    }
}
