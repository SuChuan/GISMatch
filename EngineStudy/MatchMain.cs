using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EngineStudy.LayerMange;
using EngineStudy.StatisQuery;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;

namespace EngineStudy
{
    public partial class MatchMain : Form
    {
        private string sMapUnits;
        //TOCControl控件变量 
        private ITOCControl2 m_tocControl = null;
        //TOCControl中Map菜单
        private IToolbarMenu m_menuMap = null;
        //TOCControl中图层菜单
        private IToolbarMenu m_menuLayer = null;
        public static IMapControl3 m_mapControl = null;
        public static IPageLayoutControl2 m_pageLayoutControl = null;
        private int LayerIndex = -1;
        public MatchMain()
        {
            InitializeComponent();
        }

        private void axPageLayoutControl1_OnMouseDown(object sender,
            ESRI.ArcGIS.Controls.IPageLayoutControlEvents_OnMouseDownEvent e)
        {

        }

        private void axMapControl1_OnMouseMove(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseMoveEvent e)
        {
            // 显示当前比例尺 
            this.SclaleLable.Text = " 比例尺 1:" + ((long)this.axMapControl1.MapScale).ToString();
            // 显示当前坐标 
            this.CoordinateLable.Text = " 当前坐标 X = " + e.mapX.ToString() + " Y = " + e.mapY.ToString() + " " + this.axMapControl1.MapUnits.ToString().Substring(4);
                                        //sMapUnits;

        }

        private void axToolbarControl1_OnMouseDown(object sender, IToolbarControlEvents_OnMouseDownEvent e)
        {

        }

        private void axToolbarControl1_OnMouseMove(object sender, IToolbarControlEvents_OnMouseMoveEvent e)
        {
            // 取得鼠标所在工具的索引号 
            int index = axToolbarControl1.HitTest(e.x, e.y, false);
            if (index != -1)
            {
                // 取得鼠标所在工具的 ToolbarItem 
                IToolbarItem toolbarItem = axToolbarControl1.GetItem(index);
                // 设置状态栏信息 
                this.MessageLable.Text = toolbarItem.Command.Message;
            }
            else
            {
                MessageLable.Text = " 就绪 ";
            }
        }

        private void MatchMain_Load(object sender, EventArgs e)
        {
            sMapUnits = "未知单位";
            m_mapControl = (IMapControl3)this.axMapControl1.Object;
            m_pageLayoutControl = (IPageLayoutControl2)this.axPageLayoutControl1.Object;
            m_tocControl = (ITOCControl2)this.axTOCControl1.Object;
            m_menuMap = new ToolbarMenuClass();
            m_menuLayer = new ToolbarMenuClass();
            //添加自定义菜单项到TOCCOntrol的Map菜单中 
            //打开文档菜单
       //   m_menuMap.AddItem(new OpenNewMapDocument(m_controlsSynchronizer), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            //添加数据菜单
            //-----    m_menuMap.AddItem(new ControlsAddDataCommandClass(), -1, 1, false, esriCommandStyles.esriCommandStyleIconAndText);
            //打开全部图层菜单
            //m_menuMap.AddItem(new LayerVisibility(), 1, 2, false, esriCommandStyles.esriCommandStyleTextOnly);
            //关闭全部图层菜单
            //m_menuMap.AddItem(new LayerVisibility(), 2, 3, false, esriCommandStyles.esriCommandStyleTextOnly);
            //以二级菜单的形式添加内置的“选择”菜单
            //m_menuMap.AddSubMenu("esriControls.ControlsFeatureSelectionMenu", 4, true);
            //以二级菜单的形式添加内置的“地图浏览”菜单
            //m_menuMap.AddSubMenu("esriControls.ControlsMapViewMenu", 5, true);

            //添加自定义菜单项到TOCCOntrol的图层菜单中            
           
           // 添加“移除图层”菜单项
            m_menuLayer.AddItem(new RemoveLayer(), -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
           // 添加“放大到整个图层”菜单项
            m_menuLayer.AddItem(new ZoomToLayer(), -1, 1, true, esriCommandStyles.esriCommandStyleTextOnly);
          

           //设置菜单的Hook
            m_menuLayer.SetHook(m_mapControl);
            m_menuMap.SetHook(m_mapControl);
        }

        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            esriUnits mapUnits = axMapControl1.MapUnits;
            switch (mapUnits)
            {
                case esriUnits.esriCentimeters:
                    sMapUnits = "Centimeters";
                    break;
                case esriUnits.esriDecimalDegrees:
                    sMapUnits = "Decimal Degrees";
                    break;
                case esriUnits.esriDecimeters:
                    sMapUnits = "Decimeters";
                    break;
                case esriUnits.esriFeet:
                    sMapUnits = "Feet";

                    break;
                case esriUnits.esriInches:
                    sMapUnits = "Inches";
                    break;
                case esriUnits.esriKilometers:
                    sMapUnits = "Kilometers";
                    break;
                case esriUnits.esriMeters:
                    sMapUnits = "Meters";
                    break;
                case esriUnits.esriMiles:
                    sMapUnits = "Miles";
                    break;
                case esriUnits.esriMillimeters:
                    sMapUnits = "Millimeters";
                    break;
                case esriUnits.esriNauticalMiles:
                    sMapUnits = "NauticalMiles";
                    break;
                case esriUnits.esriPoints:
                    sMapUnits = "Points";
                    break;
                case esriUnits.esriUnknownUnits:
                    sMapUnits = "Unknown";
                    break;
                case esriUnits.esriYards:
                    sMapUnits = "Yards";
                    break;
            }
            // 当主地图显示控件的地图更换时，鹰眼中的地图也跟随更换 
            this.axMapControl2.Map = new MapClass();
            // 添加主地图控件中的所有图层到鹰眼控件中 
            for (int i = 1; i <= this.axMapControl1.LayerCount; i++)
            {
                this.axMapControl2.AddLayer(this.axMapControl1.get_Layer(this.axMapControl1.LayerCount - i));
            }
            // 设置 MapControl 显示范围至数据的全局范围 
            this.axMapControl2.Extent = this.axMapControl1.FullExtent;   
            // 刷新鹰眼控件地图 
            this.axMapControl2.Refresh();
        }
        //加载数据没有新建文档时使用
        private void axMapControl1_OnAfterScreenDraw(object sender, IMapControlEvents2_OnAfterScreenDrawEvent e)
        {
            //// 当主地图显示控件的地图更换时，鹰眼中的地图也跟随更换 
            //this.axMapControl2.Map = new MapClass();
            //// 添加主地图控件中的所有图层到鹰眼控件中 
            //for (int i = 1; i <= this.axMapControl1.LayerCount; i++)
            //{
            //    this.axMapControl2.AddLayer(this.axMapControl1.get_Layer(this.axMapControl1.LayerCount - i));
            //}
            //// 设置 MapControl 显示范围至数据的全局范围 
            //this.axMapControl2.Extent = this.axMapControl1.FullExtent;
            //// 刷新鹰眼控件地图 
            //this.axMapControl2.Refresh();
        }

