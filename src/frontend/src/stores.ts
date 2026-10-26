import { create } from 'zustand'
import type { Role, User } from './lib/contracts'

type SessionState = { token: string | null; user: User | null; setSession: (token: string, user: User) => void; clear: () => void }
export const useSession = create<SessionState>((set) => ({
  token: sessionStorage.getItem('taskpulse-token'), user: null,
  setSession: (token, user) => { sessionStorage.setItem('taskpulse-token', token); set({ token, user }) },
  clear: () => { sessionStorage.removeItem('taskpulse-token'); set({ token: null, user: null }) },
}))

type Theme = 'light' | 'dark'
type ThemeState = { theme: Theme; toggle: () => void; setTheme: (theme: Theme) => void }
const initialTheme: Theme = (localStorage.getItem('taskpulse-theme') as Theme | null) ?? (matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light')
export const useTheme = create<ThemeState>((set) => ({
  theme: initialTheme,
  toggle: () => set((state) => { const theme = state.theme === 'light' ? 'dark' : 'light'; localStorage.setItem('taskpulse-theme', theme); return { theme } }),
  setTheme: (theme) => { localStorage.setItem('taskpulse-theme', theme); set({ theme }) },
}))

export const canManageAllTasks = (role?: Role | null) => role === 'Admin' || role === 'Superadmin'
export const canManageUsers = (role?: Role | null) => role === 'Superadmin'
