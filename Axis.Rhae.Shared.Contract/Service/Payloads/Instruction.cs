using Axis.Luna.Common;
using Axis.Luna.Extensions;

namespace Axis.Rhae.Contract.Service.Payloads
{
    public enum InstructionType
    {
        Modify,
        Remove
    }

    public readonly struct Instruction :
        IDefaultValueProvider<Instruction>
    {
        public InstructionType Type { get; }

        public Dia.PathQuery.Path Path { get; }

        public bool IsDefault => Path is null;

        public static Instruction Default => default;

        public Instruction(
            InstructionType type,
            Dia.PathQuery.Path path)
        {
            ArgumentNullException.ThrowIfNull(path);

            Type = type.ThrowIf(
                t => !Enum.IsDefined(t),
                t => new ArgumentException($"Invalid {nameof(type)}: undefined enum"));
            Path = path;
        }
    }
}
