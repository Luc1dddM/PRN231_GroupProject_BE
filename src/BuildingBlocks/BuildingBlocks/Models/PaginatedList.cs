using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Models
{
    public class PaginatedList<T>
    {
        public int PageIndex { get; private set; }
        public int TotalItems { get; private set; }

        public List<T> Items { get; private set; } = new List<T>();

        public PaginatedList(List<T> items, int count, int pageIndex)
        {
            PageIndex = pageIndex;
            TotalItems = count;
            Items.AddRange(items);
        }

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex);
        }
    }
}
