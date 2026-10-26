import { useEffect, type ReactNode } from "react";
import { Link } from "react-router-dom";
import { Logo } from "../components/Logo";
import { useTheme } from "../stores";

type AuthLayoutProps = {
  title: string;
  subtitle: string;
  children: ReactNode;
};

export function AuthLayout({ title, subtitle, children }: AuthLayoutProps) {
  const theme = useTheme((state) => state.theme);

  useEffect(() => {
    document.documentElement.dataset.theme = theme;
  }, [theme]);

  const isLogin = title === "Welcome back";

  return (
    <main className="auth-page">
      <div className="ambient ambient-one" />
      <div className="ambient ambient-two" />
      <div className="auth-card">
        <div className="auth-heading">
          <Logo />
          <span className="status-pill">● API connected</span>
          <h1>{title}</h1>
          <p>{subtitle}</p>
        </div>
        {children}
        <div className="auth-footer">
          {isLogin ? (
            <>
              New here? <Link to="/register">Create an account</Link>
            </>
          ) : (
            <>
              Already have an account? <Link to="/login">Sign in</Link>
            </>
          )}
        </div>
      </div>
      <small className="security-note">
        JWT protected workspace · light/dark ready
      </small>
    </main>
  );
}
