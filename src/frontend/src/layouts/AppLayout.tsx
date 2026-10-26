import { NavLink, Outlet, useLocation, useNavigate } from "react-router-dom";
import { Logo } from "../components/Logo";
import { ThemeButton } from "../components/ThemeButton";
import { initials } from "../lib/formatters";
import { canManageUsers, useSession } from "../stores";

export function AppLayout() {
  const { user, clear } = useSession();
  const navigate = useNavigate();
  const location = useLocation();
  const section = location.pathname.includes("users")
    ? "Administration"
    : "Workspace";
  const page = location.pathname.includes("tasks")
    ? "Tasks"
    : location.pathname.replace("/", "") || "Dashboard";

  function signOut() {
    clear();
    navigate("/login");
  }

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div>
          <div className="sidebar-head">
            <Logo />
            <ThemeButton />
          </div>
          <div className="workspace">
            <span className="avatar">{user ? initials(user.email) : "TP"}</span>
            <div>
              <strong>{user?.role ?? "Workspace"}</strong>
              <small>{user?.email}</small>
            </div>
          </div>
          <nav className="nav">
            <span className="eyebrow">Navigation</span>
            <NavLink to="/dashboard">
              ▦ <span>Dashboard</span>
            </NavLink>
            <NavLink to="/tasks">
              ✓ <span>Tasks</span>
            </NavLink>
            <NavLink to="/tasks/new">
              ＋ <span>New task</span>
            </NavLink>
            {canManageUsers(user?.role) && (
              <NavLink to="/users">
                ♙ <span>Users</span>
              </NavLink>
            )}
          </nav>
        </div>
        <button className="signout" onClick={signOut}>
          ↪ <span>Sign out</span>
        </button>
      </aside>
      <main className="main">
        <header className="topbar">
          <div className="crumb">
            {section} <span>/</span> {page}
          </div>
          <div className="top-actions">
            <ThemeButton />
            <span className="role-chip">{user?.role}</span>
            <span className="avatar avatar-small">
              {user ? initials(user.email) : "TP"}
            </span>
          </div>
        </header>
        <Outlet />
      </main>
    </div>
  );
}
