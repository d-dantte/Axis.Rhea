using Axis.Luna.Extensions;
using System.Collections.Immutable;

namespace Axis.Rhea.Directives.Contract;

public partial record StateMutation
{
    public ImmutableList<IMutationInstruction> Instructions { get; }

    public StateMutation(params IMutationInstruction[] instructions)
    {
        Instructions = instructions
            .ThrowIfNull(new ArgumentNullException(nameof(instructions)))
            .ThrowIfAny(
                i => i is null,
                new ArgumentException("Null instruction found in the instructins list"))
            .ToImmutableList();
    }

}
