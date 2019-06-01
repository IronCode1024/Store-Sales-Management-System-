using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NIKE_Project
{
    public partial class SalesReturn : Form
    {
        public SalesReturn()
        {
            InitializeComponent();
        }
        //引用DBHelper类
        DBHelper db = new DBHelper();

        //退货窗体加载事件
        private void SalesReturn_Load(object sender, EventArgs e)
        {
            //禁止点击dataGridView1列标题排序
            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                this.dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            this.label4.Text = "";
            this.label5.Text = "";
        }

        //创建DataGridView数据绑定函数
        public void BindGrid()
        {
            //获取用户输入的小票流水号
            string receiptsCode = this.textBox1.Text;
            if (receiptsCode == "")
            {
                MessageBox.Show("请输入小票流水号！");
            }
            else
            {
                //根据 “商品名称表”“销售记录表”“销售明细表”“员工信息表”查询退货界面商品信息
                string Sql = string.Format(@"select Se.SalesID,Sd.SDID,G.BarCode,G.GoodsName,Sd.AloneAmount,Sd.Quantity,Sn.SalesmanName,Se.SalesDate from Goods G,Salesan Sn,Sales Se,SalesDetail Sd 
                                       where  G.GoodsID=Sd.GoodsID and Sn.SalesmanID=Se.SalesmanID and Se.SalesID=Sd.SalesID and Se.ReceiptsCode='{0}'", receiptsCode);
                DataSet ds = db.getDataSet(Sql);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("抱歉，没有此记录！");
                }
                else
                {
                    this.dataGridView1.AutoGenerateColumns = false;
                    this.dataGridView1.DataSource = ds.Tables["nike"];

                    //获取根据“销售记录表”查询交易金额
                    string Amount = string.Format("select Amount from Sales where ReceiptsCode='{0}'", receiptsCode);
                    ds = db.getDataSet(Amount);
                    this.label4.Text = ds.Tables[0].Rows[0]["Amount"].ToString();
                }
            }
        }

        //查询按钮单击事件
        private void button1_Click(object sender, EventArgs e)
        {
            //调用DataGridView数据绑定函数
            BindGrid();
        }
        //单击单元格任意部分时发生
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            double sumtotalMoney = 0;
            //计算退货金额
            for (int i = 0; i < this.dataGridView1.SelectedRows.Count; i++)
            {
                sumtotalMoney += (Convert.ToDouble(this.dataGridView1.SelectedRows[i].Cells[2].Value) * Convert.ToDouble(this.dataGridView1.SelectedRows[i].Cells[3].Value));
            }
            this.label5.Text = sumtotalMoney.ToString("0.00");
        }
        //退货单击事件
        private void button2_Click(object sender, EventArgs e)
        {
            //获取交易金额
            float totalMoney = float.Parse(this.label4.Text);
            //获取退货金额
            float returnMoney = float.Parse(this.label5.Text);
            if (returnMoney == 0.00)
            {
                MessageBox.Show("请选择需要退货的商品！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string Sqlstr = "";
                //获取SalesID
                string SalesID = this.dataGridView1.SelectedRows[0].Cells["Column7"].Value.ToString();
                //判断是“全退”还是“部分退 ”
                if (returnMoney == totalMoney)
                {
                    //如果是全退，则删除销售记录和销售明细
                    Sqlstr = string.Format("delete SalesDetail where SalesID={0} delete Sales where SalesID={0}", SalesID);
                    //执行Sql
                    int rows = db.zsg(Sqlstr);
                    if (rows > 0)
                    {
                        MessageBox.Show("退货成功！");
                        string Sql = string.Format(@"select Se.SalesID,Sd.SDID,G.BarCode,G.GoodsName,Sd.AloneAmount,Sd.Quantity,Sn.SalesmanName,Se.SalesDate from Goods G,Salesan Sn,Sales Se,SalesDetail Sd 
                                       where  G.GoodsID=Sd.GoodsID and Sn.SalesmanID=Se.SalesmanID and Se.SalesID=Sd.SalesID and Se.ReceiptsCode='{0}'", this.textBox1.Text);
                        DataSet ds = db.getDataSet(Sql);
                        this.dataGridView1.AutoGenerateColumns = false;
                        this.dataGridView1.DataSource = ds.Tables["nike"];
                    }
                    else
                    {
                        MessageBox.Show("退货失败，请重试！");
                    }
                }
                else
                {
                    //如果是部分退，则删除退货商品的销售明细、修改销售记录中的交易金额
                    foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
                    {
                        //获取当前销售明细ID
                        string SDID = row.Cells["Column8"].Value.ToString();
                        Sqlstr += "delete SalesDetail where SDID=" + SDID + "";
                    }
                    Sqlstr += string.Format("update Sales set Amount=Amount-{0} where SalesID={1}", returnMoney, SalesID);
                    //执行Sql
                    int rows = db.zsg(Sqlstr);
                    if (rows > 0)
                    {
                        MessageBox.Show("退货成功！");
                        BindGrid();
                    }
                    else
                    {
                        MessageBox.Show("退货失败，请重试！");
                    }
                }
            }
            
        }
        //取消单击事件
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
