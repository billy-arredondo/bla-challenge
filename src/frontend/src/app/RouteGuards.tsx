import type { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { PageLoading } from "../components/PageStates";
import { useSession } from "../stores";

export function Protected({ children }: { children: ReactNode }) {
  const token = useSession((state) => state.token);

  return token ? <>{children}</> : <Navigate to="/login" replace />;
}

export function SuperadminOnly({ children }: { children: ReactNode }) {
  const user = useSession((state) => state.user);
  const token = useSession((state) => state.token);

  if (token && !user) {
    return <PageLoading />;
  }

  return user?.role === "Superadmin" ? (
    <>{children}</>
  ) : (
    <Navigate to="/dashboard" replace />
  );
}
