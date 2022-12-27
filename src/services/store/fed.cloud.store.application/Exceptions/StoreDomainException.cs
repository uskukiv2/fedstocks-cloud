using System;

namespace fed.cloud.store.application.Exceptions;

public class StoreDomainException : Exception
{
    public StoreDomainException()
    {
    }

    public StoreDomainException(string @string) : base(@string)
    {
    }

    public StoreDomainException(string @string, Exception innerException) : base(@string, innerException)
    {
    }
}