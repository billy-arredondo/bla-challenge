import { Navigate, Route, Routes } from "react-router-dom";
import { AppLayout } from "../layouts/AppLayout";
import { LoginPage, RegisterPage } from "../features/auth/AuthPages";
import { DashboardPage } from "../features/dashboard/DashboardPage";
import { TaskDetailPage, TasksPage } from "../features/tasks/TaskPages";
import { TaskEditorPage } from "../features/tasks/TaskEditor";
import { UsersPage } from "../features/users/UsersPage";
import { Protected, SuperadminOnly } from "./RouteGuards";

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route
        element={
          <Protected>
            <AppLayout />
          </Protected>
        }
      >
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/tasks" element={<TasksPage />} />
        <Route path="/tasks/new" element={<TaskEditorPage />} />
        <Route path="/tasks/:id" element={<TaskDetailPage />} />
        <Route path="/tasks/:id/edit" element={<TaskEditorPage />} />
        <Route
          path="/users"
          element={
            <SuperadminOnly>
              <UsersPage />
            </SuperadminOnly>
          }
        />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
