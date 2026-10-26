import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useNavigate, useParams } from "react-router-dom";
import { useState, type FormEvent } from "react";
import { Field } from "../../components/Field";
import { PageError, PageLoading } from "../../components/PageStates";
import { PageHeader } from "../../components/PageHeader";
import { api } from "../../lib/api";
import { taskFormSchema, type Task, type TaskForm } from "../../lib/contracts";
import { taskStatuses } from "../../lib/constants";
import { errorText } from "../../lib/formatters";
import { useSession } from "../../stores";

function defaultDueDate(): string {
  const tomorrow = new Date(Date.now() + 86400000);
  return tomorrow.toISOString().slice(0, 16);
}

function taskToForm(task: Task): TaskForm {
  return {
    title: task.title,
    description: task.description ?? "",
    status: task.status,
    dueDateUtc: new Date(task.dueDateUtc).toISOString().slice(0, 16),
    assignedToId: task.assignedToId,
  };
}

function allowedStatuses(task: Task | undefined) {
  if (!task) {
    return taskStatuses.slice(0, 3);
  }

  if (task.status === "Completed" || task.status === "Canceled") {
    return [task.status];
  }

  if (task.status === "Pending") {
    return ["Pending", "InProgress", "Canceled"] as const;
  }

  return ["InProgress", "Completed", "Canceled"] as const;
}

type TaskFormViewProps = {
  task?: Task;
};

function TaskFormView({ task }: TaskFormViewProps) {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const user = useSession((state) => state.user);
  const editing = Boolean(task);
  const [values, setValues] = useState<TaskForm>(() =>
    task
      ? taskToForm(task)
      : {
          title: "",
          description: "",
          status: "Pending",
          dueDateUtc: defaultDueDate(),
          assignedToId: "",
        },
  );
  const [error, setError] = useState("");
  const usersQuery = useQuery({
    queryKey: ["users"],
    queryFn: api.users,
    enabled: user?.role === "Superadmin",
  });
  const mutation = useMutation({
    mutationFn: (body: TaskForm) =>
      editing ? api.updateTask(task!.id, body) : api.createTask(body),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["tasks"] });
      navigate("/tasks");
    },
    onError: (reason) => setError(errorText(reason)),
  });

  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const parsed = taskFormSchema.safeParse(values);

    if (!parsed.success) {
      setError(parsed.error.issues[0].message);
      return;
    }

    setError("");
    mutation.mutate(parsed.data);
  }

  return (
    <section className="page narrow">
      <PageHeader
        eyebrow={editing ? "Edit task" : "New task"}
        title={editing ? "Refine the details" : "Create a task"}
        description="Capture the context, ownership and timing for this piece of work."
      />
      <form className="editor-grid" onSubmit={submit}>
        <div className="panel editor-main">
          <Field label="Title">
            <input
              value={values.title}
              onChange={(event) =>
                setValues({ ...values, title: event.target.value })
              }
              placeholder="What needs to be done?"
            />
          </Field>
          <Field label="Description">
            <textarea
              rows={7}
              value={values.description}
              onChange={(event) =>
                setValues({ ...values, description: event.target.value })
              }
              placeholder="Add context for the person working on this task."
            />
          </Field>
          <div className="form-grid">
            <Field label="Status">
              <select
                value={values.status}
                onChange={(event) =>
                  setValues({
                    ...values,
                    status: event.target.value as TaskForm["status"],
                  })
                }
              >
                {allowedStatuses(task).map((status) => (
                  <option value={status} key={status}>
                    {status}
                  </option>
                ))}
              </select>
            </Field>
            <Field label="Due date">
              <input
                type="datetime-local"
                value={values.dueDateUtc}
                onChange={(event) =>
                  setValues({ ...values, dueDateUtc: event.target.value })
                }
              />
            </Field>
          </div>
          {usersQuery.data && (
            <Field label="Assign to">
              <select
                value={values.assignedToId}
                onChange={(event) =>
                  setValues({ ...values, assignedToId: event.target.value })
                }
              >
                {usersQuery.data.map((candidate) => (
                  <option value={candidate.id} key={candidate.id}>
                    {candidate.email}
                  </option>
                ))}
              </select>
            </Field>
          )}
          {error && <div className="alert error">{error}</div>}
          <div className="editor-actions">
            <button
              type="button"
              className="secondary-button"
              onClick={() => navigate("/tasks")}
            >
              Cancel
            </button>
            <button className="primary-button" disabled={mutation.isPending}>
              {mutation.isPending
                ? "Saving…"
                : editing
                  ? "Save changes"
                  : "Create task"}
            </button>
          </div>
        </div>
        <aside className="panel editor-aside">
          <span className="eyebrow">Workflow note</span>
          <h2>Keep it actionable</h2>
          <p>
            A clear title, useful context and an agreed due date help the team
            move work forward without extra meetings.
          </p>
          <div className="aside-rule" />
          <span className="eyebrow">Access</span>
          <p>
            Changes are checked by the API according to your role and task
            state.
          </p>
        </aside>
      </form>
    </section>
  );
}

export function TaskEditorPage() {
  const { id } = useParams();
  const editing = Boolean(id);
  const taskQuery = useQuery({
    queryKey: ["task", id],
    queryFn: () => api.task(id!),
    enabled: editing,
  });

  if (editing && taskQuery.isLoading) {
    return <PageLoading />;
  }

  if (editing && (taskQuery.isError || !taskQuery.data)) {
    return <PageError message={errorText(taskQuery.error)} />;
  }

  return <TaskFormView task={taskQuery.data} />;
}
