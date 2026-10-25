# Ideas for BLA Challenge

Below is a list of ideas I have for completing the challenge.

## Guidelines

**IMPORTANT:** None of these ideas should conflict with the [challenge](challenge.md) requirements, and every decision must be properly justified.

- Create the initial scaffolding for the projects. For example, for .NET, this includes creating the solution and its respective layers, along with the tests.
- Create a presentation with the help of Gemini (or any other AI tool) and include the prompts I used to create it. The slides will cover all aspects of the application, both for end users and as a technical manual.
- To avoid overcomplicating the project, I should explicitly state that it is not highly complex, has moderate scalability requirements, and can be scaled without issues.
- I already think the best approach would be to create a monorepo, since this is a BFF (backend for frontend).
- There should be a file in the root folder (or another globally accessible location) containing the following, as specified in the README.md:
  - A command to install the project dependencies (for both the frontend and backend), as well as the database (for example, via Docker).
  - A command to start the backend and frontend.
- The following backend structure could work well (initial proposal; see the [implementation plan](implementation-plan.md) for the actual structure):
  ```
  src/
    TaskManager.Domain/
    TaskManager.Application/
    TaskManager.Infrastructure/
    TaskManager.Api/
    task-manager-web/
  tests/
    TaskManager.UnitTests/
    TaskManager.IntegrationTests/
  README.md
  GENAI.md
  ```
- Don't forget that the backend, frontend, and docs (the folder for documentation shared with GenAI) should each have their own README.md, in addition to the global one.
- It would be a good idea to deploy the projects to Azure to demonstrate my experience working with Azure.
