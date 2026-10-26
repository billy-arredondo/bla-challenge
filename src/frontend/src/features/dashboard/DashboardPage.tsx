import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { Empty, PageError, PageLoading } from "../../components/PageStates";
import { PageHeader } from "../../components/PageHeader";
import { TaskRow } from "../../components/TaskComponents";
import { api } from "../../lib/api";
import { errorText } from "../../lib/formatters";
import { useSession } from "../../stores";

type MetricProps = {
  label: string;
  value: number;
  note: string;
  tone?: string;
};

function Metric({ label, value, note, tone = "" }: MetricProps) {
  const icon =
    tone === "green"
      ? "✓"
      : tone === "amber"
        ? "◷"
        : tone === "blue"
          ? "↻"
          : "▤";

  return (
    <div className={`metric ${tone}`}>
      <span className="metric-icon">{icon}</span>
      <span className="eyebrow">{label}</span>
      <strong>{value}</strong>
      <small>{note}</small>
    </div>
  );
}

export function DashboardPage() {
  const user = useSession((state) => state.user);
  const tasksQuery = useQuery({ queryKey: ["tasks"], queryFn: api.tasks });

  if (tasksQuery.isLoading) return <PageLoading />;
  if (tasksQuery.isError)
    return <PageError message={errorText(tasksQuery.error)} />;

  const tasks = tasksQuery.data ?? [];
  const pending = tasks.filter((task) => task.status === "Pending").length;
  const inProgress = tasks.filter(
    (task) => task.status === "InProgress",
  ).length;
  const completed = tasks.filter((task) => task.status === "Completed").length;
  const activeTasks = tasks.filter(
    (task) => task.status !== "Completed" && task.status !== "Canceled",
  );
  const completion = tasks.length
    ? Math.round((completed / tasks.length) * 100)
    : 0;

  return (
    <section className="page">
      <PageHeader
        eyebrow="Overview"
        title="Dashboard"
        description={`Good to see you, ${user?.email.split("@")[0]}. Here’s the pulse of your workspace.`}
        action={
          <Link className="primary-button" to="/tasks/new">
            ＋ New task
          </Link>
        }
      />
      <div className="metric-grid">
        <Metric
          label="Total tasks"
          value={tasks.length}
          note="Visible to your role"
        />
        <Metric
          label="Pending"
          value={pending}
          note="Ready to start"
          tone="amber"
        />
        <Metric
          label="In progress"
          value={inProgress}
          note="Currently active"
          tone="blue"
        />
        <Metric
          label="Completed"
          value={completed}
          note={`${completion}% completion`}
          tone="green"
        />
      </div>
      <div className="dashboard-grid">
        <div className="panel">
          <div className="panel-heading">
            <div>
              <span className="eyebrow">Attention needed</span>
              <h2>Upcoming tasks</h2>
            </div>
            <Link to="/tasks">View all →</Link>
          </div>
          {activeTasks.slice(0, 5).map((task) => (
            <TaskRow key={task.id} task={task} />
          ))}
          {!activeTasks.length && (
            <Empty
              title="Nothing urgent"
              text="Create a task to get started."
            />
          )}
        </div>
        <div className="panel progress-panel">
          <span className="eyebrow">Completion</span>
          <h2>Task health</h2>
          <div
            className="progress-ring"
            style={{ "--progress": `${completion}%` } as React.CSSProperties}
          >
            <strong>{completion}%</strong>
          </div>
          <div className="legend">
            <span>
              <i className="dot pending" /> Pending <b>{pending}</b>
            </span>
            <span>
              <i className="dot inprogress" /> In progress <b>{inProgress}</b>
            </span>
            <span>
              <i className="dot completed" /> Completed <b>{completed}</b>
            </span>
          </div>
        </div>
      </div>
    </section>
  );
}
