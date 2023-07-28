using Axis.Luna.Extensions;
using Axis.Rhea.Directives.Contract;
using Axis.Rhea.Directives.Contract.Responses;

namespace Axis.Rhea.Shared.Contract.Directives.PolicyTriggers
{
    /// <summary>
    /// Triggers a retry if the <see cref="ErrorPayload.Title"/> matches the given <see cref="ErrorTitleTrigger.ErrorTitle"/>
    /// </summary>
    public record ErrorTitleTrigger: IRetryPolicyTriggerCondition
    {
        /// <summary>
        /// The expected error title
        /// </summary>
        public string ErrorTitle { get; }

        public ErrorTitleTrigger(string errorTitle)
        {
            ErrorTitle = errorTitle.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(errorTitle)}: '{errorTitle}'"));
        }

        public bool IsMatch(IResponsePayload payload)
        {
            if (payload is null)
                throw new ArgumentNullException(nameof(payload));

            return payload switch
            {
                ErrorPayload error => ErrorTitle.Equals(error.Title),
                _ => false
            };
        }
    }
}
