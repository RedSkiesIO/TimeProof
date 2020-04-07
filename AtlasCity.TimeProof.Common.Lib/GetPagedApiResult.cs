using System.Collections.Generic;

namespace AtlasCity.TimeProof.Common.Lib
{
    public class GetPagedApiResult<TModel>
    {
        public int Count { get; set; }

        public string OrderBy { get; set; }

        public string OrderDirection { get; set; }

        public int TotalCount { get; set; }

        public IEnumerable<TModel> Results { get; set; }

        public GetPagedApiResult(int count, string orderBy, string orderDirection, int totalCount, IEnumerable<TModel> result)
        {
            Count = count;
            OrderBy = orderBy;
            OrderDirection = orderDirection;
            Results = result;
            TotalCount = totalCount;
        }
    }
}
