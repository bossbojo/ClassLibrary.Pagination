# Pagination
 
 ## QueryPagination
 ```
  //QueryPagination<T>(string TableOrView, SQLWhere where = null, params SQLSort[] sort)
  
   var res = page.QueryPagination<Staff>("backoffice.Staff",
                   new SQLWhere("Id = 4"),
                   new SQLSort("CompaniesId", "DESC")
                  );
  //QueryPagination(string TableOrView, SQLWhere where = null, params SQLSort[] sort)
    var res = page.QueryPagination("backoffice.Staff",
                   new SQLWhere("Id = 4"),
                   new SQLSort("CompaniesId", "DESC")
                  );
 ```
  
