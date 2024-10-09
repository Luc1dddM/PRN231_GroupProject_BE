using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Models
{
    public class BaseResponse<T>
    {
        public BaseResponse()
        {
        }

        public BaseResponse(T result, string message = null)
        {
            this.Result = result;
            this.IsSuccess = true;
            this.Message = message;
        }

        public BaseResponse(string errorMessage)
        {
            this.IsSuccess = false;
            this.Message = errorMessage;
        }


        public bool IsSuccess { get; set; }
        public T Result { get; set; }
        public string Message { get; set; }
    }
}
