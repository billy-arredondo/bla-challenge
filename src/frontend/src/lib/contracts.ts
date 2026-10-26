import { z } from 'zod'

export const roleSchema = z.enum(['User', 'Admin', 'Superadmin'])
export const statusSchema = z.enum(['Pending', 'InProgress', 'Completed', 'Canceled'])

export const userSchema = z.object({
  id: z.string().uuid(), email: z.string().email(), createdAtUtc: z.string(), role: roleSchema,
})
export const taskSchema = z.object({
  id: z.string().uuid(), title: z.string(), description: z.string().nullable(), status: statusSchema,
  dueDateUtc: z.string(), ownerId: z.string().uuid(), createdById: z.string().uuid(),
  assignedToId: z.string().uuid(), updatedById: z.string().uuid().nullable(), updatedAtUtc: z.string().nullable(),
})
export const loginSchema = z.object({ user: userSchema, accessToken: z.string() })

export const credentialsSchema = z.object({
  email: z.string().email('Enter a valid email address.'),
  password: z.string().min(8, 'Password must contain at least 8 characters.'),
})
export const taskFormSchema = z.object({
  title: z.string().trim().min(1, 'Title is required.').max(200, 'Title cannot exceed 200 characters.'),
  description: z.string().max(2000, 'Description cannot exceed 2,000 characters.'),
  status: statusSchema,
  dueDateUtc: z.string().min(1, 'Due date is required.'),
  assignedToId: z.string().optional(),
})

export type Role = z.infer<typeof roleSchema>
export type TaskStatus = z.infer<typeof statusSchema>
export type User = z.infer<typeof userSchema>
export type Task = z.infer<typeof taskSchema>
export type Credentials = z.infer<typeof credentialsSchema>
export type TaskForm = z.infer<typeof taskFormSchema>
