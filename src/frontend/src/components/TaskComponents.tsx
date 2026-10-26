import { Link } from "react-router-dom";
import type { Task, TaskStatus } from "../lib/contracts";
import { formatDate } from "../lib/formatters";
import { statusLabel } from "../lib/constants";

export function StatusBadge({ status }: { status: TaskStatus }) {
  return (
    <span className={`badge ${status.toLowerCase()}`}>
      {statusLabel(status)}
    </span>
  );
}

export function TaskRow({ task }: { task: Task }) {
  return (
    <Link className="task-row" to={`/tasks/${task.id}`}>
      <span className={`task-status ${task.status.toLowerCase()}`} />
      <div>
        <strong>{task.title}</strong>
        <small>
          {task.description || "No description"} · due{" "}
          {formatDate(task.dueDateUtc)}
        </small>
      </div>
      <StatusBadge status={task.status} />
    </Link>
  );
}
