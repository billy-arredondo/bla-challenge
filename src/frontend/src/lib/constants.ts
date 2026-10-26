import type { TaskStatus } from "./contracts";

export const taskStatuses: TaskStatus[] = [
  "Pending",
  "InProgress",
  "Completed",
  "Canceled",
];

export function statusLabel(status: string): string {
  return status === "InProgress" ? "In progress" : status;
}
