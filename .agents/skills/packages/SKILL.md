---
name: packages
description: Use when adding, removing, or updating .NET package references.
---

- Use central package management in `Directory.Packages.props`
- Add package versions as `<PackageVersion>` entries there, not in `.csproj` files
- Keep `.csproj` package references versionless unless the project already has a specific exception
- Check existing package names and versions before adding duplicates
