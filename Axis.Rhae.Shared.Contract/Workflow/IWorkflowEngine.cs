using Axis.Rhae.Contract.Workflow.Identifiers;
using Axis.Rhae.Contract.Workflow.Requests;
using Axis.Rhae.Contract.Workflow.Responses;

namespace Axis.Rhae.Contract.Workflow
{
    /// <summary>
    /// Contract for the workflow engine.
    /// Exposes methods like starting, stopping, pausing, retrieving, etc, information about workflows
    /// </summary>
    public interface IWorkflowEngine
    {
        /// <summary>
        /// Starts a new workflow
        /// </summary>
        /// <param name="request">The request instance</param>
        /// <returns>The response</returns>
        Task<Response<Identifier<Identifiers.Workflow>>> StartWorkflow(Request<StartWorkflowPayload> request);

        /// <summary>
        /// Restarts a new workflow.
        /// </summary>
        /// <param name="request">The request instance</param>
        /// <returns>The response</returns>
        Task<Response<Identifier<Identifiers.Workflow>>> RetryWorkflow(Request<RetryWorkflowPayload> request);

        #region Status Modifiers
        /// <summary>
        /// Pauses a new workflow. It can be resumed later
        /// </summary>
        /// <param name="request">The request instance</param>
        /// <returns>The response</returns>
        Task<Response<WorkflowStatus>> PauseWorkflow(Request<Identifier<Identifiers.Workflow>> request);

        /// <summary>
        /// Halts a workflow. Halted workflows cannot be resumed
        /// </summary>
        /// <param name="request">The request instance</param>
        /// <returns>The response</returns>
        Task<Response<WorkflowStatus>> HaltWorkflow(Request<Identifier<Identifiers.Workflow>> request);

        /// <summary>
        /// Resumes a workflow.
        /// </summary>
        /// <param name="request">The request instance</param>
        /// <returns>The response</returns>
        Task<Response<WorkflowStatus>> ResumeWorkflow(Request<Identifier<Identifiers.Workflow>> request);
        #endregion

        /// <summary>
        /// Skips to the specified activity within the workflow definition
        /// </summary>
        /// <param name="request">The request instance</param>
        /// <returns>The response</returns>
        Task<Response<Identifier<Identifiers.Workflow>>> SkipToActivity(Request<ActivitySkipPayload> request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Response<TimelineSnapshot>> GetTimeline(Request<Identifier<Identifiers.Workflow>> request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Response<WorkflowStatus>> GetWorkflowStatus(Request<Identifier<Identifiers.Workflow>> request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Response<Identifier<Activity>>> GetCurrentActivity(Request<Identifier<Identifiers.Workflow>> request);


    }
}
