using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Api.Helpers;

namespace Wallet.Api.Extensions
{
    public static class PaginationExtension
    {
        public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

            response.Headers.Add("Pagination",
               Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));
            response.Headers.Add("access-control-expose-headers", "Pagination");
        }
    }
}
