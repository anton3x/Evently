using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Evently.Modules.Events.Application.Abstractions.Data;

public interface IDbConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync();
}
