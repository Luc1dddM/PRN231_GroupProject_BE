using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Catalog.API.Categories.GetCategories;
using Catalog.API.Models.DTO;
using Catalog.API.Repository;
using System.Data;

namespace Catalog.API.Catagories.ExportCategories
{
    public record ExportCategoriesQuery(ExportListParamsDto parameters) : IQuery<ExportCategoriesRessult>;

    public record ExportCategoriesRessult(BaseResponse<DataTable> response);


    internal class ExportCategoriesHandler:IQueryHandler<ExportCategoriesQuery, ExportCategoriesRessult>
    {
        private readonly ICategoryRepository _categoryRepository;
        public ExportCategoriesHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<ExportCategoriesRessult> Handle(ExportCategoriesQuery query, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.ExportCategoriesFilter(query.parameters);

            DataTable dt = new DataTable();
            dt.TableName = "CategoriesTable";
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("Created By", typeof(string));
            dt.Columns.Add("Created Date", typeof(string));
            dt.Columns.Add("Updated By", typeof(string));
            dt.Columns.Add("Updated Date", typeof(string));



            foreach (var item in categories)
            {
                DataRow row = dt.NewRow();
                row[0] = item.Name;
                row[1] = item.Type;
                row[2] = item.Status ? "Active" : "In Active";
                row[3] = item.CreatedBy;
                row[4] = item.CreatedAt;
                row[5] = item.UpdatedBy;
                row[6] = item.UpdatedAt;
                dt.Rows.Add(row);
            }

            
            return new ExportCategoriesRessult(new BaseResponse<DataTable>(dt));
        }
    }
}
