using Axis.Dia.Core.Contracts;
using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract
{
    public record Instruction : IValidatable
    {
        required public Dia.PathQuery.Path Path { get; init; }

        required public Type ActionType { get; init; }

        required public IDiaValue? Data { get; init; }

        public bool TryValidate(out ValidationResult[] validationException)
        {
            var errors = new List<ValidationResult>();

            if (Path is null)
                errors.Add(new ValidationResult($"'{nameof(Path)}' is null"));

            if (!Enum.IsDefined(ActionType))
                errors.Add(new ValidationResult($"'{nameof(ActionType)}' is not a defined enum"));

            if (Type.Modify.Equals(ActionType) && Data is null)
                errors.Add(new ValidationResult(
                    $"Non-null '{nameof(Data)}' expected when '{nameof(ActionType)}' is {Type.Modify}"));

            validationException = [.. errors];
            return validationException.IsEmpty();
        }

        #region Nested types
        public enum Type
        {
            Modify,
            Remove,
        }
        #endregion
    }
}
