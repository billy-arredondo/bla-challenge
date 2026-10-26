import type { FormEvent } from "react";
import { Field } from "../../components/Field";

type Credentials = {
  email: string;
  password: string;
};

type AuthFormProps = {
  values: Credentials;
  setValues: (values: Credentials) => void;
  submit: (event: FormEvent<HTMLFormElement>) => void;
  loading: boolean;
  error: string;
  action: string;
  confirm?: string;
  setConfirm?: (value: string) => void;
};

export function AuthForm({
  values,
  setValues,
  submit,
  loading,
  error,
  action,
  confirm,
  setConfirm,
}: AuthFormProps) {
  const updateEmail = (email: string) => setValues({ ...values, email });
  const updatePassword = (password: string) =>
    setValues({ ...values, password });

  return (
    <form className="auth-form" onSubmit={submit}>
      <Field label="Email address">
        <input
          type="email"
          value={values.email}
          onChange={(event) => updateEmail(event.target.value)}
          placeholder="you@company.com"
          autoComplete="email"
        />
      </Field>
      <Field label="Password">
        <input
          type="password"
          value={values.password}
          onChange={(event) => updatePassword(event.target.value)}
          placeholder="At least 8 characters"
          autoComplete={
            confirm === undefined ? "current-password" : "new-password"
          }
        />
      </Field>
      {setConfirm && (
        <Field label="Confirm password">
          <input
            type="password"
            value={confirm}
            onChange={(event) => setConfirm(event.target.value)}
            placeholder="Repeat your password"
            autoComplete="new-password"
          />
        </Field>
      )}
      {error && <div className="alert error">{error}</div>}
      <button className="primary-button wide" disabled={loading}>
        {loading ? "Working…" : action}
        <span>→</span>
      </button>
    </form>
  );
}
