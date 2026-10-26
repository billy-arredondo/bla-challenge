import { useEffect } from "react";
import { useTheme } from "../stores";

export function ThemeSync() {
  const theme = useTheme((state) => state.theme);

  useEffect(() => {
    document.documentElement.dataset.theme = theme;
  }, [theme]);

  return null;
}
