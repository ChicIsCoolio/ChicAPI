using Newtonsoft.Json;
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
        [JsonProperty("status")]
        public Status Status;
        [JsonProperty("data")]
        public T Data;

        public ChicResponse(Status status, T data)
        {
            Status = status;
            Data = data;
        }
    }
}
