# ⚡ FAST MODE: OFF
# When set to ON, agents may favor delivery speed over architectural purity and full test coverage.
# The following rules are ALWAYS enforced, even in Fast Mode:
# - No production builds during agent sessions
# - No destructive Prisma resets
# - No breaking changes outside the requested scope
# - No security shortcuts
# - No unrelated refactors

# To enable Fast Mode for a task, change this line to:
# ⚡ FAST MODE: ON

Agents must explicitly state whether they are operating in FAST MODE or NORMAL MODE before making changes.

# AGENTS Guidelines for This Repository

These guidelines define how interactive agents must work on this repository without breaking hot reload, local development, or production safety.


# AGENTS Guidelines for This Repository

These guidelines define how interactive agents must work on this repository without breaking hot reload, local development, or production safety.

⚠️ These principles are **guidelines, not dogma**. For spikes, experiments, UI-only changes, or time-boxed tasks, **delivery speed may temporarily take priority over architectural purity and full test coverage**.

---

# Technologies & How To Run

## Used Technologies
- **Backend:** .NET 9 Web API
- **Frontend:** React + TypeScript (Vite)
- **Vector Store:** Qdrant (Docker)
- **Local Models:** Ollama (optional)

## Build, Run, and Test

### Prerequisites
- .NET 9 SDK
- Node.js 20+
- Docker (for Qdrant)
- Ollama (optional for local models)

### Start Qdrant
```bash
docker-compose up -d
```

### Run the API
```bash
cd src/Dojo.Rag.Api
dotnet run
```

### Run the Frontend
```bash
cd src/Dojo.Rag.UI
npm install
npm run dev
```

### Tests
- .NET tests (if/when added):
```bash
dotnet test
```
- UI tests (if/when added):
```bash
npm test
```

---

## Aggressive Feature Implementation Workflow

When I am asked to implement, extend, or change a **feature**, I must prioritize execution speed and autonomy over interaction.

1. **Auto-locate the feature specification**
   - Immediately search `docs/feature/` for the best matching Markdown file using:
     - Feature name
     - Keywords
     - Related modules
   - If multiple matches exist:
     - Select the most relevant one automatically.
   - If no spec exists:
     - Treat the user request as the authoritative specification and proceed.

2. **Auto-extract requirements**
   - Parse the feature document (or prompt) and internally derive:
     - Core functional requirements
     - Edge cases
     - Constraints
     - Non-functional expectations (performance, security, UX)
   - Do **not** ask for clarification unless the spec directly blocks implementation.

3. **Direct implementation**
   - Immediately begin implementing the full feature across all required layers:
     - Backend
     - Frontend
     - APIs
     - Data model
     - Configuration
   - Apply established project conventions without asking.
   - Implement until the feature is **functionally complete** according to the spec.

4. **Self-verification**
   - Compare the resulting implementation against all extracted requirements.
   - Fix any detected gaps automatically.
   - Add or update tests where possible.

5. **Final validation request**
   - Only after the agent believes the feature is complete:
     - Present a concise summary:
       - What was implemented
       - Files modified
       - Known limitations (if any)
     - Ask the user for a single validation pass.

6. **Documentation update**
   - After user validation:
     - Update the corresponding file in `docs/feature/` to reflect the final implemented behavior.
   - Do not request permission for documentation changes.

7. **Post-completion state**
   - Stop all activity.
   - Wait only for explicit instructions such as:
     - "Create commit"
     - "Prepare PR"
     - "Generate release notes"

---

## Aggressive Bug Fix Workflow

When I am asked to **fix a bug**, I must operate in autonomous resolution mode with minimal user interaction.

1. **Auto-locate the bug report**
   - Search `docs/bugs/new/` and automatically select the closest matching Markdown file using:
     - Stack traces
     - Error messages
     - Affected modules
     - Keywords
   - If no bug file exists:
     - Treat the user report as the bug specification.

2. **Root cause isolation**
   - Analyze the codebase and isolate the most probable root cause immediately.
   - Reconstruct reproduction logic internally when possible.
   - Do **not** ask questions unless the bug cannot be located at all.

3. **Direct fix implementation**
   - Apply the fix immediately.
   - Refactor if necessary to fully eliminate the defect.
   - Add or update regression tests where applicable.

