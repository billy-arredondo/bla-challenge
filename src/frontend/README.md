# TaskPulse frontend

React/Vite frontend for the Bla Challenge task workspace. It uses React Router,
Zustand, TanStack Query, and Zod and connects to the ASP.NET API under `/api`.

## Development

Start the API on `http://localhost:5278`, then run:

```sh
pnpm install
pnpm dev
```

The Vite development server proxies `/api` to the API. Override the target with
`VITE_API_URL` when required.

```sh
pnpm lint
pnpm build
```

## Frontend structure

The application is organized by responsibility and feature rather than keeping
pages and data access in one component:

- `src/app` contains routing, route guards, session hydration, and global theme synchronization.
- `src/layouts` contains the authenticated workspace and authentication shells.
- `src/features/auth`, `src/features/dashboard`, `src/features/tasks`, and `src/features/users` contain feature pages and feature-specific behavior.
- `src/components` contains reusable visual building blocks.
- `src/lib` contains API access, contracts, constants, and formatting utilities.
- `src/stores` contains independent session, theme, and permission modules.
- `src/styles` contains design tokens, layout, shared components, feature styles, and responsive rules.

Components and functions use multiline formatting so rendering, event handling,
and data operations remain easy to scan and test independently.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Oxc](https://oxc.rs)
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/)

## React Compiler

The React Compiler is not enabled on this template because of its impact on dev & build performances. To add it, see [this documentation](https://react.dev/learn/react-compiler/installation).

## Expanding the Oxlint configuration

If you are developing a production application, we recommend enabling type-aware lint rules by installing `oxlint-tsgolint` and editing `.oxlintrc.json`:

```json
{
  "$schema": "./node_modules/oxlint/configuration_schema.json",
  "plugins": ["react", "typescript", "oxc"],
  "options": {
    "typeAware": true
  },
  "rules": {
    "react/rules-of-hooks": "error",
    "react/only-export-components": ["warn", { "allowConstantExport": true }]
  }
}
```

See the [Oxlint rules documentation](https://oxc.rs/docs/guide/usage/linter/rules) for the full list of rules and categories.
