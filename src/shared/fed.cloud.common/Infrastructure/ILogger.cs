using System;

namespace fed.cloud.common.Infrastructure
{
    public interface ILogger
    {
        void Info(string message);
        void Warn(string message);
        void Warn(Exception ex);
        void Debug(string message);
        void Debug(Exception ex);
        void Error(string message);
        void Error(Exception ex);
    }
}
