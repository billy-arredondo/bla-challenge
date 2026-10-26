# Presentation Guide

This guide is the concise walkthrough for the technical interview demo.

## Demo Sequence

1. Explain the user story: a team needs a secure task workspace with role-aware
   ownership and status rules.
2. Start the API and frontend using the commands in the root README.
3. Register a normal User, log in, create a task, edit it, and delete it.
4. Show that an unauthenticated request is rejected and that a User cannot read
   another user's task.
5. Log in as the seeded Superadmin, create an Admin and a User, then log in as
   the Admin and assign a task to that User.
6. Demonstrate that Completed and Canceled tasks cannot be changed.
7. Show the responsive task list and the light/dark theme preference.

## Technical Explanation

- Domain contains entities, roles, statuses, and state-transition rules.
- Application contains use cases, contracts, validation, and repository
  abstractions.
- Infrastructure contains parameterized `Microsoft.Data.Sqlite` adapters,
  password hashing, JWT generation, and database initialization.
- API contains HTTP controllers, authentication, authorization, and response
  mapping.
- Frontend is a React/Vite application organized by routes, features,
  components, stores, API contracts, and styles.
- Tests cover domain rules, application behavior, SQLite persistence, and API
  integration flows. Frontend build and lint are currently the automated UI
  checks.

## GenAI Review

Use `docs/genai.md` to explain the prompts used, representative generated
output, validation failures, corrections, and security decisions. Emphasize
that generated code was checked against the challenge constraints and tests,
especially authentication, authorization, SQL parameterization, and terminal
task states.
