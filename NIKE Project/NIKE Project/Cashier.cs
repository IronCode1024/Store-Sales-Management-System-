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
    public partial class Cashier : Form
    {
        public Cashier()
        {
            InitializeComponent();
        }
        //调用DBHelper类
        DBHelper db = new DBHelper();

        //定义DataSet全局变量
        public DataSet ds;

        //定义流水号全局变量
        public string runningWater;

        //创建生成流水号函数
        public void serial_Number()
        {
            string time = DateTime.Now.ToString("yyyyMMddhhmmss");//获取当前日期
            //this.label3.Text = "流水号：" + time;
            Random rd = new Random();
            string L = rd.Next(10, 100).ToString();//随机生成一个两位数的整数
            runningWater = time + L;
            this.label3.Text = "流水号：" + (time + L);//生成流水号，格式：时间+两位数随机数=16位数
            //string uuid = Guid.NewGuid().ToString();

        }

        //创建计算总金额购买商品总数量函数
        public void cashNum()
        {
            double sumMoney = 0;//定义购买总金额变量
            double sum = 0;//定义购买总数量变量
            //显示“折后价”列的数据
            for (int i = 0; i < dataGridView1.Rows.Count; i++)//遍历DataGridView中的所有行
            {
                //计算购买总金额   各商品折后价与购买数量之积的和
                sumMoney += (Convert.ToDouble(this.dataGridView1.Rows[i].Cells[5].Value) * Convert.ToDouble(this.dataGridView1.Rows[i].Cells[7].Value));


                //计算购买总数量
                sum += Convert.ToDouble(dataGridView1.Rows[i].Cells[7].Value.ToString());
            }
            this.textBox2.Text = sumMoney.ToString("F2");//将各商品折后价与购买数量之积的和的值赋给“应收”文本框

            this.label4.Text = "共：￥" + this.textBox2.Text;//将“应收”文本框的值赋给共￥多少金额

            this.label5.Text = "商品数量：" + sum.ToString();//将计算的购买总数量的值赋给label5显示出来
        }

        //收银台窗体加载事件
        private void Cashier_Load(object sender, EventArgs e)
        {
            //禁止点击dataGridView1列标题排序
            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                this.dataGridView1.Columns[i].SortMode=DataGridViewColumnSortMode.NotSortable;
            }

            //调用流水号函数
            serial_Number();

            //导购员下拉框
            string Sql = string.Format("select SalesmanName from Salesan where Role='导购员'");
            ds=db.getDataSet(Sql);
            DataRow dr = ds.Tables[0].NewRow();
            dr[0] = "--请选择--";
            ds.Tables["nike"].Rows.InsertAt(dr, 0);
            this.comboBox1.DataSource=ds.Tables["nike"];
            this.comboBox1.DisplayMember = "SalesmanName";



            //显示收银员姓名
            label9.Text = (LoginInfo.RoleName+"："+LoginInfo.UaerName);
        }


        //重置按钮单击事件
        private void button2_Click(object sender, EventArgs e)
        {
            //点击重置按钮将光标定位到货号/条形码输入框TextBox1
            this.textBox1.Focus();
            //调用流水号函数
            serial_Number();

            //清空textBox文本框中的数据
            this.textBox1.Text = null;
            this.textBox2.Text = null;
            this.textBox3.Text = null;
            this.textBox4.Text = null;
            //清空共计金额
            this.label4.Text = "共：";
            //清空商品数量
            this.label5.Text = "商品数量：";

            //清空DataGridView里的数据
            //for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            //{
            //    for (int j = 0; j < this.dataGridView1.ColumnCount; j++)
            //    {
            //        this.dataGridView1.Rows[i].Cells[j].Value = null;
            //    }
            //}
            this.dataGridView1.Rows.Clear();

            //重置结算按钮
            this.button1.Text = "结算";
            this.button1.Font = new Font("宋体", 20, button1.Font.Style);
            this.button1.Enabled = true;
        }

        //当用户按下某个键时引发的事件  textBox1输入货号显示数据
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            string Bc = this.textBox1.Text;
            if (e.KeyChar == 13)//绑定指定键盘键 回车键(ASCII值为13)
            {
                //判断“货号/条形码”文本框是否有值
                if (Bc != "")
                {
                    //查询DataGridView 1 数据
                    string Sqlstr = string.Format(@"select GoodsID,BarCode,GoodsName,t.TypeName,SalePrice,Discount 
                       from Goods g,Type t where g.TypeID=t.TypeID and BarCode='{0}'", Bc);
                    ds = db.getDataSet(Sqlstr);
                    //this.dataGridView1.AutoGenerateColumns = false;
                    //this.dataGridView1.DataSource = ds.Tables["nike"];
                    //DataTable dt=new DataTable();
                    if (ds.Tables[0].Rows.Count == 0)//判断输入的“货号/条形码”是否能查到数据
                    {
                        MessageBox.Show("请输入正确的货号/条形码");
                    }
                    else
                    {
                        //判断DataGridView中是否有数据
                        if (this.dataGridView1.Rows.Count == 0)//DataGridView中没有数据，把查询到的数据新建一行显示出来
                        {
                            //查一行显示一行
                            int index = this.dataGridView1.Rows.Add();//在DataGridView中新建一行
                            this.dataGridView1.Rows[index].Cells[0].Value = ds.Tables[0].Rows[0]["BarCode"].ToString();
                            this.dataGridView1.Rows[index].Cells[1].Value = ds.Tables[0].Rows[0]["GoodsName"].ToString();
                            this.dataGridView1.Rows[index].Cells[2].Value = ds.Tables[0].Rows[0]["TypeName"].ToString();
                            this.dataGridView1.Rows[index].Cells[3].Value = ds.Tables[0].Rows[0]["SalePrice"].ToString();
                            this.dataGridView1.Rows[index].Cells[4].Value = ds.Tables[0].Rows[0]["Discount"].ToString();

                            string goods = ds.Tables[0].Rows[0]["GoodsID"].ToString();

                            this.dataGridView1.Rows[index].Cells[7].Value = "1";//显示“购买数量”默认初始值
                            
                        }
                        else if (this.dataGridView1.Rows.Count != 0)
                        {
                            for (int j = 0; j < this.dataGridView1.Rows.Count; j++)
                            {
                            string wu = "";//定义变量获取遍历行的第一列的值
                            string uw = "";//定义变量获取查询到的BarCode货号/流水号的值
                            //遍历DataGridView所有行第一列数据
                            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                            {
                                wu = this.dataGridView1.Rows[i].Cells[0].Value.ToString();//获取遍历行的第一列的值
                                uw = ds.Tables[0].Rows[0]["BarCode"].ToString();//获取查询到的BarCode货号/流水号的值
                                //判断输入的“货号/流水号”是否等于DataGridView中BarCode现有数据 
                                if (wu == uw)//如果已有此商品则跳出循环
                                {
                                    break;
                                }
                            }
                                //判断购物车中是否有此商品
                                if (this.dataGridView1.Rows[j].Cells[0].Value.ToString() == ds.Tables[0].Rows[0]["BarCode"].ToString())//有此商品，购买数量加一
                                {
                                    //购买数量加一
                                    this.dataGridView1.Rows[j].Cells[7].Value = int.Parse(this.dataGridView1.Rows[j].Cells[7].Value.ToString()) + 1;
                                }
                                else if (wu != ds.Tables[0].Rows[0]["BarCode"].ToString())//没有此商品，新加入一行商品信息
                                {
                                    //查一行显示一行
                                    int index = this.dataGridView1.Rows.Add();//在DataGridView中新建一行
                                    this.dataGridView1.Rows[index].Cells[0].Value = ds.Tables[0].Rows[0]["BarCode"].ToString();
                                    this.dataGridView1.Rows[index].Cells[1].Value = ds.Tables[0].Rows[0]["GoodsName"].ToString();
                                    this.dataGridView1.Rows[index].Cells[2].Value = ds.Tables[0].Rows[0]["TypeName"].ToString();
                                    this.dataGridView1.Rows[index].Cells[3].Value = ds.Tables[0].Rows[0]["SalePrice"].ToString();
                                    this.dataGridView1.Rows[index].Cells[4].Value = ds.Tables[0].Rows[0]["Discount"].ToString();

                                    string goods = ds.Tables[0].Rows[0]["GoodsID"].ToString();

                                    this.dataGridView1.Rows[index].Cells[7].Value = "0";//显示“购买数量”默认初始值
                                }
                            }

                        }
                        //显示“折后价”列的数据
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)//遍历DataGridView中的所有行
                        {
                            if ((bool)(dataGridView1.Rows[i].Cells[5].Value = true))//判断要赋值  第几列的数据
                            {
                                //计算并赋值“折后价” 列的数据 折后价=零售价*折扣
                                dataGridView1.Rows[i].Cells[5].Value = (Convert.ToDecimal(this.dataGridView1.Rows[i].Cells[3].Value) * Convert.ToDecimal(this.dataGridView1.Rows[i].Cells[4].Value)).ToString("0.00###");//"0.00###"保留到最后一位不为0的小数
                            }
                        }

                        //调用计算总金额和购买总数量函数
                        cashNum();
                    }     
                }
                else
                {
                    MessageBox.Show("货号不能为空！");

                }
            }
        }

        //结算单击事件
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //判断条件是否满足结算条件
                if (this.comboBox1.Text == "")
                {
                    MessageBox.Show("请选择导购员再结算！");
                }
                else if (textBox2.Text == "" || textBox3.Text == "")
                {
                    MessageBox.Show("请完成“应收”“实收”数据再结算！");
                }
                else
                {
                    Double receivable = Double.Parse(textBox2.Text);//应收
                    Double official = Double.Parse(textBox3.Text);//实收

                    //判实付金额是否足够支付应收金额
                    if (receivable > official)
                    {
                        MessageBox.Show("实付金额不足，请重新支付！");
                    }
                    else
                    {
                        //----//
                        //添加销售记录
                        //----//
                        string Amount = this.textBox2.Text;//获取销售金额
                        //根据下拉框中的导购员查询此导购员的ID
                        string Salesman = string.Format("select SalesmanID from Salesan where SalesmanName='{0}'", this.comboBox1.Text);
                        ds = db.getDataSet(Salesman);
                        string SalesmanID = ds.Tables[0].Rows[0]["SalesmanID"].ToString();//获取经手的导购员ID

                        ////根据窗体下方显示的收银员姓名查询此收银员的ID  label9
                        string Cashier = string.Format("select SalesmanID from Salesan where SalesmanName='{0}'", LoginInfo.UaerName);
                        ds = db.getDataSet(Cashier);
                        string CashierID = ds.Tables[0].Rows[0]["SalesmanID"].ToString();//获取经手的收营员ID

                        //向 销售记录表 添加销售记录数据
                        String Sales = string.Format(@"insert into Sales(ReceiptsCode,Amount,SalesmanID,CashierID) Values ('{0}','{1}','{2}','{3}')",
                                              runningWater, Amount, SalesmanID, CashierID);
                        int rows1 = db.zsg(Sales);

                        //----//
                        //添加销售明细记录
                        //----//
                        //获取销售记录ID
                        string ID = string.Format("select SalesID from Sales where ReceiptsCode='{0}'", runningWater);
                        ds = db.getDataSet(ID);
                        string SalesID = ds.Tables[0].Rows[0]["SalesID"].ToString();
                        //遍历购物车 获取每件商品的信息
                        int rows2 = 0;
                        for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                        {
                            //获取商品ID
                            string barCode = this.dataGridView1.Rows[i].Cells[0].Value.ToString();
                            string Goods = string.Format("select GoodsID from Goods where BarCode='{0}'", barCode);
                            ds = db.getDataSet(Goods);
                            string GoodsID = ds.Tables[0].Rows[0]["GoodsID"].ToString();
                            //获取购买的商品数量
                            string Quantity = this.dataGridView1.Rows[i].Cells[7].Value.ToString();
                            //获取商品成交价格
                            string AloneAmount = this.dataGridView1.Rows[i].Cells[5].Value.ToString();
                            //编写SQL语句向销售明细表添加数据  根据购买数量更新相应库存数量
                            string SalesDetail = string.Format(@"insert into SalesDetail(SalesID,GoodsID,Quantity,AloneAmount) values
					                                      ('{0}','{1}','{2}','{3}') update Goods set StockNum=StockNum-{2} where GoodsID={1}",
                                                           SalesID, GoodsID, Quantity, AloneAmount);
                            rows2 = db.zsg(SalesDetail);

                        }
                        //判断销售记录和销售明细数据是否添加成功 成功就提示结算成功并计算找零金额
                        if (rows1 > 0 && rows2 > 0)
                        {
                            //MessageBox.Show("结算成功！");
                            //结算成功将光标定位到重置按钮
                            this.button2.Focus();
                            //结算按钮控制
                            this.button1.Text = "结算成功";
                            this.button1.Font = new Font("宋体", 13, button1.Font.Style);
                            this.button1.ForeColor = Color.Red;
                            this.button1.Enabled = false;

                            Double change;//找零
                            change = official - receivable;//计算找零
                            textBox4.Text = change.ToString("F2");//把Double类型转换成String类型， 赋值给找零文本框

                            //string Goods = string.Format("select * from Goods select * from Sales select * from SalesDetail");
                        }
                        else
                        {
                            MessageBox.Show("结算失败！");
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //DataGridView单击-+单元格内容事件
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //-
            if (e.ColumnIndex == 6)//判断点击的单元格在第几列
            {
                int a = 0;//定义变量计算数据
                //先把单元格中的数据赋值给变量
                a = int.Parse(this.dataGridView1.SelectedRows[0].Cells[7].Value.ToString());
                a -= 1;//计算每次点击a减一
                if (a > 0)//“购买数量”数据禁止小于0
                {
                    //把点击之后并计算减一的数据赋值给单元格
                    this.dataGridView1.SelectedRows[0].Cells[7].Value = a;

                    //调用计算总金额和购买总数量函数
                    cashNum();
                    //textBox2.Text = (Convert.ToDecimal(this.dataGridView1.SelectedRows[0].Cells[5].Value) * Convert.ToDecimal(this.dataGridView1.SelectedRows[0].Cells[7].Value)).ToString("0.00###");
                    //this.label4.Text = "共：￥" + this.textBox2.Text;
                }

            }
            //+
            if (e.ColumnIndex == 8)//判断点击的单元格在第几列
            {
                int b = 0;//定义变量计算数据
                //先把单元格中的数据赋值给变量
                b = int.Parse(this.dataGridView1.SelectedRows[0].Cells[7].Value.ToString());
                b += 1;//计算每次点击a加一
                //把点击之后并计算减一的数据赋值给单元格
                this.dataGridView1.SelectedRows[0].Cells[7].Value = b;

                //调用计算总金额和购买总数量函数
                cashNum();
            } 
        }

        //绑定+-键盘键
        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //  -
            if (e.KeyChar == 45)
            {
                if ((bool)(dataGridView1.Rows[0].Cells[6].Value = true))//判断点击的单元格在第几列
                {
                    int a = 0;//定义变量计算数据
                    //先把单元格中的数据赋值给变量
                    a = int.Parse(this.dataGridView1.SelectedRows[0].Cells[7].Value.ToString());
                    a -= 1;//计算每次点击a减一
                    if (a > 0)//“购买数量”数据禁止小于0
                    {
                        //把点击之后并计算减一的数据赋值给单元格
                        this.dataGridView1.SelectedRows[0].Cells[7].Value = a;

                        //调用计算总金额和购买总数量函数
                        cashNum();
                    }

                }
            }
            //  +
            if (e.KeyChar==43)
            {
                if ((bool)(dataGridView1.Rows[0].Cells[8].Value = true))//判断点击的单元格在第几列
                {
                    int b = 0;//定义变量计算数据
                    //先把单元格中的数据赋值给变量
                    b = int.Parse(this.dataGridView1.SelectedRows[0].Cells[7].Value.ToString());
                    b += 1;//计算每次点击a加一
                    //把点击之后并计算减一的数据赋值给单元格
                    this.dataGridView1.SelectedRows[0].Cells[7].Value = b;

                    //调用计算总金额和购买总数量函数
                    cashNum();
                }   
            }
        }

        //右键菜单单击事件
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);//删除选中行
            //显示“折后价”列的数据
            for (int i = 0; i < dataGridView1.Rows.Count; i++)//遍历DataGridView中的所有行
            {
                if ((bool)(dataGridView1.Rows[i].Cells[5].Value = true))//判断要赋值  第几列的数据
                {
                    //计算并赋值“折后价” 列的数据 折后价=零售价*折扣
                    dataGridView1.Rows[i].Cells[5].Value = (Convert.ToDecimal(this.dataGridView1.Rows[i].Cells[3].Value) * Convert.ToDecimal(this.dataGridView1.Rows[i].Cells[4].Value)).ToString("0.00###");//"0.00###"保留到最后一位不为0的小数

                }
            }
            //调用计算总金额和购买总数量函数
            cashNum();
        }

      

    }
}
