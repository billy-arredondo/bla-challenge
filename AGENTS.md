# Repository guidance

- All repository content must be written in English, even if some existing content is in Spanish, the developer's native language.
- Follow `docs/contributing.md` for branch names, commit messages, and PR titles/descriptions.
- Start with `docs/challenge.md` for requirements. `docs/brainstorming.md` contains proposals, not an implemented architecture; its folder tree, BFF label, and Azure deployment idea are not established facts.
- The repository currently contains only those documents and an empty `src/`. There are no manifests, build/test commands, or configured toolchains yet; verify these after scaffolding rather than assuming a stack or commands.
- The challenge prohibits Entity Framework, Dapper, and “Mediator.” The last term is ambiguous about library versus pattern; do not assume a homemade mediator is allowed. CQRS can use direct calls to use cases.
- The backend must use .NET/C# and Clean Architecture, with business rules independent of persistence and API code. The frontend framework and data store are not selected yet.
- Plan tests alongside implementation: the challenge calls for coverage of data access, business logic, and API endpoints, and introduces TDD as the development approach.
- Preserve evidence of GenAI work as it happens: prompts, representative generated code, validation, corrections, and handling of edge cases/authentication. This is part of the assessed deliverable, not just optional documentation.
- Delivery must include an informal user story, setup instructions, seeded demo data/credentials, and a presentation demonstrating and explaining the implementation. Consult the challenge for the full acceptance criteria.
