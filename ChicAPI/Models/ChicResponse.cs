using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChicAPI.Models
{
    public enum Status
    {
        NOT_READY,
        OK
    }

    public struct ChicResponse<T>
    {
        public Status Status { get; set; }
        public T Data { get; set; }

        public ChicResponse(Status status, T data)
        {
            Status = status;
            Data = data;
        }
    }
}
