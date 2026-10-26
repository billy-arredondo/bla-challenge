import { useTheme } from "../stores/theme-store";

export function ThemeButton() {
  const { theme, toggle } = useTheme();
  const nextTheme = theme === "light" ? "dark" : "light";

  return (
    <button
      className="icon-button"
      onClick={toggle}
      title={`Switch to ${nextTheme} mode`}
      aria-label="Toggle color theme"
    >
      {theme === "light" ? "☾" : "☀"}
    </button>
  );
}
