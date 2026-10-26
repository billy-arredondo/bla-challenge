# Design References

This directory stores the frontend designs extracted from Google Stitch. The original HTML mockups are stored as individual files under `stitch/` and can be replaced manually as the designs evolve.

## Screen Mapping

| File | Screen |
| --- | --- |
| `stitch/login.html` | Login |
| `stitch/register.html` | Register |
| `stitch/dashboard.html` | Dashboard |
| `stitch/list-tasks.html` | Task list |
| `stitch/edit-task.html` | Edit task |
| `stitch/delete-task.html` | Delete task confirmation |

The designs have been extracted from Stitch as HTML references. They are not production frontend code and should not be treated as implemented UI, styling, JavaScript, or React components.

## Implementation Notes

The production frontend adapts these references to the current API contract. It
uses React Router, Zustand, TanStack Query, and Zod. The API currently provides
task title, description, status, due date, assignment, creator, and update
metadata; it does not provide priority, tags, comments, projects, or activity
events, so those mockup-only elements are not presented as real data.

The interface supports `User`, `Admin`, and `Superadmin` roles. A responsive
light/dark theme uses the system preference initially and persists an explicit
selection in `localStorage`. The Superadmin user-management screen is a new
composition using the same visual language because no Stitch reference exists
for it.
