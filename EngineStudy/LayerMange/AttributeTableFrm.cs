using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace EngineStudy.LayerMange
{
    public partial class AttributeTableFrm : Form
    {
        public DataTable attributeTable; 
        public AttributeTableFrm()
        {
            InitializeComponent();
        }
        #region 根据图层字段创建一个只含字段的空DataTable
        /// <summary>
        /// 根据图层字段创建一个只含字段的空DataTable
        /// </summary>
        /// <param name="pLayer"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static DataTable CreateDataTableByLayer(ILayer pLayer, string tableName)
        {
            //创建一个DataTable表
            DataTable pDataTable = new DataTable(tableName);
            
            //取得ITable接口
            ITable pTable = pLayer as ITable;
            IField pField = null;
            DataColumn pDataColumn;
            //根据每个字段的属性建立DataColumn对象
            for (int i = 0; i < pTable.Fields.FieldCount; i++)
            {
                pField = pTable.Fields.get_Field(i);
                //新建一个DataColumn并设置其属性
                pDataColumn = new DataColumn(pField.Name);
                if (pField.Name == pTable.OIDFieldName)
                {
                    pDataColumn.Unique = true;//字段值是否唯一
                }
                //字段值是否允许为空
                pDataColumn.AllowDBNull = pField.IsNullable;
                //字段别名
                pDataColumn.Caption = pField.AliasName;
                //字段数据类型
                pDataColumn.DataType = System.Type.GetType(ParseFieldType(pField.Type));
                //字段默认值
                pDataColumn.DefaultValue = pField.DefaultValue;

                //当字段为String类型是设置字段长度
                if (pField.VarType == 8)
                {
                    pDataColumn.MaxLength = pField.Length;
                }
                //字段添加到表中
                pDataTable.Columns.Add(pDataColumn);
                pField = null;
                pDataColumn = null;
            }
            return pDataTable;
        }

        //因为GeoDatabase的数据类型与.NET的数据类型不同，故要进行转换。转换函数如下：
        /// <summary>
        /// 将GeoDatabase字段类型转换成.Net相应的数据类型
        /// </summary>
        /// <param name="fieldType">字段类型</param>
        /// <returns></returns>
        public static string ParseFieldType(esriFieldType fieldType)
        {
            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeBlob:
                    return "System.String";
                case esriFieldType.esriFieldTypeDate:
                    return "System.DateTime";
                case esriFieldType.esriFieldTypeDouble:
                    return "System.Double";
                case esriFieldType.esriFieldTypeGeometry:
                    return "System.String";
                case esriFieldType.esriFieldTypeGlobalID:
                    return "System.String";
                case esriFieldType.esriFieldTypeGUID:
                    return "System.String";
                case esriFieldType.esriFieldTypeInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeOID:
                    return "System.String";
                case esriFieldType.esriFieldTypeRaster:
                    return "System.String";
                case esriFieldType.esriFieldTypeSingle:
                    return "System.Single";
                case esriFieldType.esriFieldTypeSmallInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeString:
                    return "System.String";
                default:
                    return "System.String";
            }
        }
        #endregion

        #region 填充DataTable中的数据

        /// <summary> 
        /// 填充DataTable中的数据
        /// </summary>
        /// <param name="pLayer"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable CreateDataTable(ILayer pLayer, string tableName)
        {
            //创建空DataTable,并确定表头的名称
            DataTable pDataTable = CreateDataTableByLayer(pLayer, tableName);

            //取得图层类型,如果是shape字段,则表的数据里放该类型名称
            string shapeType = getShapeType(pLayer);

            //创建DataTable的行对象
            DataRow pDataRow = null;
            //从ILayer查询到ITable
            ITable pTable = pLayer as ITable;
            ICursor pCursor = pTable.Search(null, false);
            //取得ITable中的行信息
            IRow pRow = pCursor.NextRow();
            int n = 0;
            while (pRow != null)
            {
                //新建DataTable的行对象
                pDataRow = pDataTable.NewRow();
                for (int i = 0; i < pRow.Fields.FieldCount; i++)
                {
                    //如果字段类型为esriFieldTypeGeometry，则根据图层类型设置字段值
                    if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        pDataRow[i] = shapeType;
                    }
                    //当图层类型为Anotation时，要素类中会有esriFieldTypeBlob类型的数据，
                    //其存储的是标注内容，如此情况需将对应的字段值设置为Element
                    else if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeBlob)
                    {
                        pDataRow[i] = "Element";
                    }
                    else
                    {
                        pDataRow[i] = pRow.get_Value(i);
                    }
                }
                //添加DataRow到DataTable
                pDataTable.Rows.Add(pDataRow);
                pDataRow = null;
                n++;
                //为保证效率，一次只装载最多条记录
                if (n == 2000)
                {
                    pRow = null;
                }
                else
                {
                    pRow = pCursor.NextRow();
                }

            }
            return pDataTable; 

    }


       



        //上面的代码中涉及到一个获取图层类型的函数getShapeType，此函数是通过ILayer判断图层类型的，代码如下：
        /// <summary>
        /// 获得图层的Shape类型
        /// </summary>
        /// <param name="pLayer">图层</param>
        /// <returns></returns>
        public static string getShapeType(ILayer pLayer)
        {
            IFeatureLayer pFeatLyr = (IFeatureLayer)pLayer;
            switch (pFeatLyr.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    return "Point";
                case esriGeometryType.esriGeometryPolyline:
                    return "Polyline";
                case esriGeometryType.esriGeometryPolygon:
                    return "Polygon";
                default:
                    return "";
            }
        }
        #endregion

        #region 绑定DataTable到DataGridView
        ///<summary> 
        /// 绑定DataTable到DataGridView
        /// </summary>
        /// <param name="player"></param>
        public void CreateAttributeTable(ILayer player)
        {
            string tableName;
            tableName = getValidFeatureClassName(player.Name);
            attributeTable = CreateDataTable(player, tableName);

            //gridview的东西不允许用户进行修改
            attributeTable.DefaultView.AllowNew = false;

            //设置数据源
            this.DataGrdView.DataSource = attributeTable;

            this.Text = "属性表[" + tableName + "] " + "记录数：" + attributeTable.Rows.Count.ToString();
        }

        //因为DataTable的表名不允许含有“.”，因此我们用“_”替换。函数如下：
        /// <summary>
        /// 替换数据表名中的点
        /// </summary>
        /// <param name="FCname"></param>
        /// <returns></returns>
        public static string getValidFeatureClassName(string FCname)
        {
            int dot = FCname.IndexOf(".");
            if (dot != -1)
            {
                return FCname.Replace(".", "_");
            }
            return FCname;
        }
        #endregion

        public void showTable(DataTable pDataTable)
        {
            this.DataGrdView.DataSource = pDataTable;
        }
    
        private void setDataSource(IDisplayTable pDisplayTable)
        {
            DataTable pTable = new DataTable();
            IField pField = null;
            DataColumn pColumn;
            for (int i = 0; i < pDisplayTable.DisplayTable.Fields.FieldCount; i++)
            {
                pField = pDisplayTable.DisplayTable.Fields.get_Field(i);
                string name = getRealName(pField);
                pColumn = new DataColumn(name);//delete table name
                //
                MessageBox.Show("Field name:" + name + " " + pField.Name + "; Field aliasName: " + pField.AliasName);
                pColumn.AllowDBNull = pField.IsNullable;
                if (pTable.Columns.Contains(pColumn.ColumnName))
                {
                    pColumn.ColumnName += "1";
                    pTable.Columns.Add(pColumn);
                }
                else
                {
                    pTable.Columns.Add(pColumn);
                }
            }
            ICursor pcusor = pDisplayTable.SearchDisplayTable(null, false);
            IRow pRow;
            pRow = pcusor.NextRow();
            DataRow pDataRow;
            while (pRow != null)
            {
                pDataRow = pTable.NewRow();
                for (int i = 0; i < pRow.Fields.FieldCount; i++)
                {
                    if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        pDataRow[i] = "shapeType";//ensure the type, need to be changed
                    }
                    else if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeBlob)
                    {
                        pDataRow[i] = "Element";
                    }
                    else
                    {
                        pDataRow[i] = pRow.get_Value(i);
                    }
                }
                pTable.Rows.Add(pDataRow);
                pRow = pcusor.NextRow();
            }
            DataGrdView.DataSource = pTable;
            DataGrdView.Refresh();
        }
  
        private string getRealName(IField pfield)
        {
            string pname = pfield.Name;
            string name;
            int dot = pname.IndexOf(".");
            if (dot != -1)
            {
                name = pname.Substring(dot + 1);
                return name;
            }
            else
            {
                return pname;
            }
        }

      
        private void FrmSelectByLocaResult_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DataGrdView.DataSource = null;
            this.DataGrdView.Refresh();
        }

    }
}
