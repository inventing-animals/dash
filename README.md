# Dash

> [!WARNING]
> This project is in very early, heavy development. Expect breaking changes, missing features, and rough edges. It is not ready for use.

Information dashboard application built with [Avalonia](https://avaloniaui.net/) and [Ink](https://github.com/inventing-animals/ink). Runs locally as a desktop app or on a server with multi-user support, with a WASM demo available in the browser.

**[Documentation](https://inventing-animals.github.io/dash/)** | **[Live demo](https://inventing-animals.github.io/dash/demo/)**

## Solution Layout

- `src/Dash.Client/`: master folder for the shared Avalonia client project and platform-specific hosts.
- `src/Dash.Server/`: master folder for the ASP.NET Core health API scaffold, persistence readiness checks, and observability wiring.
- `src/Dash.WidgetSdk/`: public widget SDK contracts for configuration, state envelopes, Avalonia rendering, server execution, and shared JSON serialization.
- `src/Dash.Widgets/`: isolated widget implementations. Widgets depend on the SDK, not on each other.

## Widget Shape

- `DigitalClock` demonstrates a client-only widget: the server only needs to persist its configuration.
- `EmailChecker` demonstrates a client-and-server widget: the server produces JSON state and the client renders it.
- `Dash.WidgetSdk.Avalonia` is the client rendering boundary. Widgets return Avalonia controls and the app provides styling later.
- `Dash.Client.WidgetHost` and `Dash.Server.WidgetHost` are lightweight catalogs for Avalonia and server widget implementations.


## Contact

- Security isssues: security@inventing-animals.com
- Talk to us at: hello@inventing-animals.com

## License

MIT
