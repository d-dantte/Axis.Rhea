using Axis.Luna.Extensions;
using System;
using System.Collections.Immutable;

namespace Axis.Rhea.Core.Workflow.Activities
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceInvocation: IActivity
    {
        /// <summary>
        /// The ID of the <see cref="Workflow.ServiceInvocation.IServiceInvocation"/> to be used.
        /// </summary>
        public string InvocationId { get; }

        public string Name { get; }

        /// <summary>
        /// A map of <see cref="Workflow.ServiceInvocation.InvocationResponse.ResponseIdentifier"/> to <see cref="IActivity.Name"/>.
        /// <para>
        /// This table tells what activity to transition to based on the returned response identifier.
        /// </para>
        /// </summary>
        public ImmutableDictionary<string, string> TransitionTable { get; }

        public ServiceInvocation(string invocationId, string name, params (string ResponseIdentifier, string ActivityName)[] transitionTable)
        {
            InvocationId = invocationId.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(invocationId)}: '{invocationId}'"));

            Name = name.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(name)}: '{name}'"));

            TransitionTable = transitionTable
                .ThrowIfNull(new ArgumentNullException(nameof(transitionTable)))
                .ThrowIfAny(
                    tuple => string.IsNullOrWhiteSpace(tuple.ResponseIdentifier) || string.IsNullOrWhiteSpace(tuple.ActivityName),
                    tuple => new ArgumentException($"Invalid transition pair: {tuple}"))
                .ToImmutableDictionary(
                    pair => pair.ResponseIdentifier,
                    pair => pair.ActivityName);
        }
    }
}
