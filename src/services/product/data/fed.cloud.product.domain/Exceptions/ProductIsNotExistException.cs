using System;

namespace fed.cloud.product.domain.Exceptions
{
    public class ProductIsNotExistException : Exception
    {
        public ProductIsNotExistException()
        {
        }

        public ProductIsNotExistException(string @string) : base(@string)
        {
        }

        public ProductIsNotExistException(string @string, Exception innerException) : base(@string, innerException)
        {
        }
    }
}