        private void axMapControl1_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            // 得到新范围 
            IEnvelope pEnv = (IEnvelope)e.newEnvelope;
            IGraphicsContainer pGra = axMapControl2.Map as IGraphicsContainer;
            IActiveView pAv = pGra as IActiveView;
            // 在绘制前，清除 axMapControl2 中的任何图形元素 
            pGra.DeleteAllElements();
            IRectangleElement pRectangleEle = new RectangleElementClass(); 
            IElement pEle = pRectangleEle as IElement;
            pEle.Geometry = pEnv;
            // 设置鹰眼图中的红线框 
            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0; 
            pColor.Blue = 0;
            pColor.Transparency = 255;
            // 产生一个线符号对象 
            ILineSymbol pOutline = new SimpleLineSymbolClass();
            pOutline.Width = 2;
            pOutline.Color = pColor;       
            // 设置颜色属性 
            pColor = new RgbColorClass(); 
            pColor.Red = 255;
            pColor.Green = 0; 
            pColor.Blue = 0;
            pColor.Transparency = 0;
            // 设置填充符号的属性 
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Color = pColor;
            pFillSymbol.Outline = pOutline;
            IFillShapeElement pFillShapeEle = pEle as IFillShapeElement;
            pFillShapeEle.Symbol = pFillSymbol;
            pGra.AddElement((IElement)pFillShapeEle, 0);
            // 刷新 
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void axMapControl2_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (this.axMapControl2.Map.LayerCount != 0)
            {
                // 按下鼠标左键移动矩形框
                if (e.button == 1)
                {
                    IPoint pPoint = new PointClass(); 
                    pPoint.PutCoords(e.mapX, e.mapY);
                    IEnvelope pEnvelope = this.axMapControl1.Extent;
                    pEnvelope.CenterAt(pPoint); 
                    this.axMapControl1.Extent = pEnvelope;
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography,null, null);  // 按下鼠标右键绘制矩形框
                }
                    //右键绘制属性框
                else if (e.button == 2)
                {
                    IEnvelope pEnvelop = this.axMapControl2.TrackRectangle();
                    this.axMapControl1.Extent = pEnvelop; 
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography,
                    null, null);
                }
            }

        }

        private void axMapControl2_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            //如果不是左键按下就直接返回 
            if (e.button != 1) return;
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(e.mapX, e.mapY);
            this.axMapControl1.CenterAt(pPoint);
            this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);

        }

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            //若果不是鼠标右键按下就直接退出
            if (e.button != 2) return;
            esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap map = null;
            ILayer layer = null;
            object other = null;
            object index = null;


         
            //判断所选菜单的类型
            m_tocControl.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
          
               
            //确定选定的菜单类型，Map或是图层菜单
            if (item == esriTOCControlItem.esriTOCControlItemMap)
            {
                m_tocControl.SelectItem(map, null);
                m_menuMap.PopupMenu(e.x, e.y, m_tocControl.hWnd);
            }
            else
            {
                m_tocControl.SelectItem(layer, null);
                //添加 打开属性表 
                m_menuLayer.AddItem(new OpenAttributeTable(layer), -1, 2, true, esriCommandStyles.esriCommandStyleTextOnly);
                m_menuLayer.PopupMenu(e.x, e.y, m_tocControl.hWnd);
                //移除OpenAttributeTable菜单项，以防止重复添加
                m_menuLayer.Remove(2);
            }


        


         
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            axMapControl1.DeleteLayer(LayerIndex);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ArttiuteQueryFrm frm=new ArttiuteQueryFrm();
            frm.Show();
        }

      

       



    }
}
