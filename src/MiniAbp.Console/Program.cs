using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using Castle.DynamicProxy;
using MiniAbp.DataAccess;
using MiniAbp.Dependency;
using MiniAbp.Domain;
using MiniAbp.Extension;
using MiniAbp.Logging;
using Newtonsoft.Json;

namespace MiniAbp.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, eventArgs) =>
                {
                    IocManager.Instance.Resolve<ILogger>()
                        .Error(sender.ToString() + eventArgs.ExceptionObject.ToString());
                };
            MiniAbp.StartWithSqlServer(
                "Data Source = 136.17.76.54;Initial Catalog=Yfvic.Bpm0321;Persist Security Info=true;User ID=sa;PWD=Abcd1234;Packet Size=4096;");
            //            DbDapper.RunDataTableSql("");

            string allTableSql = @"--查找所涉及的表及字段
SELECT  TableName = s.SchemaName ,
        ColumnName = c.SchemaName ,
        c.WhereInputType ,
        c.WhereInputContent,
		CASE WHEN  ISNULL(s.ParentBusinessTableId, '') = ''
		THEN 1
		ELSE 
		0
		END AS IsMainTable
		 FROM    dbo.AppBusinessTableColumn c
        INNER JOIN ( SELECT t.Id ,
                            t.SchemaName,
							t.ParentBusinessTableId
                     FROM   dbo.WfdWorkflowNode n
                            INNER JOIN dbo.AppPage p ON n.AppPageId = p.Id
                            INNER JOIN dbo.AppBusinessTable t ON p.TableIds LIKE '%'
                                                              + t.Id + '%'
                     WHERE  n.Id = '{{NodeId}}'
                   ) s ON s.Id = c.BusinessTableId
WHERE   ISNULL(c.WhereInputType, '') <> ''";
            string buildTaskSql = @" 
--创建表
IF NOT EXISTS (SELECT * FROM sys.objects o WHERE o.name = '{{TableName}}')
BEGIN 
	CREATE TABLE [dbo].[{{TableName}}](
		[Id] [NVARCHAR](50) NOT NULL,
		[LanguageCulture] [NVARCHAR](50) NOT NULL,
		[TaskId] [NVARCHAR](50) NOT NULL,
		[GridOrder] [INT] NULL,
	 CONSTRAINT [PK_dbo.{{TableName}}] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END ";
            var buildColumnSql = @"
--字段不存在则生成字段
IF NOT EXISTS(SELECT * FROM sys.[columns] c INNER JOIN sys.[objects] o ON o.[object_id] = c.[object_id] WHERE c.name = '{{ColumnName}}' AND o.name = '{{TableName}}')
BEGIN
ALTER TABLE dbo.[{{TableName}}] ADD [{{ColumnName}}] NVARCHAR(4000) NULL
END";
            //查询并生成多语言 数据表和字段
            var allTableColumn =
                DbDapper.Query<TableColName>(allTableSql.Replace("{{NodeId}}", "14bf7911-a7d9-4623-9a19-cbe60d0c04e4"));
            var allTable = allTableColumn.Select(r => r.TableName).Distinct();
            foreach (var t in allTable)
            {
                //生成表
                var newTbName = t + "_$lang";
                var buildTb = buildTaskSql.Replace("{{TableName}}", newTbName);
                DbDapper.ExecuteNonQuery(buildTb);
                //生成所有字段
                var allColumns = allTableColumn.Where(r => r.TableName == t);
                foreach (var col in allColumns)
                {
                    var buildCol = buildColumnSql.Replace("{{TableName}}", newTbName);
                    buildCol = buildCol.Replace("{{ColumnName}}", col.ColumnName);
                    DbDapper.ExecuteNonQuery(buildCol);
                }
            }


            var addOrUpdateColumnsSql = @"
--蓄水池
--Id/Text
IF NOT EXISTS(SELECT * FROM dbo.{{TableName}} l WHERE l.TaskId = '{{TaskId}}'AND ('{{GridOrder}}' = '' OR '{{GridOrder}}' = l.GridOrder AND l.LanguageCulture = '{{LanguageCulture}}')
BEGIN 
INSERT INTO dbo.yfvic_fin05_quot_$lang(Id ,LanguageCulture, TaskId, GridOrder, {{Columns}})VALUES (NEWID(), '{{LanguageCulture}}', '{{TaskId}}', '{{ColumnValues}}')
END
ELSE 
BEGIN 
UPDATE dbo.{{TableName}} SET  {{ColumnPair}} WHERE TaskId = '{{TaskId}}'AND ('{{GridOrder}}' = '' OR '{{GridOrder}}' = GridOrder AND LanguageCulture = '{{LanguageCulture}}'
END ";

            var executeSql = addOrUpdateColumnsSql;
            executeSql = executeSql.Replace("{{TableName}}", "");
            executeSql = executeSql.Replace("{{TaskId}}", "");
            executeSql = executeSql.Replace("{{LanguageCulture}}", "");
            executeSql = executeSql.Replace("{{GridOrder}}", "");
            executeSql = executeSql.Replace("{{Columns}}", "");
            executeSql = executeSql.Replace("{{ColumnValues}}", "");
            executeSql = executeSql.Replace("{{ColumnPair}}", "");
            DbDapper.ExecuteNonQuery(executeSql);
        }
    }



    public class TableColName
        {
            public string TableName { get; set; }
            public string ColumnName { get; set; }
            public string WhereInputType { get; set; }
            public string WhereInputContent { get; set; }
            public bool IsMainTable { get; set; }
        }

}
