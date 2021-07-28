using UnityEngine;
using ExcelDataReader;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Collections;
 
namespace excel_d
{
    public class ExcelAccess
    {
    
        //查询menu表
        public static Dictionary<string, Dictionary<string, string>> SelectMenuTable(string file_path)
        {
            string sheetName = "sheet1";
            DataRowCollection collect = ExcelAccess.ReadExcel(file_path, sheetName);
            Dictionary<string, Dictionary<string, string>> excel_map = new Dictionary<string, Dictionary<string, string>>();
    
            for (int i = 1; i < collect.Count; i++)
            {
                string col_id = collect[i][0].ToString();
                excel_map[col_id] = new Dictionary<string, string>();
                foreach(DataColumn column in collect[i].Table.Columns)
                {
                    excel_map[col_id][collect[0][column].ToString()] = collect[i][column].ToString();
                }
            }

            return excel_map;
        }
    
        /// <summary>
        /// 读取 Excel ; 需要添加 Excel.dll; System.Data.dll;
        /// </summary>
        /// <param name="excelName">excel文件名</param>
        /// <param name="sheetName">sheet名称</param>
        /// <returns>DataRow的集合</returns>
        static DataRowCollection ReadExcel(string path, string sheetName)
        {
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
    
            DataSet result = excelReader.AsDataSet();
            //int columns = result.Tables[0].Columns.Count;
            //int rows = result.Tables[0].Rows.Count;
    
            //tables可以按照sheet名获取，也可以按照sheet索引获取
            //return result.Tables[0].Rows;
            return result.Tables[sheetName].Rows;
        }
    }

}
