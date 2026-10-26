# User Story

## Informal Story

As a small delivery team member, I want to sign in to a shared task workspace,
create and update tasks with an owner, status, description, and due date, and
see only the work I am allowed to access so that the team can coordinate work
without sharing spreadsheets or credentials.

Administrators can manage all tasks and assign work to users. Superadmins can
also provision accounts and grant Admin privileges. A task cannot be changed
after it is canceled or completed, which preserves the history of finished
work.

## Acceptance Flow

1. A visitor can register or log in.
2. An authenticated User can create, view, update, and delete their own task.
3. A User cannot read or modify another user's task.
4. An Admin can view all users, manage all tasks, and assign tasks.
5. A Superadmin can create users and change User/Admin roles.
6. The React frontend displays loading, empty, validation, authorization, and
   API error states on desktop and mobile layouts.
