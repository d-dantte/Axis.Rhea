using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Rhea.Core.Serializers
{
    public interface ISerializableEntity<TEntity>
    where TEntity: IParsable<TEntity>
    {
        string Serialize();
    }
}
