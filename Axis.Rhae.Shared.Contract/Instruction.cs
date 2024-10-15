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

        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (Path is null)
                errors.Add(new ValidationResult($"Invalid '{nameof(Path)}': null"));

            if (!Enum.IsDefined(ActionType))
                errors.Add(new ValidationResult($"Invalid '{nameof(ActionType)}': not a defined enum"));

            if (Type.Modify.Equals(ActionType) && Data is null)
                errors.Add(new ValidationResult(
                    $"Invalid '{nameof(Data)}': non-null expected when '{nameof(ActionType)}' is {Type.Modify}"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
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
