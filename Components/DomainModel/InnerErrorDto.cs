using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModel
{
    public sealed class InnerErrorDto
    {
        public string ErrorMessage { get; set; } = string.Empty;
        public string? ErrorDescription { get; set; }
        public int ErrorCode { get; set; }
    }
}
