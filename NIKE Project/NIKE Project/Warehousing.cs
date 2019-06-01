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
    public partial class Warehousing : Form
    {
        public Warehousing()
        {
            InitializeComponent();
        }

        //调用DBHelper类
        DBHelper db = new DBHelper();

        //商品入库窗体加载事件
        private void Warehousing_Load(object sender, EventArgs e)
        {
            //隐藏折扣填写错误提示
            this.label9.Visible = false;
            //this.comboBox1.Text = "--请选择--";
            this.comboBox2.Text = "--请选择--";
            string Sql = string.Format("select TypeName,ParentID from Type where TypeName in('鞋类','服装','户外装备')");
            DataSet ds =db.getDataSet(Sql);
            DataRow dr = ds.Tables[0].NewRow();
            dr[0] = "--请选择--";
            ds.Tables["nike"].Rows.InsertAt(dr, 0);
            //显示comboBox1下拉框数据
            this.comboBox1.DisplayMember = "TypeName";
            this.comboBox1.DataSource = ds.Tables["nike"];
          
        }
        //下拉框Text值改变时发生，当选择comboBox1下拉框值时改变comboBox2下拉框的值
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            string xc = this.comboBox1.Text;
            //根据一级分类名称获取一级分类TypeID  SQL语句
            string yj = string.Format("select TypeID from Type where TypeName='{0}' ", xc);
            DataSet dsyj = db.getDataSet(yj);
            //判断comboBox1的值查出来是否有TypeIDID数据
            if (dsyj.Tables["nike"].Rows.Count!=0)
            {
                //根据一级分类名称获取一级分类TypeID
                string yjTypeID = dsyj.Tables["nike"].Rows[0]["TypeID"].ToString();
                //根据一级分类TypeID查询二级分类名称
                string Sql1 = string.Format("select t2.TypeName,t2.ParentID from Type t1,Type t2 where t1.TypeID=t2.ParentID and t2.ParentID in('{0}');", yjTypeID);
                DataSet ds1 = db.getDataSet(Sql1);
                DataRow dr = ds1.Tables[0].NewRow();
                dr[0] = "--请选择--";
                ds1.Tables["nike"].Rows.InsertAt(dr, 0);
                this.comboBox2.DataSource = ds1.Tables["nike"];
                this.comboBox2.DisplayMember = "TypeName";
            }
            else if (this.comboBox1.Text == "--请选择--")
            {
                //二级分类下拉框
                string Sqldt12 = string.Format("select t2.TypeName,t2.ParentID from Type t1,Type t2 where t1.TypeID=t2.TypeID and t2.ParentID in('1','2','3');");
                DataSet ds = db.getDataSet(Sqldt12);
                DataRow dr2 = ds.Tables[0].NewRow();
                dr2[0] = "--请选择--";
                ds.Tables["nike"].Rows.InsertAt(dr2, 0);
                this.comboBox2.DataSource = ds.Tables["nike"];
                this.comboBox2.DisplayMember = "TypeName";
            }
            else
            {
                return;
            }
        }

        //下拉框选项更改
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (this.comboBox1.Text == "")
            //{
            //    string xc = this.comboBox1.Text;
            //    string yj = string.Format("select TypeID from Type where TypeName='{0}' ", xc);
            //    DataSet dsyj = db.getDataSet(yj);
            //    string yjTypeID = dsyj.Tables["nike"].Rows[0]["TypeID"].ToString();


            //    string Sql1 = string.Format("select t2.TypeName,t2.ParentID from Type t1,Type t2 where t1.TypeID=t2.ParentID and t2.ParentID in('{0}');", yjTypeID);
            //    DataSet ds1 = db.getDataSet(Sql1);
            //    this.comboBox2.DataSource = ds1.Tables["nike"];
            //    this.comboBox2.DisplayMember = "TypeName";
            //}
            //if (xc == "鞋类")
            //{
                //显示comboBox2下拉框数据 

            //}
            //else if (xc == "服装")
            //{
            //    //显示comboBox2下拉框数据
            //    string Sql1 = string.Format("select t2.TypeName,t2.ParentID from Type t1,Type t2 where t1.TypeID=t2.ParentID and t2.ParentID in('2');");
            //    DataSet ds1 = db.getDataSet(Sql1);
            //    this.comboBox2.DataSource = ds1.Tables["nike"];
            //    this.comboBox2.DisplayMember = "TypeName";

            //}
            //else if (xc == "户外装备")
            //{
            //    //显示comboBox2下拉框数据
            //    string Sql1 = string.Format("select t2.TypeName,t2.ParentID from Type t1,Type t2 where t1.TypeID=t2.ParentID and t2.ParentID in('3');");
            //    DataSet ds1 = db.getDataSet(Sql1);
            //    this.comboBox2.DataSource = ds1.Tables["nike"];
            //    this.comboBox2.DisplayMember = "TypeName";
            //}
            //else if (xc == "")
            //{
            //    this.comboBox1.Text = this.comboBox2.Text;
            //}

        }

        //读取信息单击事件
        private void button1_Click(object sender, EventArgs e)
        {
            //隐藏折扣填写错误提示
            this.label9.Visible = false;
            //货号/条形码
            string Dq = this.textBox1.Text;
            if (Dq != "")
            {
                string SQl = string.Format("select * from Goods,Type where Goods.TypeID=Type.TypeID and BarCode='{0}'", Dq);
                DataSet ds = db.getDataSet(SQl);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("抱歉，没有此商品的库存！");
                }
                else
                {
                    //商品名称
                    this.textBox2.Text = ds.Tables["nike"].Rows[0]["GoodsName"].ToString();

                    //一级分类下拉框
                    //获取一级分类TypeID
                    string TypeName1 = ds.Tables["nike"].Rows[0]["TypeID"].ToString();
                    //根据一级分类TypeID名称查询ParentID
                    string Parentid = string.Format("select ParentID from Type where TypeID='{0}' ", TypeName1);
                    DataSet dsId = db.getDataSet(Parentid);
                    string Tpid = dsId.Tables["nike"].Rows[0]["ParentID"].ToString();
                    //根据一级分类查到的ParentID通过TypeID查询一级分类名称
                    string Name = string.Format("select TypeName from Type where TypeID='{0}' ", Tpid);
                    DataSet dsName = db.getDataSet(Name);
                    //把查询到的名称赋值给下拉框
                    this.comboBox1.Text = dsName.Tables["nike"].Rows[0]["TypeName"].ToString();


                    //二级分类下拉框
                    this.comboBox2.Text = ds.Tables["nike"].Rows[0]["TypeName"].ToString();
                    string Sql1 = string.Format("select TypeName from Type,Goods where Type.TypeID=Goods.TypeID and BarCode='{0}';", Dq);
                    DataSet ds1 = db.getDataSet(Sql1);
                    this.comboBox2.DataSource = ds1.Tables["nike"];
                    this.comboBox2.DisplayMember = "TypeName";

                    //进货价格
                    this.textBox3.Text = ds.Tables["nike"].Rows[0]["StorePrice"].ToString();
                    //零售价格
                    this.textBox4.Text = ds.Tables["nike"].Rows[0]["SalePrice"].ToString();
                    //折扣
                    this.textBox5.Text = ds.Tables["nike"].Rows[0]["Discount"].ToString();
                    //库存数量
                    this.textBox6.Text = ds.Tables["nike"].Rows[0]["StockNum"].ToString();    
                }
            }
            else
            {
                MessageBox.Show("请输入货号/条形码");
            }

        }

        //入库单击事件
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "" || this.textBox2.Text == "" || this.comboBox1.Text == "--请选择--" || this.comboBox2.Text == "--请选择--" || this.textBox3.Text == "" || this.textBox4.Text == "" || this.textBox5.Text == "" || this.textBox6.Text == "")
            {
                MessageBox.Show("请填写完整的入库信息！");
                return;
            }
            //验证输入的文本是否是单精度数字
            float result;
            if (!float.TryParse(this.textBox5.Text, out result))
            {
                //显示折扣填写错误提示
                this.label9.Show();
                //this.label9.Visible = true;
                this.label9.Text = "折扣必须是数字！";
                return;
            }
            else if (result < 0 || result > 1)
            {
                //显示折扣填写错误提示
                this.label9.Visible = true;
                this.label9.Text = "折扣必须是0～1的数字！";
                return;
            }

            string Sqlb = string.Format("select * from Type,Goods where Type.TypeID=Goods.TypeID and BarCode='{0}';", this.textBox1.Text);
            DataSet dsb = db.getDataSet(Sqlb);

            string Type = this.comboBox2.Text;
            string Typeid = string.Format("select TypeID from Type where TypeName='{0}' ", Type);
            DataSet dsId = db.getDataSet(Typeid);
            string Tpid = dsId.Tables["nike"].Rows[0]["TypeID"].ToString();
            //判断是商品库中是否有此商品  如果有就更新商品库存   如果没有就添加商品
            if (dsb.Tables["nike"].Rows.Count == 0)
            {
                //添加商品SQL语句
                string Sql = string.Format(@"insert into Goods(BarCode,TypeID,GoodsName,StorePrice,SalePrice,Discount,StockNum) values
				 ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", this.textBox1.Text, Tpid, this.textBox2.Text, this.textBox3.Text, this.textBox4.Text, this.textBox5.Text, this.textBox6.Text);
                int rows = db.zsg(Sql);
                if (rows > 0)
                {
                    //隐藏折扣填写错误提示
                    this.label9.Visible = false;
                    MessageBox.Show("入库成功");
                    //清空文本框内容
                    this.textBox1.Text = "";
                    this.textBox2.Text = "";
                    this.comboBox1.Text = "--请选择--";
                    this.comboBox2.Text = "--请选择--";
                    this.textBox3.Text = "";
                     //零售价格
                     this.textBox4.Text = "";
                     //折扣
                     this.textBox5.Text = "";
                     //库存数量
                     this.textBox6.Text = "";

                }
                else
                {
                    MessageBox.Show("入库失败！");
                }
            }
            else
            {
                //更新库存SQL语句
                string SqlStr = string.Format(@"update Goods set TypeID='{0}',GoodsName='{1}',StorePrice='{2}',SalePrice='{3}',Discount='{4}',StockNum=StockNum+{5} 
                                              where BarCode='{6}'", Tpid, this.textBox2.Text, this.textBox3.Text, this.textBox4.Text, this.textBox5.Text, this.textBox6.Text, this.textBox1.Text);
                int Rows = db.zsg(SqlStr);
                if (Rows > 0)
                {
                    //隐藏折扣填写错误提示
                    this.label9.Visible = false;
                    MessageBox.Show("更新成功！");
                    //清空文本框内容
                    this.textBox1.Text = "";
                    this.textBox2.Text = "";
                    this.comboBox1.Text = "--请选择--";
                    this.comboBox2.Text = "--请选择--";
                    this.textBox3.Text = "";
                    //零售价格
                    this.textBox4.Text = "";
                    //折扣
                    this.textBox5.Text = "";
                    //库存数量
                    this.textBox6.Text = "";
                }
                else
                {
                    MessageBox.Show("更新失败！");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }



    }
}
