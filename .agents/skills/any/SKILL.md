```markdown
# any Development Patterns

> Auto-generated skill from repository analysis

## Overview
This skill teaches the core development patterns and conventions found in the "any" repository, which is written in C# without a specific framework. You'll learn about file naming, import/export styles, commit message patterns, and how to structure and run tests. This guide is ideal for contributors looking to quickly align with the repository's established practices.

## Coding Conventions

### File Naming
- Use **PascalCase** for all file names.
  - **Example:** `UserService.cs`, `OrderProcessor.cs`

### Import Style
- Use **relative imports** to reference other files or namespaces within the project.
  - **Example:**
    ```csharp
    using MyProject.Models;
    using MyProject.Utilities;
    ```

### Export Style
- Use **named exports** for classes, interfaces, and methods.
  - **Example:**
    ```csharp
    public class OrderService
    {
        // Class implementation
    }
    ```

### Commit Message Pattern
- Commit messages are **freeform** and do not follow a strict template.
- Commonly, messages are concise (average 35 characters).
  - **Example:**
    ```
    Fix null reference in OrderProcessor
    Add unit tests for UserService
    ```

## Workflows

### Adding a New Feature
**Trigger:** When implementing a new functionality  
**Command:** `/add-feature`

1. Create a new C# file using PascalCase for the feature.
2. Implement the feature using named exports (public classes/methods).
3. Use relative imports for any dependencies.
4. Write corresponding test files following the `*.test.*` pattern.
5. Commit changes with a concise, descriptive message.

### Fixing a Bug
**Trigger:** When resolving a reported issue or bug  
**Command:** `/fix-bug`

1. Identify the affected file(s).
2. Apply the fix, ensuring code style consistency.
3. Update or add relevant tests in `*.test.*` files.
4. Commit with a short message describing the fix.

### Writing and Running Tests
**Trigger:** When validating new or existing code  
**Command:** `/run-tests`

1. Write test files using the `*.test.*` naming convention (e.g., `OrderService.test.cs`).
2. Use the project's preferred (unknown) testing framework.
3. Run tests according to the project's setup (consult documentation or maintainers if unclear).
4. Review test results and address failures.

## Testing Patterns

- Test files are named using the `*.test.*` pattern, such as `UserService.test.cs`.
- The specific testing framework is not detected; check with maintainers or existing test files for details.
- Tests should cover both typical and edge cases for each feature or bug fix.

**Example Test File:**
```csharp
// UserService.test.cs
using MyProject.Models;
using Xunit;

public class UserServiceTests
{
    [Fact]
    public void CreateUser_ShouldReturnValidUser()
    {
        // Arrange
        var service = new UserService();

        // Act
        var user = service.CreateUser("Alice");

        // Assert
        Assert.NotNull(user);
        Assert.Equal("Alice", user.Name);
    }
}
```

## Commands
| Command      | Purpose                                   |
|--------------|-------------------------------------------|
| /add-feature | Scaffold and implement a new feature      |
| /fix-bug     | Apply and commit a bug fix                |
| /run-tests   | Write and execute tests for the codebase  |
```
