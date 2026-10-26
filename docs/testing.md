# Testing Guide

This document describes what the current automated tests verify. It is an
inventory of behavior, not a claim of percentage code coverage.

## Test Strategy

The backend is tested at four levels:

- **Domain:** validates entity rules without persistence or HTTP concerns.
- **Application:** validates use-case behavior with in-memory test doubles.
- **Infrastructure:** validates the SQLite adapters and password hashing.
- **API:** validates HTTP status codes, authentication, authorization, and the
  end-to-end request flow through an in-memory test host and temporary SQLite
  database.

The test projects are located under `src/backend` and use xUnit. Coverlet is
configured in the test projects, but this repository does not currently publish
a coverage threshold or report.

## Current Coverage

### Domain tests

`BlaChallenge.Domain.Tests` verifies:

| Test file | Test method | Behavior verified |
| --- | --- | --- |
| [`UserTests.cs`](../src/backend/BlaChallenge.Domain.Tests/UserTests.cs) | `Create_normalizes_email` | User email is normalized to lowercase and trimmed. |
| [`UserTests.cs`](../src/backend/BlaChallenge.Domain.Tests/UserTests.cs) | `Create_rejects_empty_email` | An empty email is rejected. |
| [`TaskTests.cs`](../src/backend/BlaChallenge.Domain.Tests/TaskTests.cs) | `Create_trims_fields_and_preserves_status` | Task text fields are trimmed and the status is preserved. |
| [`TaskTests.cs`](../src/backend/BlaChallenge.Domain.Tests/TaskTests.cs) | `Create_rejects_empty_title` | Empty and whitespace-only task titles are rejected. |
| [`TaskTests.cs`](../src/backend/BlaChallenge.Domain.Tests/TaskTests.cs) | `Update_changes_all_mutable_fields` | Title, description, status, and due date can be updated through an allowed transition. |
| [`TaskTests.cs`](../src/backend/BlaChallenge.Domain.Tests/TaskTests.cs) | `Create_rejects_unknown_status` | An unknown task status is rejected. |
| [`TaskTests.cs`](../src/backend/BlaChallenge.Domain.Tests/TaskTests.cs) | `Update_rejects_pending_to_completed_transition` | Pending tasks cannot skip directly to Completed. |
| [`TaskTests.cs`](../src/backend/BlaChallenge.Domain.Tests/TaskTests.cs) | `Canceled_task_cannot_be_modified` | Canceled tasks are terminal. |

### Application tests

`BlaChallenge.Application.Tests` verifies:

| Test file | Test method | Behavior verified |
| --- | --- | --- |
| [`AuthenticationTests.cs`](../src/backend/BlaChallenge.Application.Tests/AuthenticationTests.cs) | `Register_rejects_duplicate_email` | Duplicate registration is rejected. |
| [`AuthenticationTests.cs`](../src/backend/BlaChallenge.Application.Tests/AuthenticationTests.cs) | `Register_hashes_password_and_returns_user` | Registration normalizes the email and hashes the password instead of storing plaintext. |
| [`AuthenticationTests.cs`](../src/backend/BlaChallenge.Application.Tests/AuthenticationTests.cs) | `Login_rejects_invalid_password` | Invalid login credentials are rejected. |
| [`AuthenticationTests.cs`](../src/backend/BlaChallenge.Application.Tests/AuthenticationTests.cs) | `Register_rejects_short_password` | Short passwords are rejected. |
| [`AuthenticationTests.cs`](../src/backend/BlaChallenge.Application.Tests/AuthenticationTests.cs) | `Get_current_user_returns_user_response` | The current-user use case returns the expected user response. |
| [`TaskTests.cs`](../src/backend/BlaChallenge.Application.Tests/TaskTests.cs) | `Create_assigns_the_authenticated_owner` | Task creation assigns the authenticated user as owner. |
| [`TaskTests.cs`](../src/backend/BlaChallenge.Application.Tests/TaskTests.cs) | `Get_tasks_returns_only_repository_results_for_owner` | Task queries use the repository results for the requested owner. |
| [`TaskTests.cs`](../src/backend/BlaChallenge.Application.Tests/TaskTests.cs) | `Update_missing_task_returns_not_found_error` | Updating a missing or non-owned task returns a not-found error. |

### Infrastructure tests

`BlaChallenge.Infrastructure.Tests` verifies:

