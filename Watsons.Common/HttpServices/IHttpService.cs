using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watsons.Common.HttpServices
{
    public interface IHttpService
    {
        Task<Tout?> PostAsnyc<Tin, Tout>(string url, Tin? body, Dictionary<string, dynamic>? headers = null, Dictionary<string, dynamic>? query = null);
        Task<Tout?> GetAsync<Tout>(string url, Dictionary<string, dynamic>? body, Dictionary<string, dynamic>? headers = null);
    }
}
