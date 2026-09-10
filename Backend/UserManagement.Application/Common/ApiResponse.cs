using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
        public List<string> Errors { get; set; }
        public DateTime ResponseTime { get; set; }

        public static ApiResponse<T> SuccessResponse(T? data, string message = "Success"){
        
            return new() { Success = true, Message = message, Result = data,ResponseTime = DateTime.Now };
        }

        public static ApiResponse<T> FailureResponse(string message, IEnumerable<string>? errors = null)
        {
            return new() { Success = false, Message = message, Errors = errors?.ToList() ?? new List<string>(), ResponseTime = DateTime.Now };
        }
    }
}
