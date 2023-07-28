namespace Axis.Rhea.Shared.Contract.Directives.PolicyTriggers
{
    public interface ITriggerConditionGroup: IRetryPolicyTriggerCondition
    {
        IEnumerable<IRetryPolicyTriggerCondition> Conditions { get; }
    }
}
