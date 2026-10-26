import type { Role } from "../lib/contracts";

export function canManageAllTasks(role?: Role | null): boolean {
  return role === "Admin" || role === "Superadmin";
}

export function canManageUsers(role?: Role | null): boolean {
  return role === "Superadmin";
}
