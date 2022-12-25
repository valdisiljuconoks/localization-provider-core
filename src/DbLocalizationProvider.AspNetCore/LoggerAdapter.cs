// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Microsoft.Extensions.Logging;
using ILogger = DbLocalizationProvider.Logging.ILogger;

namespace DbLocalizationProvider.AspNetCore;

public class LoggerAdapter : ILogger
{
    private readonly Microsoft.Extensions.Logging.ILogger _logger;

    public LoggerAdapter(Microsoft.Extensions.Logging.ILogger logger)
    {
        _logger = logger;
    }

    public void Debug(string message)
    {
        _logger?.LogDebug(message);
    }

    public void Info(string message)
    {
        _logger?.LogInformation(message);
    }

    public void Error(string message)
    {
        _logger?.LogError(message);
    }

    public void Error(string message, Exception exception)
    {
        _logger?.LogError(message, exception);
    }
}
