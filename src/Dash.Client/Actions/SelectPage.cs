using System;

namespace Dash.Client.Core;

public sealed record SelectPage(Guid PageId) : ClientAction;