| Test file | Test method | Behavior verified |
| --- | --- | --- |
| [`SqliteUserRepositoryTests.cs`](../src/backend/BlaChallenge.Infrastructure.Tests/SqliteUserRepositoryTests.cs) | `Add_and_find_user_round_trip_through_sqlite` | User persistence round-trips through SQLite by email and id. |
| [`SqliteUserRepositoryTests.cs`](../src/backend/BlaChallenge.Infrastructure.Tests/SqliteUserRepositoryTests.cs) | `Add_rejects_duplicate_email` | SQLite rejects duplicate user email addresses case-insensitively. |
| [`SqliteTaskRepositoryTests.cs`](../src/backend/BlaChallenge.Infrastructure.Tests/SqliteTaskRepositoryTests.cs) | `Crud_round_trip_and_owner_filtering_work` | Task persistence supports create, list-by-owner, read, update, delete, and owner filtering. |
| [`PasswordHasherTests.cs`](../src/backend/BlaChallenge.Infrastructure.Tests/PasswordHasherTests.cs) | `Hash_does_not_store_plaintext_and_verifies_original_password` | Password hashes do not contain plaintext and verify the original password while rejecting a different password. |

Each SQLite test uses a temporary database and clears SQLite connection pools
before cleanup.

### API tests

`BlaChallenge.Api.Tests` verifies:

| Test file | Test method | Behavior verified |
| --- | --- | --- |
| [`AuthenticationApiTests.cs`](../src/backend/BlaChallenge.Api.Tests/AuthenticationApiTests.cs) | `Ping_is_public` | The public authentication ping endpoint returns `200 OK`. |
| [`AuthenticationApiTests.cs`](../src/backend/BlaChallenge.Api.Tests/AuthenticationApiTests.cs) | `Me_requires_authentication` | The current-user endpoint requires authentication. |
| [`AuthenticationApiTests.cs`](../src/backend/BlaChallenge.Api.Tests/AuthenticationApiTests.cs) | `Register_login_and_me_round_trip` | Registration, login, JWT authentication, and the current-user endpoint work together. |
| [`AuthenticationApiTests.cs`](../src/backend/BlaChallenge.Api.Tests/AuthenticationApiTests.cs) | `Login_with_wrong_password_returns_unauthorized` | Invalid login credentials return `401 Unauthorized`. |
| [`AuthenticationApiTests.cs`](../src/backend/BlaChallenge.Api.Tests/AuthenticationApiTests.cs) | `Register_with_existing_email_returns_generic_conflict` | Duplicate registration returns `409 Conflict` without exposing the email or an account-existence message. |
| [`TasksApiTests.cs`](../src/backend/BlaChallenge.Api.Tests/TasksApiTests.cs) | `Tasks_require_authentication` | Task endpoints require authentication. |
| [`TasksApiTests.cs`](../src/backend/BlaChallenge.Api.Tests/TasksApiTests.cs) | `Authenticated_user_can_crud_own_task` | An authenticated user can create, read, list, update, and delete their own task. |
| [`TasksApiTests.cs`](../src/backend/BlaChallenge.Api.Tests/TasksApiTests.cs) | `User_cannot_access_another_users_task` | A user cannot read or delete another user's task; the API returns `404 Not Found`. |
| [`TasksApiTests.cs`](../src/backend/BlaChallenge.Api.Tests/TasksApiTests.cs) | `Invalid_task_title_returns_bad_request` | An invalid task title returns `400 Bad Request`. |
| [`TasksApiTests.cs`](../src/backend/BlaChallenge.Api.Tests/TasksApiTests.cs) | `User_cannot_modify_completed_task` | A User cannot delete or modify a task after it reaches `Completed`. |
| [`TasksApiTests.cs`](../src/backend/BlaChallenge.Api.Tests/TasksApiTests.cs) | `Superadmin_can_create_admin_and_admin_can_assign_task_to_user` | Superadmin user management and Admin assignment work through the API. |

## Running The Tests

Run from the repository root:

```sh
dotnet restore src/backend/BlaChallenge.slnx
dotnet build src/backend/BlaChallenge.slnx --no-restore
dotnet test src/backend/BlaChallenge.slnx --no-build
```

The API tests configure an isolated JWT key and temporary SQLite database, so
they do not require a running API process or a repository database file.

The latest run passed 32 test cases: 9 Domain, 8 Application, 4 Infrastructure,
and 11 API cases. The Domain total is higher than the number of methods because
`Create_rejects_empty_title` is an xUnit theory with two inputs.

## Not Covered Yet

- Frontend automated behavior tests are not yet configured. The implemented UI
  requires coverage for route protection, role-aware task actions, Zod form
  validation, API error states, theme persistence, and the Superadmin user flow.
- Detailed user-management application tests and SQLite migration tests remain limited; API coverage verifies the primary Superadmin flow.
- Browser-level or visual responsive tests.
- Automatic seeded demo data and demo-credential verification.
- A published line, branch, or mutation coverage target.
- Load, performance, and deployment tests.

The frontend validation currently consists of `pnpm lint` and `pnpm build`; both
complete successfully with no lint warnings. Add component/browser tests before
delivery and record their results here.
