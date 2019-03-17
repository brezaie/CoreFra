using System;

namespace CoreFra.Domain
{
    public class BaseEntity<T>
    {
        public T Id { get; set; }
    }
}
