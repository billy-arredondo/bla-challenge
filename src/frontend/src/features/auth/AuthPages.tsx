import { useState, type FormEvent } from "react";
import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { AuthForm } from "./AuthForm";
import { AuthLayout } from "../../layouts/AuthLayout";
import { credentialsSchema } from "../../lib/contracts";
import { errorText } from "../../lib/formatters";
import { api } from "../../lib/api";
import { useSession } from "../../stores";

const demoCredentials = {
  email: "superadmin@example.com",
  password: "Password123!",
};

export function LoginPage() {
  const { setSession } = useSession();
  const navigate = useNavigate();
  const [values, setValues] = useState({ email: "", password: "" });
  const [error, setError] = useState("");
  const mutation = useMutation({
    mutationFn: () => api.login(values),
    onSuccess: (result) => {
      setSession(result.accessToken, result.user);
      navigate("/dashboard");
    },
    onError: (reason) => setError(errorText(reason)),
  });

  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");
    const parsed = credentialsSchema.safeParse(values);

    if (!parsed.success) {
      setError(parsed.error.issues[0].message);
      return;
    }

    mutation.mutate();
  }

  return (
    <AuthLayout title="Welcome back" subtitle="Sign in to manage your work">
      <div className="demo-banner">
        <span>⌘</span>
        <div>
          <strong>Demo access</strong>
          <small>Use your configured API credentials</small>
        </div>
        <button type="button" onClick={() => setValues(demoCredentials)}>
          Auto-fill
        </button>
      </div>
      <AuthForm
        values={values}
        setValues={setValues}
        submit={submit}
        loading={mutation.isPending}
        error={error}
        action="Sign in"
      />
    </AuthLayout>
  );
}

export function RegisterPage() {
  const navigate = useNavigate();
  const [values, setValues] = useState({ email: "", password: "" });
  const [confirm, setConfirm] = useState("");
  const [error, setError] = useState("");
  const mutation = useMutation({
    mutationFn: () => api.register(values),
    onSuccess: () => navigate("/login?registered=true"),
    onError: (reason) => setError(errorText(reason)),
  });

  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");
    const parsed = credentialsSchema.safeParse(values);

    if (!parsed.success) {
      setError(parsed.error.issues[0].message);
      return;
    }

    if (values.password !== confirm) {
      setError("Passwords do not match.");
      return;
    }

    mutation.mutate();
  }

  return (
    <AuthLayout
      title="Create an account"
      subtitle="Start organizing your team’s work"
    >
      <AuthForm
        values={values}
        setValues={setValues}
        submit={submit}
        loading={mutation.isPending}
        error={error}
        action="Create account"
        confirm={confirm}
        setConfirm={setConfirm}
      />
    </AuthLayout>
  );
}
