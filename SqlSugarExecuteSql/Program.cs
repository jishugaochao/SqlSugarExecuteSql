using SqlSugar;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SqlSugarExecuteSql
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlSugarClient db = new SqlSugarClient(
             new ConnectionConfig()
             {
                 ConnectionString = "连接字符串***",
                 DbType = DbType.SqlServer,//设置数据库类型
                 IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                 InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
             });

            var dt = db.Ado.GetDataTable("Sql语句***",
                new List<SugarParameter>(){
                  new SugarParameter("@name","1")
                });
            List<StudentModel> studentModels = new List<StudentModel>();
            studentModels = ConvertToList(dt);
            Console.WriteLine("Hello World!");

        }

        //DataTable转成List
        public static List<StudentModel> ConvertToList(DataTable dt)
        {
            // 定义集合  
            List<StudentModel> ts = new List<StudentModel>();

            // 获得此模型的类型  
            Type type = typeof(StudentModel);
            //定义一个临时变量  
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行  
            foreach (DataRow dr in dt.Rows)
            {
                StudentModel t = new StudentModel();
                // 获得此模型的公共属性  
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性  
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量  
                    //检查DataTable是否包含此列（列名==对象的属性名）    
                    if (dt.Columns.ContainsKey(tempName))
                    {
                        // 判断此属性是否有Setter  
                        if (!pi.CanWrite) continue;//该属性不可写，直接跳出  
                        //取值  
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性  
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                //对象添加到泛型集合中  
                ts.Add(t);
            }
            return ts;
        }
    }
}
