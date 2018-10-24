using ClassLibrary.Pagination.Paginationv2.Installer;
using ClassLibrary.Pagination.Paginationv2.Condition;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using ClassLibrary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using ClassLibrary.Pagination.MultipleResultStoreProc;
using System.Web;

namespace ClassLibrary.Pagination.Paginationv2
{
    public class Pagination
    {
        private DbContext db;
        private int page = 0 ;
        private int pagelimit = 0;
        private string filter = "";
        private string name = "";
        public Pagination(DbContext db_)
        {
            db = db_;
            Starter.go(db);
        }
        public Response<T> QueryPagination<T>(string TableOrView, SQLWhere where = null, params SQLSort[] sort)
        {
            GetParamsRequest();
            string get_where = where == null ? "" : where.GetWhere;
            string get_column = name;
            string get_value = filter;
            string get_sort = GetSort(sort);
            var res = new MultipleResultStore(db);
            res.Execute("EXEC [dbo].[s_Pagination] @TableOrView,@Where,@FilterColumn,@FilterValue,@Page,@Limit_page,@Sortby",
                new SqlParameter("@TableOrView", TableOrView),
                new SqlParameter("@Where", get_where),
                new SqlParameter("@FilterColumn", get_column),
                new SqlParameter("@FilterValue", get_value),
                new SqlParameter("@Page", page),
                new SqlParameter("@Limit_page", pagelimit),
                new SqlParameter("@Sortby", get_sort)
            );
            var item = res.GetData<T>();
            var count = res.GetData<resObject>().FirstOrDefault();
            return new Response<T>
            {
                currentPage = page,
                limitRow = pagelimit,
                resultRow = count.Count_row,
                items = item
            };

        }
        public ResponseJSON QueryPagination(string TableOrView, SQLWhere where = null, params SQLSort[] sort)
        {
            GetParamsRequest();
            string get_where = where == null ? "" : where.GetWhere;
            string get_column = name;
            string get_value = filter;
            string get_sort = GetSort(sort);
            var res = db.Database.SqlQuery<resJson>("EXEC [dbo].[s_PaginationJSON] @TableOrView,@Where,@FilterColumn,@FilterValue,@Page,@Limit_page,@Sortby",
                   new SqlParameter("@TableOrView", TableOrView),
                   new SqlParameter("@Where", get_where),
                   new SqlParameter("@FilterColumn", get_column),
                   new SqlParameter("@FilterValue", get_value),
                   new SqlParameter("@Page", page),
                   new SqlParameter("@Limit_page", pagelimit),
                   new SqlParameter("@Sortby", get_sort)
               ).FirstOrDefault();
            if (res.Item != null)
            {
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                var result = json_serializer.DeserializeObject(res.Item);
                return new ResponseJSON
                {
                    currentPage = page,
                    limitRow = pagelimit,
                    resultRow = res.Count_row,
                    items = result
                };
            }
            else
            {
                return new ResponseJSON
                {
                    currentPage = page,
                    limitRow = pagelimit,
                    resultRow = res.Count_row,
                    items = new object[] { }
                };
            }
        }
        public object QueryJSON(string TableOrView, SQLWhere where = null, params SQLSort[] sort)
        {
            string get_where = where == null ? "" : where.GetWhere;
            string get_sort = GetSort(sort);
            var res = db.Database.SqlQuery<resJson>("EXEC [dbo].[s_PaginationJSON] @TableOrView,@Where,@FilterColumn,@FilterValue,@Page,@Limit_page,@Sortby",
                    new SqlParameter("@TableOrView", TableOrView),
                    new SqlParameter("@Where", get_where),
                    new SqlParameter("@FilterColumn", ""),
                    new SqlParameter("@FilterValue", ""),
                    new SqlParameter("@Page", page),
                    new SqlParameter("@Limit_page", pagelimit),
                    new SqlParameter("@Sortby", get_sort)
                ).FirstOrDefault();
            if (res.Item != null)
            {
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                var result = json_serializer.DeserializeObject(res.Item);
                return result;
            }
            return new object[] { };
        }
        private string GetSort(SQLSort[] sort)
        {
            string res = "";
            int i = 0;
            foreach (var sorts in sort)
            {
                if (i == 0)
                    res += sorts.GetSort;
                else
                    res += " , " + sorts.GetSort;
                i++;
            }
            return res;
        }
        private void GetParamsRequest()
        {
            HttpRequest Request = HttpContext.Current.Request;
            page = string.IsNullOrEmpty(Request["page"]) ? 1 : int.Parse(Request["page"]);
            pagelimit = string.IsNullOrEmpty(Request["limit"]) ? 15 : int.Parse(Request["limit"]);
            filter = string.IsNullOrEmpty(Request["filter"]) ? "" : Request["filter"];
            name = string.IsNullOrEmpty(Request["name"]) ? "" : Request["name"];
        }

    }
    public class resJson
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public int Count_row { get; set; }
    }
    public class resObject {
        public int Id { get; set; }
        public int Count_row { get; set; }
    }

}
