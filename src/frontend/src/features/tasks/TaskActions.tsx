import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { Task } from "../../lib/contracts";
import { errorText } from "../../lib/formatters";
import { api } from "../../lib/api";

export function DeleteButton({ task }: { task: Task }) {
  const queryClient = useQueryClient();
  const mutation = useMutation({
    mutationFn: () => api.deleteTask(task.id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["tasks"] }),
  });

  function removeTask() {
    if (window.confirm(`Delete “${task.title}”?`)) {
      mutation.mutate();
    }
  }

  return (
    <>
      <button
        className="text-button danger"
        disabled={mutation.isPending}
        onClick={removeTask}
      >
        {mutation.isPending ? "Deleting…" : "Delete"}
      </button>
      {mutation.isError && (
        <small className="form-error">{errorText(mutation.error)}</small>
      )}
    </>
  );
}
