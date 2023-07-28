using Axis.Ion.Types;
using Axis.Luna.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Value
{
    public record TimelineEventSelectionValuee<TIonType> : IValueSelectionExpression<TIonType>
    where TIonType : struct, IIonType
    {
        public IResult<TIonType> Selection => throw new NotImplementedException();

        public TIonType Ion => throw new NotImplementedException();

        public ITypedAtom<TIonType> Evaluate()
        {
            throw new NotImplementedException();
        }

        IAtom IExpression.Evaluate()
        {
            throw new NotImplementedException();
        }
    }
}
