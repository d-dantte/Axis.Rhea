
### 2024/10/12
 - `EventType` needs to have a few more items
	- `Paused`
	- `Halted`
	- `Resumed`
	- `Failed`
	- `Retried`: for instances where a failed workflow is restarted. This spawns a new workflow, but this event is used to capture the `FQN` of the new instance that was started.

 - 