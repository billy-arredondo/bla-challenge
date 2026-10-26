import { ApiError } from "./api";

export function formatDate(value: string): string {
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
  }).format(new Date(value));
}

export function initials(email: string): string {
  return email.split("@")[0].slice(0, 2).toUpperCase();
}

export function errorText(error: unknown): string {
  if (error instanceof ApiError) {
    return error.message;
  }

  return "Something went wrong. Try again.";
}
