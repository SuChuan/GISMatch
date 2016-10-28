using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EngineStudy.StatisQuery
{
    public partial class ArttiuteQueryFrm : Form
    {
        public ArttiuteQueryFrm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ArttiuteQueryFrm_Load(object sender, EventArgs e)
        {
            this.comboBox1.Items.Clear();
            if (MatchMain.m_mapControl.LayerCount <= 0)
            {
                MessageBox.Show("请先加载图层");
                return;
            }
            for (int i = 0; i <MatchMain.m_mapControl.LayerCount ; i++)
            {
                this.comboBox1.Items.Add(MatchMain.m_mapControl.Layer[i].Name);
            }

            this.comboBox1.SelectedIndex = 0;
        }

      
    }
}
