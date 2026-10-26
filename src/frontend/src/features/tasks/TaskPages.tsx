import { useState } from "react";
import { Link, useParams } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { Empty, PageError, PageLoading } from "../../components/PageStates";
import { PageHeader } from "../../components/PageHeader";
import { StatusBadge } from "../../components/TaskComponents";
import { api } from "../../lib/api";
import { taskStatuses, statusLabel } from "../../lib/constants";
import { errorText, formatDate } from "../../lib/formatters";
import { DeleteButton } from "./TaskActions";

export function TasksPage() {
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState<(typeof taskStatuses)[number] | "All">(
    "All",
  );
  const query = useQuery({ queryKey: ["tasks"], queryFn: api.tasks });

  if (query.isLoading) return <PageLoading />;
  if (query.isError) return <PageError message={errorText(query.error)} />;

  const allTasks = query.data ?? [];
  const tasks = allTasks.filter((task) => {
    const matchesFilter = filter === "All" || task.status === filter;
    const searchableText =
      `${task.title} ${task.description ?? ""}`.toLowerCase();
    return matchesFilter && searchableText.includes(search.toLowerCase());
  });

  return (
    <section className="page">
      <PageHeader
        eyebrow="Workspace"
        title="Tasks"
        description="Track work, ownership and progress in one place."
        action={
          <Link className="primary-button" to="/tasks/new">
            ＋ Create task
          </Link>
        }
      />
      <div className="toolbar">
        <div className="search">
          <span>⌕</span>
          <input
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Search tasks…"
          />
        </div>
        <div className="filter-tabs">
          <button
            className={filter === "All" ? "selected" : ""}
            onClick={() => setFilter("All")}
          >
            All <small>{allTasks.length}</small>
          </button>
          {taskStatuses.map((status) => (
            <button
              className={filter === status ? "selected" : ""}
              key={status}
              onClick={() => setFilter(status)}
            >
              {statusLabel(status)}{" "}
              <small>
                {allTasks.filter((task) => task.status === status).length}
              </small>
            </button>
          ))}
        </div>
      </div>
      <div className="task-table">
        <div className="table-head">
          <span>Task details</span>
          <span>Status</span>
          <span>Due date</span>
          <span>Actions</span>
        </div>
        {tasks.map((task) => (
          <div className="table-row" key={task.id}>
            <Link to={`/tasks/${task.id}`}>
              <strong>{task.title}</strong>
              <small>{task.description || "No description"}</small>
            </Link>
            <StatusBadge status={task.status} />
            <span>{formatDate(task.dueDateUtc)}</span>
            <div className="row-actions">
              <Link to={`/tasks/${task.id}/edit`}>Edit</Link>
              <DeleteButton task={task} />
            </div>
          </div>
        ))}
        {!tasks.length && (
          <Empty
            title="No tasks found"
            text="Adjust your filters or create a new task."
          />
        )}
      </div>
    </section>
  );
}

export function TaskDetailPage() {
  const { id } = useParams();
  const query = useQuery({
    queryKey: ["task", id],
    queryFn: () => api.task(id!),
  });

  if (query.isLoading) return <PageLoading />;
  if (query.isError || !query.data)
    return <PageError message={errorText(query.error)} />;

  const task = query.data;

  return (
    <section className="page narrow">
      <PageHeader
        eyebrow="Task detail"
        title={task.title}
        description={task.description ?? "No description added."}
        action={
          <Link className="secondary-button" to={`/tasks/${id}/edit`}>
            Edit task
          </Link>
        }
      />
      <div className="detail-grid">
        <div className="panel detail-card">
          <div className="detail-status">
            <StatusBadge status={task.status} />
            <span>Due {formatDate(task.dueDateUtc)}</span>
          </div>
          <p>
            {task.description || "This task has no additional description."}
          </p>
          <div className="detail-meta">
            <span>
              <small>Assigned to</small>
              <strong>{task.assignedToId}</strong>
            </span>
            <span>
              <small>Created by</small>
              <strong>{task.createdById}</strong>
            </span>
            <span>
              <small>Last updated</small>
              <strong>
                {task.updatedAtUtc
                  ? formatDate(task.updatedAtUtc)
                  : "Not updated"}
              </strong>
            </span>
          </div>
        </div>
        <div className="panel">
          <span className="eyebrow">Actions</span>
          <h2>Manage task</h2>
          <p className="muted">
            Changes are checked by the API according to your role and task
            state.
          </p>
          <DeleteButton task={task} />
        </div>
      </div>
    </section>
  );
}
