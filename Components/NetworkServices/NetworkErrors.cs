using DomainModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;

namespace NetworkServices
{
    public static class NetworkErrors
    {
        public static InnerErrorDto InternetConnectionError => new InnerErrorDto() { ErrorMessage = "Unable to connect to the internet.Please check your connection and try again.", ErrorCode = 13};
        public static InnerErrorDto HttpStatusCodeError(HttpStatusCode statusCode) => new InnerErrorDto() { ErrorMessage = $"The request returned an unsuccessful status code. Status code: {statusCode}", ErrorCode = 14 };
        public static InnerErrorDto HttpRequestError(HttpRequestException exception) => new InnerErrorDto() { ErrorMessage = "Unable to reach the server. It might be down or temporarily unavailable. Please try again later.", ErrorDescription = $"{exception.Message}\n{exception.HttpRequestError}", ErrorCode = 15 };
        public static InnerErrorDto TimeoutError(TaskCanceledException exception) => new InnerErrorDto() { ErrorMessage = "The server took too long to respond.", ErrorDescription = $"{exception.Message}\n{exception.InnerException}", ErrorCode = 16 };
        public static InnerErrorDto JsonError(JsonException exception) => new InnerErrorDto() { ErrorMessage = "We couldn't read the data from the server. Please try again later or contact support..", ErrorDescription = $"{exception.Message}\n{exception.BytePositionInLine}", ErrorCode = 17 };
    }
}