4. **Autonomous validation**
   - Verify that:
     - The original failure path is no longer possible
     - No obvious regressions are introduced

5. **Final validation request**
   - Present a concise fix report:
     - Root cause
     - Fix applied
     - Files changed
   - Ask the user for a single confirmation pass that the issue is resolved.

6. **Documentation closure**
   - After confirmation:
     - Update the bug file with:
       - Root cause
       - Fix summary
       - Fixed version / date
     - Move the file from:
       - `docs/bugs/new/`
       - to `docs/bugs/done/`
   - Do not ask for permission to move the file.

7. **Post-fix state**
   - Stop all activity.
   - Wait only for explicit follow-up instructions such as:
     - "Create commit"
     - "Prepare hotfix notes"
     - "Open a PR"

---

# Clean Code & Software Engineering Principles (Agent Behavior)

These rules define how agents must design, implement, and modify code in this repository.

## Scope Control (Critical)
- **Agents must strictly limit changes to the minimum file and code surface required to solve the task.**
- No unrelated refactors, formatting changes, or cleanups unless explicitly requested.
- Do not “opportunistically improve” surrounding code without a clear requirement.
  - exeptions to this is:
    - if the existing css class can be reused, then missing css file for a module can be added to extract css styles.
    - if existing components could be used but the local code is reinventing similar logic, behavior or style, then the user can be consulted for a quick yes or nor response.

---

## Core Clean Code Principles
- **Readability over cleverness**
- **Single Responsibility Principle (SRP)**
- **KISS (Keep It Simple)**
- **DRY (Don’t Repeat Yourself – without over-abstraction)**
- **Separation of Concerns**
- **Explicit over implicit**
- **Refactoring is continuous, but scoped**

---

## Naming & Structure
- Use **descriptive, intention-revealing names**.
- Functions must be **small, focused, and composable**.
- Avoid generic names like `data`, `handler`, `service`, `utils` unless strongly contextual.
- Prefer **flat module structures** over deep nesting where possible.

---

## Architecture & Dependencies
- Favor **low coupling and high cohesion**.
- Use **Dependency Injection** where appropriate.
- Avoid tight coupling between domain logic and frameworks.
- Do not introduce cyclical dependencies.
- Shared logic belongs in `packages/`, never duplicated across apps.

---

## Error Handling & Reliability
- Never silently swallow errors.
- Exceptions are for exceptional cases only.
- Provide meaningful error messages for logs and monitoring.
- Fail fast when invariants are broken.

---

## Testing Discipline
- New behavior must include appropriate tests unless explicitly waived.
- Prefer **unit tests for business logic**, integration tests for system boundaries.
- Tests must be **deterministic, fast, and isolated**.
- Do not test implementation details—test observable behavior.
- Keep the test pyramid intact.

---

## Performance & Scalability
- Avoid unnecessary re-renders, queries, and data transfers.
- Be aware of **N+1 queries**, blocking I/O, and unbounded loops.
- Use caching and async processing deliberately.
- Never optimize blindly—base optimizations on evidence.

---

## Security by Default
- Validate all external input.
- Treat client input as untrusted.
- Keep authentication and authorization strictly separated.
- Never log secrets, tokens, or credentials.
- Follow the **principle of least privilege**.

---

## Code Quality Gates
- All changes must:
  - Pass type checks
  - Pass linting
  - Keep tests green (unless explicitly waived for spikes)
- Avoid introducing technical debt without explicit justification.
- Prefer deleting unused code over commenting it out.

---

## Decision Priority Order (Conflict Resolution)
When principles conflict, **prioritize strictly in this order**:
1. **System stability**
2. **Data safety**
3. **Correctness**
4. **Clarity**
5. **Testability**
6. **Performance**
7. **Refactoring and optimization**

---

## Professional Engineering Behavior
- Make reasoning explicit in code and commit messages.
- Avoid speculative abstractions.
- Consider maintenance and onboarding cost as first-class constraints.
- Optimize for long-term sustainability over short-term speed.
- Prefer boring, proven solutions over experimental ones unless explicitly requested.

---

This file is binding for all agent-driven code changes in this repository.
