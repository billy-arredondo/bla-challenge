import { loginSchema, taskSchema, userSchema, type Credentials, type Role, type TaskForm, type TaskStatus } from './contracts'

const baseUrl = import.meta.env.VITE_API_URL ?? ''

export class ApiError extends Error {
  status: number
  constructor(status: number, message: string) { super(message); this.status = status }
}

async function request<T>(path: string, init: RequestInit = {}, schema?: { parse: (value: unknown) => T }): Promise<T> {
  const token = sessionStorage.getItem('taskpulse-token')
  const response = await fetch(`${baseUrl}${path}`, {
    ...init,
    headers: { 'Content-Type': 'application/json', ...(token ? { Authorization: `Bearer ${token}` } : {}), ...init.headers },
  })
  if (!response.ok) {
    const body = await response.json().catch(() => null) as { error?: string } | null
    throw new ApiError(response.status, body?.error ?? `Request failed with status ${response.status}.`)
  }
  if (response.status === 204) return undefined as T
  const data: unknown = await response.json()
  return schema ? schema.parse(data) : data as T
}

export const api = {
  login: (body: Credentials) => request('/api/auth/login', { method: 'POST', body: JSON.stringify(body) }, loginSchema),
  register: (body: Credentials) => request('/api/auth/register', { method: 'POST', body: JSON.stringify(body) }, userSchema),
  me: () => request('/api/auth/me', {}, userSchema),
  tasks: () => request('/api/tasks', {}, { parse: (value) => (Array.isArray(value) ? value.map((item) => taskSchema.parse(item)) : []) }),
  task: (id: string) => request(`/api/tasks/${id}`, {}, taskSchema),
  createTask: (body: TaskForm) => request('/api/tasks', { method: 'POST', body: JSON.stringify({ ...body, dueDateUtc: new Date(body.dueDateUtc).toISOString(), assignedToId: body.assignedToId || null }) }, taskSchema),
  updateTask: (id: string, body: TaskForm) => request(`/api/tasks/${id}`, { method: 'PUT', body: JSON.stringify({ ...body, dueDateUtc: new Date(body.dueDateUtc).toISOString(), assignedToId: body.assignedToId || null }) }, taskSchema),
  deleteTask: (id: string) => request(`/api/tasks/${id}`, { method: 'DELETE' }),
  users: () => request('/api/users', {}, { parse: (value) => (Array.isArray(value) ? value.map((item) => userSchema.parse(item)) : []) }),
  createUser: (body: { email: string; password: string; role: Role }) => request('/api/users', { method: 'POST', body: JSON.stringify(body) }, userSchema),
  changeRole: (id: string, role: Role) => request(`/api/users/${id}/role`, { method: 'PUT', body: JSON.stringify({ role }) }, userSchema),
  statusLabel: (status: TaskStatus) => status === 'InProgress' ? 'In progress' : status,
}
