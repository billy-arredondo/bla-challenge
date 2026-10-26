import { create } from "zustand";

export type Theme = "light" | "dark";

type ThemeState = {
  theme: Theme;
  toggle: () => void;
  setTheme: (theme: Theme) => void;
};

function getInitialTheme(): Theme {
  const storedTheme = localStorage.getItem("taskpulse-theme") as Theme | null;

  if (storedTheme === "light" || storedTheme === "dark") {
    return storedTheme;
  }

  return matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
}

export const useTheme = create<ThemeState>((set) => ({
  theme: getInitialTheme(),
  toggle: () =>
    set((state) => {
      const theme = state.theme === "light" ? "dark" : "light";

      localStorage.setItem("taskpulse-theme", theme);
      return { theme };
    }),
  setTheme: (theme) => {
    localStorage.setItem("taskpulse-theme", theme);
    set({ theme });
  },
}));
