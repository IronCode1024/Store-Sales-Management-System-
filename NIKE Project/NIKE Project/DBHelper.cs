using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace NIKE_Project
{
    class DBHelper
    {
        //创建SqlConnection对象连接数据库
        SqlConnection conn = new SqlConnection("server=.;database=NIKE;uid=sa;pwd=123456");

        //查询，获取DataTable
        //public DataTable getDataTable(string Sql)
        //{
        //    DataTable table = new DataTable();
        //    SqlDataAdapter dap = new SqlDataAdapter(Sql, conn);
            
        //    dap.Fill(table);
        //    return table;
        //}
        
        //创建DataSet方法
        public DataSet getDataSet(string Sql)
        {
            SqlDataAdapter dap = new SqlDataAdapter(Sql,conn);
            DataSet ds = new DataSet();
            dap.Fill(ds,"nike");
            return ds;
        }

        ////创建登录logons方法
        //public int logons(string Sql)
        //{
        //    conn.Open();
        //    SqlCommand cmd = new SqlCommand(Sql, conn);
        //    int rows1 = (int)cmd.ExecuteScalar();
        //    conn.Close();
        //    return rows1;
        //}

        //创建增删改方法
        public int zsg(string Sql)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand(Sql,conn);
            int rows2 = cmd.ExecuteNonQuery();
            conn.Close();
            return rows2;
        }
    }
}
