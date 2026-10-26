import { useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Field } from "../../components/Field";
import { PageError, PageLoading } from "../../components/PageStates";
import { PageHeader } from "../../components/PageHeader";
import { api } from "../../lib/api";
import { type Role } from "../../lib/contracts";
import { errorText, formatDate, initials } from "../../lib/formatters";

export function UsersPage() {
  const query = useQuery({ queryKey: ["users"], queryFn: api.users });
  const queryClient = useQueryClient();
  const [form, setForm] = useState({
    email: "",
    password: "",
    role: "User" as Role,
  });
  const [error, setError] = useState("");
  const mutation = useMutation({
    mutationFn: () => api.createUser(form),
    onSuccess: () => {
      setForm({ email: "", password: "", role: "User" });
      queryClient.invalidateQueries({ queryKey: ["users"] });
    },
    onError: (reason) => setError(errorText(reason)),
  });
  const roleMutation = useMutation({
    mutationFn: ({ id, role }: { id: string; role: Role }) =>
      api.changeRole(id, role),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["users"] }),
    onError: (reason) => setError(errorText(reason)),
  });

  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");
    mutation.mutate();
  }

  if (query.isLoading) return <PageLoading />;
  if (query.isError) return <PageError message={errorText(query.error)} />;

  return (
    <section className="page">
      <PageHeader
        eyebrow="Superadmin"
        title="Users"
        description="Create accounts and control workspace access."
      />
      <div className="user-layout">
        <div className="panel">
          <div className="panel-heading">
            <div>
              <span className="eyebrow">Directory</span>
              <h2>Workspace users</h2>
            </div>
            <span className="count-chip">{query.data?.length ?? 0}</span>
          </div>
          {query.data?.map((candidate) => (
            <div className="user-row" key={candidate.id}>
              <span className="avatar">{initials(candidate.email)}</span>
              <div>
                <strong>{candidate.email}</strong>
                <small>Joined {formatDate(candidate.createdAtUtc)}</small>
              </div>
              <select
                value={candidate.role}
                disabled={
                  candidate.role === "Superadmin" || roleMutation.isPending
                }
                onChange={(event) =>
                  roleMutation.mutate({
                    id: candidate.id,
                    role: event.target.value as Role,
                  })
                }
              >
                <option value="User">User</option>
                <option value="Admin">Admin</option>
                <option value="Superadmin">Superadmin</option>
              </select>
            </div>
          ))}
        </div>
        <form className="panel create-user" onSubmit={submit}>
          <span className="eyebrow">Provision access</span>
          <h2>New account</h2>
          <Field label="Email">
            <input
              type="email"
              value={form.email}
              onChange={(event) =>
                setForm({ ...form, email: event.target.value })
              }
              placeholder="person@company.com"
            />
          </Field>
          <Field label="Temporary password">
            <input
              type="password"
              value={form.password}
              onChange={(event) =>
                setForm({ ...form, password: event.target.value })
              }
              placeholder="At least 8 characters"
            />
          </Field>
          <Field label="Role">
            <select
              value={form.role}
              onChange={(event) =>
                setForm({ ...form, role: event.target.value as Role })
              }
            >
              <option value="User">User</option>
              <option value="Admin">Admin</option>
            </select>
          </Field>
          {error && <div className="alert error">{error}</div>}
          <button className="primary-button wide" disabled={mutation.isPending}>
            {mutation.isPending ? "Creating…" : "Create account"}
          </button>
        </form>
      </div>
    </section>
  );
}
