using System;
using System.Collections.Generic;

namespace Dash.Client.Api;

public sealed record PageData(Guid PageId, string Name, IReadOnlyList<WidgetData> Widgets);

public sealed record WidgetData(Guid WidgetId, string WidgetType, string ConfigurationJson);
