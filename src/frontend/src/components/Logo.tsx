import { Link } from "react-router-dom";

export function Logo() {
  return (
    <Link className="brand" to="/tasks">
      <span className="brand-mark">✓</span>
      <span>TaskPulse</span>
    </Link>
  );
}
