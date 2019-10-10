﻿namespace RX.Nyss.Web.Features.Logging
{
    public interface ILoggerAdapter
    {
        void Debug(object obj);

        void DebugWithCaller(string caller, string message);

        void DebugFormat(string format, params object[] args);

        void Info(object obj);

        void InfoFormat(string format, params object[] args);

        void Error(object obj);

        void ErrorFormat(string format, params object[] args);

        void Warn(object obj);

        void WarnFormat(string format, params object[] args);
    }
}
