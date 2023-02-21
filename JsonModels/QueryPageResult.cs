using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTeachingInstall.JsonModels
{
    public class QueryPageResult<T> : ResponseData
    {
        public int page { get; set; }

        public int size { get; set; }

        public int count { get; set; }

        public int totalPages { get; set; }

        public long totalElements { get; set; }

        public List<T> contents { get; set; }

    }
}
