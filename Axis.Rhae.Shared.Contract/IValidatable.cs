using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract
{
    public interface IValidatable
    {
        /// <summary>
        /// Validates the entity, returning false and any errors detected, if presenet, else returning true.
        /// </summary>
        /// <param name="validationResults"></param>
        /// <returns>True if valid, false otherwise</returns>
        bool TryValidate(out ValidationResult[] validationResults);
    }

    public static class ValidatableExtensions
    {
        /// <summary>
        /// Validates the entity, throwing an <see cref="AggregateException"/> if errors are detected
        /// </summary>
        public static void Validate(this IValidatable validatable)
        {
            ArgumentNullException.ThrowIfNull(validatable);

            if (!validatable.IsValid(out var errors))
                throw errors
                    .Select(error => new validationResults(error.ErrorMessage))
                    .ApplyTo(_errors => new AggregateException(_errors.ToArray()));
        }
    }
}
