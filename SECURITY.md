# Security Policy

## Reporting a Vulnerability

Please do not report security vulnerabilities through public GitHub issues.

Instead, email us at **security@inventing-animals.com**. Include as much detail as you can: what the issue is, how to reproduce it, and what the potential impact might be. We will respond as quickly as possible and keep you informed as we work on a fix.

## Scope

Dash is a dashboard application that can run in multi-user server mode, which means the attack surface includes both the application itself and its server-side deployment. The following are considered security issues:

- Authentication or authorization bypasses in multi-user mode
- Any functionality that collects, transmits, or exposes user data beyond its stated purpose
- Dependencies with known vulnerabilities
- Debugging hooks, backdoors, or diagnostic features left in non-debug builds
