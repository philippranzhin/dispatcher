﻿namespace DispatcherDesktop.Domain.Logger
{
    using System;

    public interface ILogger
    {
        event EventHandler<string> InfoLogged;
        event EventHandler<string> ErrorLogged;

        void LogInfo(string log);

        void LogError(string log);
    }
}
