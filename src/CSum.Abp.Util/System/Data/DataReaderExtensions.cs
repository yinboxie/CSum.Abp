using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace System.Data
{
    public static class DataReaderExtensions
    {
        #region 将IDataReader转换List<T>集合
        /// <summary>
        /// 将IDataReader转换为 集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> DrToList<T>(this IDataReader reader) where T: new()
        {
            var field = new List<string>(reader.FieldCount);
            for (var i = 0; i < reader.FieldCount; i++) field.Add(reader.GetName(i).ToLower());
            var list = new List<T>();
            while (reader.Read())
            {
                var model = Activator.CreateInstance<T>();
                foreach (var property in model.GetType()
                    .GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                {
                    var dname = property.Name;
                    var drValue = reader[dname];
                    if (field.Contains(dname.ToLower()))
                    {
                        if (!(drValue == null || drValue is DBNull))
                            property.SetValue(model, drValue.To(property.PropertyType), null);
                    }
                }

                list.Add(model);
            };

            return list;
        }

        #endregion

        #region 将IDataReader转换为DataTable
        /// <summary>
        /// 将IDataReader转换为DataTable
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this IDataReader reader)
        {
            var dt = new DataTable("Table");
            while (reader.Read())
            {
                var count = reader.FieldCount;
                var row = dt.NewRow();
                for (var i = 0; i < count; i++)
                {
                    if (!dt.Columns.Contains(reader.GetName(i)))
                        dt.Columns.Add(new DataColumn(reader.GetName(i)));
                    if (reader.GetFieldType(i) == typeof(string) && reader.IsDBNull(i))
                        row[i] = "";
                    else
                        row[i] = reader.GetValue(i);
                }

                dt.Rows.Add(row);
            }
            reader.Close();
            return dt;
        }

        #endregion

        #region 将IDataReader转换为DataSet
        /// <summary>
        /// 将IDataReader转换为DataSet
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static DataSet ToDataSet(this IDataReader reader)
        {
            var dataSet = new DataSet();
            do
            {
                // Create new data table 
                var schemaTable = reader.GetSchemaTable();
                var dataTable = new DataTable();

                if (schemaTable != null)
                {
                    // A query returning records was executed 

                    for (var i = 0; i < schemaTable.Rows.Count; i++)
                    {
                        var dataRow = schemaTable.Rows[i];
                        // Create a column name that is unique in the data table 
                        var columnName =
                            (string)dataRow["ColumnName"]; //+ " // Add the column definition to the data table 
                        var column = new DataColumn(columnName, (Type)dataRow["DataType"]);
                        dataTable.Columns.Add(column);
                    }

                    dataSet.Tables.Add(dataTable);

                    // Fill the data table we just created 

                    while (reader.Read())
                    {
                        var dataRow = dataTable.NewRow();
                        for (var i = 0; i < reader.FieldCount; i++)
                            if (reader.GetFieldType(i) == typeof(string) && reader.IsDBNull(i))
                                dataRow[i] = "";
                            else
                                dataRow[i] = reader.GetValue(i);
                        dataTable.Rows.Add(dataRow);
                    }
                }
                else
                {
                    // No records were returned 
                    var column = new DataColumn("RowsAffected");
                    dataTable.Columns.Add(column);
                    dataSet.Tables.Add(dataTable);
                    var dataRow = dataTable.NewRow();
                    dataRow[0] = reader.RecordsAffected;
                    dataTable.Rows.Add(dataRow);
                }
            } while (reader.NextResult());

            return dataSet;
        }
        #endregion
    }
}
