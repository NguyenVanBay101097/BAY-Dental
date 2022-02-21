using JetBrains.Annotations;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;

namespace Microsoft.AspNetCore.Http
{
    public static class AbpHttpRequestExtensions
    {
        public static bool IsAjax([NotNull] this HttpRequest request)
        {
            Check.NotNull(request, nameof(request));

            return string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
                   string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal);
        }

        public static bool CanAccept([NotNull] this HttpRequest request, [NotNull] string contentType)
        {
            Check.NotNull(request, nameof(request));
            Check.NotNull(contentType, nameof(contentType));

            return request.Headers[HeaderNames.Accept].ToString().Contains(contentType, StringComparison.OrdinalIgnoreCase);
        }
    }
}
