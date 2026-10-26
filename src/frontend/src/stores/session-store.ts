import { create } from "zustand";
import type { User } from "../lib/contracts";

type SessionState = {
  token: string | null;
  user: User | null;
  setSession: (token: string, user: User) => void;
  clear: () => void;
};

export const useSession = create<SessionState>((set) => ({
  token: sessionStorage.getItem("taskpulse-token"),
  user: null,
  setSession: (token, user) => {
    sessionStorage.setItem("taskpulse-token", token);
    set({ token, user });
  },
  clear: () => {
    sessionStorage.removeItem("taskpulse-token");
    set({ token: null, user: null });
  },
}));
