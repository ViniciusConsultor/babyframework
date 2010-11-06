using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Microsoft.Office.Interop;
//using Microsoft.Office.Interop.Excel;
using Excel;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace Baby.Framework.Office
{
    /// <summary>
    /// Excel帮助类
    /// </summary>
    public class BabyExcelHelper
    {
        private string _templateFile = null;
		private string _outputFile = null;
		private DateTime _beforeTime;
		private DateTime _afterTime;
		private object missing = System.Reflection.Missing.Value;
		private Application _excel;
		private Workbook _wkbook;
		private Worksheet _wksheet;
		private int _sheetCount = 1;
		private string _sheetPrefixName = "页";
		
        /// <summary>
        /// 临时文件路径
        /// </summary>
		public string TemplateFilePath
		{
			set
			{
				this._templateFile = value;
			}
		}
		
        /// <summary>
        /// 输出文件路径
        /// </summary>
		public string OutputFilePath
		{
			set
			{
				this._outputFile = value;
			}
		}
		
        /// <summary>
        /// Sheet文件名
        /// </summary>
		public string SheetPrefixName
		{
			set
			{
				this._sheetPrefixName = value;
			}
		}
		
        /// <summary>
        /// Sheet页数
        /// </summary>
		public int SheetCount
		{
			get
			{
				return this._sheetCount;
			}
		}
		
		/// <summary>
		/// 初始化Excel
		/// </summary>
		/// <param name="templateFilePath">Excel模板路径</param>
		/// <param name="outputFilePath">Excel输出路径</param>
		public BabyExcelHelper(string templateFilePath,string outputFilePath)
		{
			if(templateFilePath == null)
			{
				throw new Exception("Excel模板路径不能为空!");
			}
			if(outputFilePath == null)
			{
				throw new Exception("Excel输出路径不能为空");
			}
			if(!File.Exists(templateFilePath))
			{
				throw new Exception("模板文件不存在！");
			}
			
			this._templateFile = templateFilePath;
			this._outputFile = outputFilePath;
			
			this._beforeTime = DateTime.Now;
			this._excel = new Application();
			this._excel.Visible = true;
			this._afterTime = DateTime.Now;
			
			this._wkbook = this._excel.Workbooks.Open(this._templateFile,missing,missing,missing,missing
			                                          ,missing,missing,missing,missing,missing,
			                                          missing,missing,missing,missing,missing);
			this._wksheet = (Worksheet)this._wkbook.Sheets.get_Item(1);
		}
		
		/// <summary>
		/// 打开已有的Excel
		/// </summary>
		/// <param name="filePath">已有Excel的路径</param>
		public BabyExcelHelper(string filePath)
		{
			if(!File.Exists(filePath))
			{
				throw new Exception("指定路径的文件不存在撒！");
			}
			_beforeTime = DateTime.Now;
            _excel = new Application();
			_excel.Visible = true;
			_afterTime = DateTime.Now;
			
			_wkbook = _excel.Workbooks.Open(filePath,Type.Missing,Type.Missing,Type.Missing,Type.Missing,
			                               Type.Missing,Type.Missing,Type.Missing,Type.Missing,
			                               Type.Missing,Type.Missing,Type.Missing,Type.Missing,
			                               Type.Missing,Type.Missing);
			_wksheet = (Worksheet)_wkbook.Sheets.get_Item(1);
		}
		
		/// <summary>
		/// 新建一个Excel
		/// </summary>
        public BabyExcelHelper()
		{
			//创建一个Application对象并使其可见
			this._beforeTime = DateTime.Now;
            this._excel = new Application();
			this._excel.Visible = true;
			this._afterTime = DateTime.Now;

			//新建一个WorkBook
			this._wkbook = this._excel.Workbooks.Add(Type.Missing);

			//得到WorkSheet对象
			this._wksheet = (Worksheet)_wkbook.Sheets.get_Item(1);
		}
		
		/// <summary>
		/// 从Excel中获取数据（使用Oledb连接）
		/// </summary>
		/// <param name="filePath">Excel文件路径</param>
		/// <returns>填充数据后的DataTable</returns>
		public System.Data.DataTable ExcelToDtByOledb(string filePath)
		{
			string conn_str = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+filePath+";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';";
			string sqlstr = "select * from [{0}$]";
			sqlstr = string.Format(sqlstr,this._wksheet.Name);
			
			DataSet ds = new DataSet();
			
			OleDbConnection oledbConn = new OleDbConnection(conn_str);
			oledbConn.Open();
			
			OleDbDataAdapter oledbDAdapter = new OleDbDataAdapter(sqlstr,oledbConn);
			oledbDAdapter.Fill(ds,this._wksheet.Name);
			
			oledbDAdapter.Dispose();
			oledbConn.Close();
			oledbConn.Dispose();
			
			return ds.Tables[0];
		}
		
		/// <summary>
		/// 从Excel中获取数据（使用Oledb连接）
		/// </summary>
		/// <param name="filePath">Excel文件路径</param>
		/// <param ame="sheetName">sheet名称</param>
		/// <returns>填充数据后的DataTable</returns>
		public System.Data.DataTable ExcelToDtByOledb(string filePath,string sheetName)
		{
			string conn_str = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+filePath+";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';";
			string sqlstr = "select * from [{0}$]";
			sqlstr = string.Format(sqlstr,sheetName);
			
			DataSet ds = new DataSet();
			
			OleDbConnection oledbConn = new OleDbConnection(conn_str);
			oledbConn.Open();
			
			OleDbDataAdapter oledbDAdapter = new OleDbDataAdapter(sqlstr,oledbConn);
			oledbDAdapter.Fill(ds,sheetName);
			
			oledbDAdapter.Dispose();
			oledbConn.Close();
			oledbConn.Dispose();
			
			return ds.Tables[0];
		}
		
		/// <summary>
		/// 从Excel中获取数据（使用Excel组件）
		/// </summary>
		/// <returns>填充数据后的DataTable</returns>
		public System.Data.DataTable ExcelToDtByComponent()
		{
			System.Data.DataTable dt = new System.Data.DataTable();
			for(int i=0;i<this._wksheet.UsedRange.Columns.Count;i++)
			{
				dt.Columns.Add((Convert.ToChar(65+i)).ToString(),typeof(System.String));
			}
			
			for(int i=0;i<this._wksheet.UsedRange.Rows.Count;i++)
			{
				DataRow dr = dt.NewRow();
				for(int j=0;j<this._wksheet.UsedRange.Columns.Count;j++)
				{
					Range rang = this._wksheet.get_Range(this._wksheet.Cells[i+1,j+1],this._wksheet.Cells[i+1,j+1]);
					if(rang.Value2 != null)
					{
						dr[j] = rang.Value2;
						rang = null;
					}
					else
					{
						break;
					}
				}
				dt.Rows.Add(dr);
			}
			dt.TableName = this._wksheet.Name;
			return dt;
		}
		
		/// <summary>
		/// 将DataTable导入Excel
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <param name="rows">每个WorkSheet写入多少行数据</param>
		/// <param name="top">表格数据起始行索引</param>
		/// <param name="left">表格数据起始列索引</param>
		public void DtToExcelByComponent(System.Data.DataTable dt,int rows,int top,int left)
		{
			int rowCount = dt.Rows.Count;
			int colCount = dt.Columns.Count;
			int sheetCount = this.GetSheetCount(rowCount,rows);
			
			for(int i=1;i<sheetCount;i++)
			{
				this._wksheet = (Worksheet)this._wkbook.Worksheets.get_Item(i);
				this._wksheet.Copy(missing,this._wkbook.Worksheets[i]);
			}
			
			for(int i=1;i<sheetCount;i++)
			{
				int starRow = (i-1)*rows;
				int endRow = i*rows;
				
				if(i==sheetCount)
				{
					endRow = rowCount;
				}
				this._wksheet = (Worksheet)this._wkbook.Worksheets.get_Item(i);
				this._wksheet.Name = this._sheetPrefixName+"-"+i.ToString();
				
				int row = endRow-starRow;
				string[,] ss = new string[row,colCount];
				
				for(int j=0;j<row;j++)
				{
					for(int k=0;k<colCount;k++)
					{
						ss[j,k] = dt.Rows[starRow+j][k].ToString();
					}
				}
				
				Range rang = (Range)this._wksheet.Cells[top,left];
				rang = rang.get_Resize(row,colCount);
				rang.Value = ss;
			}
		}
		
		/// <summary>
		/// 将DataTable写入Excel
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <param name="top">表格数据起始行索引</param>
		/// <param name="left">表格数据起始列索引</param>
		public void DtToExcelByComponent(System.Data.DataTable dt,int top,int left)
		{
			int rowCount = dt.Rows.Count;
			int colCount = dt.Columns.Count;
			
			string[,] arr = new string[rowCount,colCount];
			
			for(int j=0;j<rowCount;j++)
			{
				for(int k=0;k<colCount;k++)
				{
					arr[j,k] = dt.Rows[j][k].ToString();
				}
			}
			
			Range rang = (Range)this._wksheet.Cells[top,left];
			rang = rang.get_Resize(rowCount,colCount);
			rang.Value = arr;
		}
		
		public void DtToExcelByComponent(System.Data.DataTable dt,int rows,int top,int left,int mergeColumnIndex)
		{
			
		}
		
		/// <summary>
		/// 计算WorkSheet的总量
		/// </summary>
		/// <param name="rowCount">记录总行数</param>
		/// <param name="rows">每WorkSheet行数</param>
		/// <returns>worksheet 总量</returns>
		public int GetSheetCount(int rowCount,int rows)
		{
			int n = rowCount%rows;
			
			if(n == 0)
			{
				return rowCount/rows;
			}
			else
			{
				return Convert.ToInt32(rowCount/rows)+1;
			}
		}
		
		/// <summary>
		/// 退出Excel
		/// </summary>
		public void Quit()
		{
			if(this._excel.Visible)
			{
				this._wkbook.Close(null,null,null);
				this._excel.Workbooks.Close();
				this._excel.Quit();
			}
		}
		
		/// <summary>
		/// 关闭Excel进程
		/// </summary>
		public void KillExcelProcess()
		{
			Process[] myProcess,myProcess2,allProcess;
			DateTime startTime;
			myProcess = Process.GetProcessesByName("Excel");
			myProcess2 = Process.GetProcessesByName("EXCEL");
            allProcess = Process.GetProcesses();
			
			foreach(Process mp in myProcess)
			{
				startTime = mp.StartTime;
				
				if(startTime > this._beforeTime && startTime < this._afterTime)
				{
					mp.Kill();
				}
			}
			foreach(Process mp in myProcess2)
			{
				startTime = mp.StartTime;
				
				if(startTime > this._beforeTime && startTime < this._afterTime)
				{
					mp.Kill();
				}
			}
            foreach (Process mp in allProcess)
            {
                if (mp.ProcessName.ToLower().Contains("excel"))
                {
                    startTime = mp.StartTime;

                    if (startTime > this._beforeTime && startTime < this._afterTime)
                    {
                        mp.Kill();
                    }
                }
            }
		}
		
		/// <summary>
		/// 回收Excel
		/// </summary>
		public void Dispose()
		{
			this.Quit();
			if(this._wksheet != null)
			{
				System.Runtime.InteropServices.Marshal.ReleaseComObject(this._wksheet);
				this._wksheet = null;
			}
			
			if(this._wkbook != null)
			{
				System.Runtime.InteropServices.Marshal.ReleaseComObject(this._wkbook);
				this._wkbook = null;
			}
			
			if(this._excel != null)
			{
				System.Runtime.InteropServices.Marshal.ReleaseComObject(this._excel);
				this._excel = null;
			}
			
			GC.Collect();
			
			this.KillExcelProcess();
		}

        /// <summary>
        /// 导出Excel(使用Compt组件)
        /// </summary>
        /// <param name="dt">要导出的DataTable</param>
        /// <param name="savePath">保存的路径</param>
        /// <param name="saveName">保存的名称（无需带后缀）</param>
        public void ExportToExcel(System.Data.DataTable dt, string savePath, string saveName)
        {
            if (dt == null) return;
            Application xlApp = new Application();
            if (xlApp == null)
            {
                // lblMsg.Text = "无法创建Microsoft.Office.Interop.Excel对象，可能您的机子未安装Microsoft.Office.Interop.Excel";
                throw new Exception("无法创建Microsoft.Office.Interop.Excel对象，可能您的机子未安装Microsoft.Office.Interop.Excel");
            }
            Workbooks workbooks = xlApp.Workbooks;
            Workbook workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet worksheet = (Worksheet)workbook.Worksheets[1];//取得sheet1
            Range range = null;
            long totalCount = dt.Rows.Count;
            long rowRead = 0;
            float percent = 0;

            //写入标题
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                worksheet.Cells[1, i + 1] = dt.Columns[i].ColumnName;
                range = (Range)worksheet.Cells[1, i + 1];
                //range.Interior.ColorIndex = 15;//背景颜色
                range.Font.Bold = true;//粗体
                range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//居中
                //加边框
                range.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, null);
                //range.ColumnWidth = 4.63;//设置列宽
                //range.EntireColumn.AutoFit();//自动调整列宽
                //r1.EntireRow.AutoFit();//自动调整行高

            }
            //写入内容
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = dt.Rows[r][i];
                    range = (Range)worksheet.Cells[r + 2, i + 1];
                    range.Font.Size = 9;//字体大小
                    //加边框
                    range.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, null);
                    range.EntireColumn.AutoFit();//自动调整列宽
                }
                rowRead++;
                percent = ((float)(100 * rowRead)) / totalCount;
            }

            range.Borders[XlBordersIndex.xlInsideHorizontal].Weight = XlBorderWeight.xlThin;
            if (dt.Columns.Count > 1)
            {
                range.Borders[XlBordersIndex.xlInsideVertical].Weight = XlBorderWeight.xlThin;
            }

            try
            {
                workbook.Saved = true;
                workbook.SaveCopyAs(savePath + saveName + ".xls");
            }
            catch (Exception ex)
            {
                //lblMsg.Text = "导出文件时出错,文件可能正被打开！\n" + ex.Message;
                throw ex;
            }


            workbooks.Close();
            if (xlApp != null)
            {
                xlApp.Workbooks.Close();

                xlApp.Quit();

                int generation = System.GC.GetGeneration(xlApp);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);

                xlApp = null;
                System.GC.Collect(generation);
            }
            GC.Collect();//强行销毁

            KillExcelProcess();
        }

        /// <summary>
        /// 导出Datatable到Excel(使用oledb方式)
        /// </summary>
        /// <param name="dt">要导出的DataTable</param>
        /// <param name="tempFilePath">模板文件完整路径</param>
        /// <param name="savePath">要保存的完整路径(包括后缀名称)</param>
        /// <returns>true 成功；false 失败</returns>
        public static bool ExportToExcelUseCompet(System.Data.DataTable dt, string tempFilePath, string savePath)
        {
            bool success = false;
            string fileName = Path.GetFileNameWithoutExtension(savePath);

            try
            {
                File.Copy(tempFilePath, savePath, true);
            }
            catch
            {
                throw;
            }
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Persist Security Info=True;Data Source=" + savePath + ";Extended Properties=Excel 8.0;";
            OleDbConnection conn = null;
            OleDbCommand cmd = null;
            try
            {
                conn = new OleDbConnection(strConn);
                conn.Open();

                StringBuilder sbTable = new StringBuilder();
                StringBuilder sbInsertSql = new StringBuilder();
                string insertColumn = string.Empty;
                string insertParam = string.Empty;
                sbTable.Append("Create Table [Sheet1$](");

                int i = 0;
                foreach (DataColumn dc in dt.Columns)
                {
                    if ((i++) != 0)
                    {
                        sbTable.Append(",[" + dc.ColumnName + "] Text");
                        insertColumn += ",[" + dc.ColumnName + "]";
                        insertParam += ",?";
                    }
                    else
                    {
                        sbTable.Append("[" + dc.ColumnName + "] Text");
                        insertColumn += "[" + dc.ColumnName + "]";
                        insertParam += "?";
                    }
                }
                sbTable.Append(")");
                sbInsertSql.Append("INSERT INTO [Sheet1$] (" + insertColumn + ") values(" + insertParam + ")");

                cmd = new OleDbCommand("Drop Table [Sheet1$]", conn);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand(sbTable.ToString(), conn);
                cmd.ExecuteNonQuery();

                foreach (DataRow dr in dt.Rows)
                {
                    cmd = new OleDbCommand(sbInsertSql.ToString(), conn);
                    int j = 0;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        OleDbParameter oledbParam = new OleDbParameter((j++).ToString(), dr[dc.ColumnName].ToString());
                        cmd.Parameters.Add(oledbParam);
                    }
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                success = true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    if(conn.State == ConnectionState.Open)
                        conn.Close();

                    conn.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }

            return success;
        }
    }
}
