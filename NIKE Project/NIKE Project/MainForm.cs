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
    public partial class MainForm : Form
    {
        //定义IrisSkin皮肤引擎对象
        Sunisoft.IrisSkin.SkinEngine skinEngine = null;


        public MainForm()
        {
            InitializeComponent();
        }

        //调用DBHelper类
        DBHelper db = new DBHelper();

        //定义DataSet全局变量
        public DataSet ds;

        //创建选择日期区间，自动确定起始日期和结束日期函数
        public void timeSecyion()
        { }

        //创建销售统计显示“销售金额”“单笔利润”函数
        public string dataGridView2Show(string SqlStr)
        {
            DataSet ds2 = db.getDataSet(SqlStr);
            //绑定DataGridView2，商品浏览
            this.dataGridView2.AutoGenerateColumns = false;
            this.dataGridView2.DataSource = ds2.Tables["nike"];
            //遍历DataGridView2 
            for (int i = 0; i < this.dataGridView2.Rows.Count; i++)
            {
                //根据 货号/条形码查询销售记录SalesID
                string BarCode = string.Format("select SalesID from Sales where ReceiptsCode='{0}'", this.dataGridView2.Rows[i].Cells[0].Value.ToString());
                DataSet ds = db.getDataSet(BarCode);
                //获取SalesID
                string SalesID = ds.Tables[0].Rows[0]["SalesID"].ToString();
                //编写计算单笔利润SQL语句  单笔利润 =（折后价-进货价）* 销售数量
                //根据SalesID获取单笔利润的和
                string Sales = string.Format(@"select sum((SalesDetail.AloneAmount-Goods.StorePrice)*SalesDetail.Quantity) 
                                             from SalesDetail,Goods 
                                             where SalesDetail.GoodsID=Goods.GoodsID and SalesID='{0}'", SalesID);
                DataSet ds1 = db.getDataSet(Sales);
                //为单笔利润列赋值
                this.dataGridView2.Rows[i].Cells[3].Value = ds1.Tables[0].Rows[0][0].ToString();//计算的单笔利润  获取第零行第零列sum
            }
            //显示销售记录的条数
            this.label34.Text = "销售记录" + ds2.Tables["nike"].Rows.Count + "条";
            //显示销售金额 和利润
            Double salesAmount = 0;
            Double salesProfit = 0;
            //遍历DataGridView2 计算销售金额和利润
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                if (this.dataGridView2.Rows[i].Cells[3].Value.ToString() != "")
                {
                    //计算销售总金额
                    salesAmount += Convert.ToDouble(this.dataGridView2.Rows[i].Cells[2].Value.ToString());
                    //计算利润
                    salesProfit += Convert.ToDouble(this.dataGridView2.Rows[i].Cells[3].Value.ToString());
                }
            }
            //显示销售金额和利润
            this.label35.Text = "销售金额￥" + salesAmount.ToString("0.00") + "元" + "，" + "利润￥" + salesProfit.ToString("F2") + "元";
            return SqlStr;
        }

        //下拉框切换皮肤
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //将皮肤下拉框改变值存到数据库
            string sqlStr = string.Format(@"update BasicConfiguration set DoorShopName='{0}',Skin='{1}',PicturePath='{2}',Assess='{3}' 
                                          where ConfigurationID='1'", Settings.ShopName, this.comboBox1.Text, Settings.AdImagePath, Settings.BaseSaleroom);
            int Rows = db.zsg(sqlStr);
            //Settings.SkinName = this.comboBox1.Text;
            //通过下拉框值显示相应的皮肤
            skinEngine.SkinFile = "ssk皮肤/" + this.comboBox1.Text + ".ssk";
        }

        //主窗体加载事件
        private void MainForm_Load(object sender, EventArgs e)
        {
            //创建IrisSkin皮肤引擎对象
            skinEngine = new Sunisoft.IrisSkin.SkinEngine();
            //设置项目中所有窗体均应用该皮肤
            skinEngine.SkinAllForm = true;
            skinEngine.SkinDialogs = true;

            //加载基本配置内容，并赋值给Settings类中相应的变量临时保存，以方便调用
            string Base = string.Format("select * from BasicConfiguration");
            DataSet Bs = db.getDataSet(Base);
            Settings.ShopName = Bs.Tables["nike"].Rows[0]["DoorShopName"].ToString();
            Settings.SkinName = Bs.Tables["nike"].Rows[0]["Skin"].ToString();
            Settings.AdImagePath = Bs.Tables["nike"].Rows[0]["PicturePath"].ToString();
            Settings.BaseSaleroom = Convert.ToDouble(Bs.Tables["nike"].Rows[0]["Assess"].ToString());

            //加载皮肤 通过查询数据库中的皮肤名称  赋值给皮肤下拉框
            //string Skin = string.Format("select * from BasicConfiguration");
            //DataSet dsSkin = db.getDataSet(Skin);
            //加载皮肤，通过查询数据库中的皮肤名称给Settings类，从Settings类中获取皮肤值
            this.comboBox1.Text = Settings.SkinName;

            //根据员工角色显示不同的界面
            if (LoginInfo.RoleName == "店长")
            {
                //显示员工姓名、角色和欢迎语
                string RoleName = LoginInfo.RoleName;
                this.label2.Text = (LoginInfo.UaerName + "(" + RoleName + "),欢迎您！");
            }
            else if (LoginInfo.RoleName == "导购员")
            {
                //显示员工姓名、角色和欢迎语
                string RoleName = LoginInfo.RoleName;
                this.label2.Text = (LoginInfo.UaerName + "(" + RoleName + "),欢迎您！");

                //默认隐藏选项卡中的部分标签
                this.tabPage2.Parent = null;
                this.tabPage4.Parent = null;
                //隐藏Panel控件
                this.panel5.Visible = false;
                this.panel8.Visible = false;
            }
            
            //默认隐藏选项卡中的部分标签
            this.tabPage6.Parent = null; 
            this.tabPage7.Parent = null;
            this.tabPage8.Parent = null;
            this.tabPage9.Parent = null;
            this.tabPage10.Parent = null;

            //禁止点击dataGridView1列标题排序
            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                this.dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            //禁止点击dataGridView2列标题排序
            for (int i = 0; i < this.dataGridView2.Columns.Count; i++)
            {
                this.dataGridView2.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            //禁止点击dataGridView3列标题排序
            for (int i = 0; i < this.dataGridView3.Columns.Count; i++)
            {
                this.dataGridView3.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            //禁止点击dataGridView4列标题排序
            for (int i = 0; i < this.dataGridView4.Columns.Count; i++)
            {
                this.dataGridView4.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            //禁止点击dataGridView5列标题排序
            for (int i = 0; i < this.dataGridView5.Columns.Count; i++)
            {
                this.dataGridView5.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            //移除首页原选项卡
            this.tabControl2.TabPages.Remove(tabPage5);

            //显示首页广告图片窗体  图片轮播效果
            TabPage tabPage = new TabPage();//新建选项卡
            tabPage.Name = "bomo";
            tabPage.Text = "首页";
            homePage form = new homePage();
            form.TopLevel = false;      //设置为非顶级控件
            form.Visible = true;
            tabPage.Controls.Add(form);
            tabControl2.TabPages.Add(tabPage);//把窗体添加到TabPages5中
            //form.Show();
            this.tabControl2.SelectedTab = tabPage;//显示窗体
        }

        //--------------------------------------------------------------------------------------------------------//
        //--------------------------------------------------------------------------------------------------------//


        //显示当前系统时间
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.label3.Text = DateTime.Now.ToString("yyyy年MM月dd日hh：mm：ss");
        }

        //--------------------------------------------------------------------------------------------------------//
        //--------------------------------------------------------------------------------------------------------//

        //tabControl2选项卡单击事件   刷新tabPage控件中的数据
        private void tabControl2_MouseClick(object sender, MouseEventArgs e)
        {
            ////商品浏览
            ////tabControl 2\tabPage 6
            ////DataGridView 1查询
            ////一级分类下拉框
            //string Sqldt1 = string.Format("select t1.TypeName from Type t1,Type t2 where t1.TypeID=t2.TypeID and t1.TypeID in('1','2','3');");
            //ds = db.getDataSet(Sqldt1);
            //DataRow dr = ds.Tables[0].NewRow();
            //dr[0] = "全部";
            //ds.Tables["nike"].Rows.InsertAt(dr, 0);
            //this.comboBox2.DataSource = ds.Tables["nike"];
            //this.comboBox2.DisplayMember = "TypeName";
            ////二级分类下拉框
            //string Sqldt12 = string.Format("select t2.TypeName,t2.ParentID from Type t1,Type t2 where t1.TypeID=t2.TypeID and t2.ParentID in('1','2','3');");
            //ds = db.getDataSet(Sqldt12);
            //DataRow dr2 = ds.Tables[0].NewRow();
            //dr2[0] = "全部";
            //ds.Tables["nike"].Rows.InsertAt(dr2, 0);
            //this.comboBox3.DataSource = ds.Tables["nike"];
            //this.comboBox3.DisplayMember = "TypeName";
            ////时间选择下拉框
            //this.comboBox6.Text = "全部";
            ////加载DataGridview 1数据
            //string Sqldd1 = string.Format("select * from Goods;");
            //DataSet ds3 = db.getDataSet(Sqldd1);
            //this.dataGridView1.AutoGenerateColumns = false;
            //this.dataGridView1.DataSource = ds3.Tables["nike"];
            ////加载当前商品条数
            //this.label14.Text = "当前共" + ds3.Tables["nike"].Rows.Count + "条商品信息！";

            ////--------------------------------------------------------------------------------------------------------//
            ////--------------------------------------------------------------------------------------------------------//
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////


            ////销售统计
            ////tabControl 2\tabPage 7
            ////DataGridView 2加载事件
            ////导购员下拉框
            //string Sql = string.Format("select SalesmanName from Salesan where Role='导购员'");
            //ds = db.getDataSet(Sql);
            //this.comboBox7.DataSource = ds.Tables["nike"];
            //this.comboBox7.DisplayMember = "SalesmanName";
            ////时间选择下拉框
            //this.comboBox4.Text = "全部";

            ////--------------------------------------------------------------------------------------------------------//
            ////--------------------------------------------------------------------------------------------------------//


            ////工资核算
            ////时间选择下拉框
            //this.comboBox11.Text = "全部";


            ////商品分类管理
            ////tabControl 2\tabPage 9
            ////DataGridView 4加载事件
            ////父级分类下拉框
            //string Sqldg2 = string.Format("select t1.TypeName from Type t1,Type t2 where t1.TypeID=t2.TypeID and t1.TypeID in('1','2','3');");
            //ds = db.getDataSet(Sqldg2);
            ////dr = ds.Tables[0].NewRow();
            ////dr[0] = "--请选择--";
            ////ds.Tables["nike"].Rows.InsertAt(dr, 0);
            //this.button4.Text = "新增分类";
            //this.comboBox14.DataSource = ds.Tables["nike"];
            //this.comboBox14.DisplayMember = "TypeName";
            //this.comboBox14.Text = "--请选择--";
            ////加载DataGridView4的数据
            //string Sqldv4 = string.Format("Select TypeID,TypeName,ParentID from Type;");
            //ds = db.getDataSet(Sqldv4);
            //this.dataGridView4.AutoGenerateColumns = false;
            //this.dataGridView4.DataSource = ds.Tables["nike"];
            //dataGridView4.ClearSelection();

            ////--------------------------------------------------------------------------------------------------------//
            ////--------------------------------------------------------------------------------------------------------//


            ////员工管理
            ////tabControl2\tabPage10
            ////员工角色下拉框
            //string Sqlxlk = string.Format("select SalesmanID,SalesmanName,Mobile,Gender,BaseSalary,CommissionRate,Role from Salesan;");
            //ds = db.getDataSet(Sqlxlk);
            //this.comboBox16.DataSource = ds.Tables["nike"];
            //this.comboBox16.DisplayMember = "Role";
            //this.button5.Text = "新增员工";
            //this.textBox8.Text = null;
            //this.textBox4.Text = null;
            //this.textBox5.Text = null;
            ////性别下拉框
            //this.comboBox15.Text = "--请选择--";
            //this.textBox6.Text = null;
            //this.textBox7.Text = null;
            ////员工角色下拉框默认值
            //this.comboBox16.Text = "--请选择--";
            ////DataGridView5加载事件
            //string Sqldv5 = string.Format("select SalesmanID,SalesmanName,Mobile,Gender,BaseSalary,CommissionRate,Role from Salesan;");
            //ds = db.getDataSet(Sqldv5);
            //this.dataGridView5.AutoGenerateColumns = false;
            //this.dataGridView5.DataSource = ds.Tables["nike"];
            //dataGridView5.ClearSelection();
        }

        //--------------------------------------------------------------------------------------------------------//
        //--------------------------------------------------------------------------------------------------------//


        //单击主窗体中的收银台图像打开收银台窗体
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Cashier cashier = new Cashier();
            cashier.Show();
        }

        //商品入库单击事件
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            //打开商品入库窗体
            Warehousing Wh = new Warehousing();
            Wh.ShowDialog();
        }

        //商品浏览，单击显示选项卡
        private void pictureBox4_Click(object sender, EventArgs e) 
        {
            //商品浏览
            //tabControl 2\tabPage 6
            //DataGridView 1查询
            //一级分类下拉框
            string Sqldt1 = string.Format("select t1.TypeName from Type t1,Type t2 where t1.TypeID=t2.TypeID and t1.TypeID in('1','2','3');");
            ds = db.getDataSet(Sqldt1);
            DataRow dr = ds.Tables[0].NewRow();
            dr[0] = "全部";
            ds.Tables["nike"].Rows.InsertAt(dr, 0);
            this.comboBox2.DataSource = ds.Tables["nike"];
            this.comboBox2.DisplayMember = "TypeName";
            //二级分类下拉框
            string Sqldt12 = string.Format("select t2.TypeName,t2.ParentID from Type t1,Type t2 where t1.TypeID=t2.TypeID and t2.ParentID in('1','2','3');");
             //string Sqldt12 = string.Format("select TypeName from Type where ParentID='{0}'", Type);
            ds = db.getDataSet(Sqldt12);
            DataRow dr2 = ds.Tables[0].NewRow();
            dr2[0] = "全部";
            ds.Tables["nike"].Rows.InsertAt(dr2, 0);
            this.comboBox3.DataSource = ds.Tables["nike"];
            this.comboBox3.DisplayMember = "TypeName";
            //时间选择下拉框
            this.comboBox6.Text = "全部";
            //加载DataGridview 1数据
            string Sqldd1 = string.Format("select * from Goods,Type where Goods.TypeID=Type.TypeID order by StockDate desc;");
            DataSet ds3 = db.getDataSet(Sqldd1);
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.DataSource = ds3.Tables["nike"];
            //加载当前商品条数
            this.label14.Text = "当前共" + ds3.Tables["nike"].Rows.Count + "条商品信息！";


            //打开选项卡
            this.tabPage6.Parent = this.tabControl2;
            //选中选项卡
            this.tabControl2.SelectedTab = tabPage6;
        }

        //退货单击事件
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            SalesReturn Return = new SalesReturn();
            Return.Show();
        }

        //销售统计，单击显示选项卡
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            //打开选项卡
            this.tabPage7.Parent = this.tabControl2;
            //选中选项卡
            this.tabControl2.SelectedTab = tabPage7;

            //销售统计
            //tabControl 2\tabPage 7
            //DataGridView 2加载事件
            //导购员下拉框
            string Sql = string.Format("select SalesmanName from Salesan where Role='导购员'");
            ds = db.getDataSet(Sql);
            DataRow dr = ds.Tables[0].NewRow();
            dr[0] = "全部";
            ds.Tables["nike"].Rows.InsertAt(dr, 0);
            this.comboBox7.DataSource = ds.Tables["nike"];
            this.comboBox7.DisplayMember = "SalesmanName";
            //时间选择下拉框
            this.comboBox4.Text = "全部";

            string SqlStr = string.Format(@"select ReceiptsCode,SalesDate,Amount,Sn.SalesmanName,S.CashierID from Sales S,Salesan Sn
                                       where S.SalesmanID=Sn.SalesmanID order by SalesDate desc");
            //调用销售统计显示“销售金额”“单笔利润”函数
            string Sal = dataGridView2Show(SqlStr);
        }

        //工资核算，单击显示选项卡
        private void pictureBox8_Click(object sender, EventArgs e)
        {
            //打开选项卡
            this.tabPage8.Parent = this.tabControl2;
            //选中选项卡
            this.tabControl2.SelectedTab = tabPage8;
            //工资核算
            //时间选择下拉框
            this.comboBox11.Text = "本月";

            //查询数据显示到DataGridView中
            string Sql = string.Format(@"select SalesmanName,Role,Mobile,BaseSalary,CommissionRate from Salesan");
            ds = db.getDataSet(Sql);
            this.dataGridView3.AutoGenerateColumns = false;
            this.dataGridView3.DataSource = ds.Tables["nike"];
        }

        //商品分类，单击显示选项卡
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            //打开选项卡
            this.tabPage9.Parent = this.tabControl2;
            //选中选项卡
            this.tabControl2.SelectedTab = tabPage9;

            //商品分类管理
            //tabControl 2\tabPage 9
            //DataGridView 4加载事件
            //父级分类下拉框
            string Sqldg2 = string.Format("select t1.TypeName from Type t1,Type t2 where t1.TypeID=t2.TypeID and t1.TypeID in('1','2','3');");
            ds = db.getDataSet(Sqldg2);
            //DataRow dr = ds.Tables[0].NewRow();
            //dr = ds.Tables[0].NewRow();
            //dr[0] = "--请选择--";
            //ds.Tables["nike"].Rows.InsertAt(dr, 0);
            this.button4.Text = "新增分类";
            this.comboBox14.DataSource = ds.Tables["nike"];
            this.comboBox14.DisplayMember = "TypeName";
            this.comboBox14.Text = "--请选择--";
            //加载DataGridView4的数据
            string Sqldv4 = string.Format("Select TypeID,TypeName,ParentID from Type order by ParentID;");
            ds = db.getDataSet(Sqldv4);
            this.dataGridView4.AutoGenerateColumns = false;
            this.dataGridView4.DataSource = ds.Tables["nike"];
            dataGridView4.ClearSelection();
        }

        //员工管理，单击显示选项卡
        private void pictureBox10_Click(object sender, EventArgs e)
        {
            //打开选项卡
            this.tabPage10.Parent = this.tabControl2;
            //选中选项卡
            this.tabControl2.SelectedTab = tabPage10;

            //员工管理
            //tabControl2\tabPage10
            //隐藏确认密码错误提示信息
            this.label37.Visible = false;
            //设置密码文本框为可编辑状态
            this.textBox8.ReadOnly = false;
            this.textBox9.ReadOnly = false;
            //文本框的背景颜色
            this.textBox8.BackColor = Color.White;
            this.textBox9.BackColor = Color.White;
            //字体，把密码显示为
            this.textBox8.Font = new Font("Wingdings", 9, textBox1.Font.Style);
            this.textBox8.PasswordChar = '|';
            //字体颜色
            this.textBox8.ForeColor = Color.Black;
            //清空文本框内容
            this.textBox8.Text = "";
            this.textBox9.Text = "";
            //员工角色下拉框
            string Sqlxlk = string.Format("select distinct Role from Salesan;");
            ds = db.getDataSet(Sqlxlk);
            this.comboBox16.DataSource = ds.Tables["nike"];
            this.comboBox16.DisplayMember = "Role";
            this.button5.Text = "新增员工";
            this.textBox4.Text = null;
            this.textBox5.Text = null;
            //性别下拉框
            this.comboBox15.Text = "--请选择--";
            this.textBox6.Text = null;
            this.textBox7.Text = null;
            //员工角色下拉框默认值
            this.comboBox16.Text = "--请选择--";
            //DataGridView5加载事件
            string Sqldv5 = string.Format("select SalesmanID,SalesmanName,Mobile,Gender,BaseSalary,CommissionRate,Role from Salesan;");
            ds = db.getDataSet(Sqldv5);
            this.dataGridView5.AutoGenerateColumns = false;
            this.dataGridView5.DataSource = ds.Tables["nike"];
            dataGridView5.ClearSelection();
        }

        //系统配置，单击显示选项卡
        private void pictureBox11_Click(object sender, EventArgs e)
        {
            SystemConfiguration Configuration = new SystemConfiguration();
            Configuration.ShowDialog();
        }

        //tabControl 2双击事件，双击隐藏选项卡控件标签
        private void tabControl2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //获取当前选中的Tab标签
            TabPage selectedPage = this.tabControl2.SelectedTab;
            //首页标签不允许移除
            if (selectedPage.Text == "首页")
            {
                return;
            }
            //移除选中的标签
            this.tabControl2.TabPages.Remove(selectedPage);
        }




        //--------------------------------------------------------------------------------------------------------//
        //商品浏览
        //--------------------------------------------------------------------------------------------------------//

        //商品浏览，选择日期区间，自动确定起始日期和结束日期
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            //获取系统当前日期
            DateTime dt = DateTime.Now;
            //获取今天是本月的第几天
            int dayOfMonth = dt.Day;
            switch (this.comboBox6.Text)
            {
                case "本日":
                    this.dtp_start.Value = dt;
                    this.dtp_end.Value = dt;
                    break;
                case "本周":
                    //获取今天是本周的第几天
                    int dayOfWeek = (int)dt.DayOfWeek;
                    if (dayOfWeek == 0)
                    {
                        dayOfWeek = 7;
                    }
                    this.dtp_start.Value = dt.AddDays(-dayOfWeek + 1);
                    this.dtp_end.Value = dt.AddDays(-dayOfWeek + 1 + 6);
                    break;
                case "本月":
                    this.dtp_start.Value = dt.AddDays(-dayOfMonth + 1);
                    this.dtp_end.Value = dt.AddDays(-dayOfMonth + 1).AddMonths(1).AddDays(-1);
                    break;
                case "上月":
                    this.dtp_start.Value = dt.AddDays(-dayOfMonth + 1).AddMonths(-1);
                    this.dtp_end.Value = dt.AddDays(-dayOfMonth + 1).AddDays(-1);
                    break;
                case "本年":
                    int dayOfYear = dt.DayOfYear;
                    this.dtp_start.Value = dt.AddDays(-dayOfYear + 1);
                    this.dtp_end.Value = dt.AddDays(-dayOfYear + 1).AddYears(1).AddDays(-1);
                    break;
                default:
                    break;
            }
        }
        //下拉框Text值改变时发生，当选择comboBox2下拉框值时改变comboBox3下拉框的值
        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            string browseCom = this.comboBox2.Text;
            //根据一级分类名称获取一级分类TypeID  SQL语句
            string yj = string.Format("select TypeID from Type where TypeName='{0}' ", browseCom);
            DataSet dsyj = db.getDataSet(yj);
            //判断comboBox1的值查出来是否有TypeIDID数据
            if (dsyj.Tables["nike"].Rows.Count != 0)
            {
                //根据一级分类名称获取一级分类TypeID
                string yjTypeID = dsyj.Tables["nike"].Rows[0]["TypeID"].ToString();
                //根据一级分类TypeID查询二级分类名称
                string Sql1 = string.Format("select t2.TypeName,t2.ParentID from Type t1,Type t2 where t1.TypeID=t2.ParentID and t2.ParentID in('{0}');", yjTypeID);
                DataSet ds1 = db.getDataSet(Sql1);
                DataRow dr = ds1.Tables[0].NewRow();
                dr[0] = "全部";
                ds1.Tables["nike"].Rows.InsertAt(dr, 0);
                this.comboBox3.DataSource = ds1.Tables["nike"];
                this.comboBox3.DisplayMember = "TypeName";
            }
            //如果选择的一级分类为全部则二级分类下拉框显示全部二级分类
            else if (this.comboBox2.Text == "全部")
            {
                //二级分类下拉框
                string Sqldt12 = string.Format("select t2.TypeName,t2.ParentID from Type t1,Type t2 where t1.TypeID=t2.TypeID and t2.ParentID in('1','2','3');");
                //string Sqldt12 = string.Format("select TypeName from Type where ParentID='{0}'", Type);
                ds = db.getDataSet(Sqldt12);
                DataRow dr2 = ds.Tables[0].NewRow();
                dr2[0] = "全部";
                ds.Tables["nike"].Rows.InsertAt(dr2, 0);
                this.comboBox3.DataSource = ds.Tables["nike"];
                this.comboBox3.DisplayMember = "TypeName";
            }
            else
            {
                return;
            }
        }
        //多条件组合查询商品函数
        private void QueryGoods(string goodsCode, string goodsName, string typeName, DateTime startTime, DateTime endTime)
        {
            //定义查询所有商品的SQL语句
            string SqlStr = "select GoodsID,BarCode,GoodsName,Type.TypeName,StorePrice,SalePrice,Discount,StockNum,StockDate from Goods,Type where Goods.TypeID=Type.TypeID";                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
            //DataSet ds = db.getDataSet(SqlStr);
            //根据用户的选择，拼接不同的条件                   
            if (goodsCode != "")
            {
                //货号/条形码
                //SqlStr = string.Format("select * from Goods,Type where Goods.TypeID=Type.TypeID and BarCode like '{0}%';", goodsCode);
                SqlStr += " and BarCode like '" + goodsCode + "%'";
            }
            if (goodsName != "")
            {
                //商品名称
                SqlStr += " and GoodsName like '%" + goodsName + "%'";
            }
            if (typeName != "全部")
            {
                //二级分类
                //用户选择了二级分类，按二级分类查询
                SqlStr += " and TypeName = '" + typeName + "'";
            }
            else if (this.comboBox2.Text != "全部")
            {
                //一级分类
                string TypeName1 = this.comboBox2.Text;
                //根据一级分类名称查询TypeID
                string Typeid = string.Format("select TypeID from Type where TypeName='{0}' ", TypeName1);
                DataSet dsId = db.getDataSet(Typeid);
                string Tpid = dsId.Tables["nike"].Rows[0]["TypeID"].ToString();
                //根据一级分类的TypeID用ParentID查询所有对应的商品
                //SqlStr = string.Format("select * from Goods,Type where Goods.TypeID=Type.TypeID and ParentID = '{0}';", Tpid);
                SqlStr += " and ParentID = '" + Tpid + "'";
            } 
            if (this.comboBox6.Text != "全部")
            {
                //入库时间
                //SqlStr = string.Format("select * from Goods,Type where Goods.TypeID=Type.TypeID and StockDate between'{0}' and '{1}';", startTime.ToString("yyyy-MM-dd- 00:00:00"), endTime.ToString("yyyy-MM-dd 23:59:59"));
                //SqlStr += "and StockDate between'" + startTime.ToString("yyyy-MM-dd- 00:00:00") + "' and '" + endTime.ToString("yyyy-MM-dd 23:59:59") + "'";
                //按入库时间降序排序
                SqlStr += "and StockDate between'" + startTime + "' and '" + endTime + "'";
            }
            //根据入库时间降序排列
            SqlStr += " order by StockDate desc";
            //执行SQL语句
            DataSet ds1 = db.getDataSet(SqlStr);
            //绑定DataGridView1，商品浏览
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.DataSource = ds1.Tables["nike"];
            //显示当前查询的商品条数
            this.label14.Text = "当前共" + ds1.Tables["nike"].Rows.Count + "条商品信息！";
        }
        //商品浏览，查询单击事件
        private void button1_Click(object sender, EventArgs e)        
        {
            string goodsCode = this.textBox1.Text;
            string goodsName = this.textBox2.Text;
            string typeName = this.comboBox3.Text;
            DateTime startTime = this.dtp_start.Value;
            DateTime endTime = this.dtp_end.Value;
            //调用多条件组合查询商品函数
            QueryGoods(goodsCode,goodsName,typeName,startTime,endTime);
        }




        //--------------------------------------------------------------------------------------------------------//
        //销售统计
        //--------------------------------------------------------------------------------------------------------//

        //销售统计下拉框，选择日期区间，自动确定起始日期和结束日期
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
             //获取系统当前日期
            DateTime dt = DateTime.Now;
            //获取今天是本月的第几天
            int dayOfMonth = dt.Day;
            switch (this.comboBox4.Text)
            {
                case "本日":
                    this.dtp_start2.Value = dt;
                    this.dtp_end2.Value = dt;
                    break;
                case "本周":
                    //获取今天是本周的第几天
                    int dayOfWeek = (int)dt.DayOfWeek;
                    if (dayOfWeek == 0)
                    {
                        dayOfWeek = 7;
                    }
                    this.dtp_start2.Value = dt.AddDays(-dayOfWeek + 1);
                    this.dtp_end2.Value = dt.AddDays(-dayOfWeek + 1 + 6);
                    break;
                case "本月":
                    this.dtp_start2.Value = dt.AddDays(-dayOfMonth + 1);
                    this.dtp_end2.Value = dt.AddDays(-dayOfMonth + 1).AddMonths(1).AddDays(-1);
                    break;
                case "上月":
                    this.dtp_start2.Value = dt.AddDays(-dayOfMonth + 1).AddMonths(-1);
                    this.dtp_end2.Value = dt.AddDays(-dayOfMonth + 1).AddDays(-1);
                    break;
                case "本年":
                    int dayOfYear = dt.DayOfYear;
                    this.dtp_start2.Value = dt.AddDays(-dayOfYear + 1);
                    this.dtp_end2.Value = dt.AddDays(-dayOfYear + 1).AddYears(1).AddDays(-1);
                    break;
                default:
                    break;
            }
        }

        //销售统计“查询按钮”单击事件
        private void button2_Click(object sender, EventArgs e)
        {

            DateTime startTime = this.dtp_start2.Value;
            DateTime endTime = this.dtp_end2.Value;
            string SqlStr = "";
            if (comboBox7.Text != "全部" && this.comboBox4.Text != "全部")
            {
                //定义变量获取下拉框值 获取导购员ID
                string SalesanName = string.Format("select SalesmanID from Salesan where SalesmanName='{0}'", this.comboBox7.Text);
                DataSet ds1 = db.getDataSet(SalesanName);
                string SalesmanID = ds1.Tables[0].Rows[0]["SalesmanID"].ToString();
                //按入库时间降序排序   编写SQL语句  根据导购员ID查询
                SqlStr = string.Format(@"select ReceiptsCode,SalesDate,Amount,Sn.SalesmanName,S.CashierID from Sales S,Salesan Sn
                                       where S.SalesmanID=Sn.SalesmanID and S.SalesmanID='{0}' and SalesDate between'{1}' and '{2}' 
                                       order by SalesDate desc", SalesmanID, startTime, endTime);
            }
            if (comboBox7.Text == "全部" && this.comboBox4.Text != "全部")
            {
                //按入库时间降序排序   编写SQL语句  根据交易时间查询并降序排序
                SqlStr = string.Format(@"select ReceiptsCode,SalesDate,Amount,Sn.SalesmanName,S.CashierID from Sales S,Salesan Sn
                                       where S.SalesmanID=Sn.SalesmanID and SalesDate between'{0}' and '{1}' 
                                       order by SalesDate desc;", startTime, endTime);
            }
            if (comboBox7.Text != "全部" && this.comboBox4.Text == "全部")
            {
                //定义变量获取下拉框值  获取导购员ID
                string SalesanName = string.Format("select SalesmanID from Salesan where SalesmanName='{0}'", this.comboBox7.Text);
                DataSet ds1 = db.getDataSet(SalesanName);
                string SalesmanID = ds1.Tables[0].Rows[0]["SalesmanID"].ToString();
                //按入库时间降序排序   编写SQL语句  根据导购员ID查询
                SqlStr = string.Format(@"select ReceiptsCode,SalesDate,Amount,Sn.SalesmanName,S.CashierID from Sales S,Salesan Sn
                                       where S.SalesmanID=Sn.SalesmanID and S.SalesmanID='{0}'
                                       order by SalesDate desc", SalesmanID);
            }
            if (this.comboBox7.Text == "全部" && this.comboBox4.Text == "全部")
            {
                //按入库时间降序排序   编写SQL语句  根据导购员或交易时间查询
                SqlStr = string.Format(@"select ReceiptsCode,SalesDate,Amount,Sn.SalesmanName,S.CashierID from Sales S,Salesan Sn
                                       where S.SalesmanID=Sn.SalesmanID order by SalesDate desc");
            }
            //调用销售统计显示“销售金额”“单笔利润”函数
            string Sal = dataGridView2Show(SqlStr);
        }
        //销售统计，单元格内容显示格式
        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //判断在要第几列改变显示值
            if (e.ColumnIndex == 5)
            {
                //遍历DataGridView2 获取每一行的收银员ID的值
                for (int i = 0; i < this.dataGridView2.Rows.Count; i++)
                {
                    //获取本行的收银员ID
                    string Cashier = this.dataGridView2.Rows[i].Cells[5].Value.ToString();
                    //判断显示值等于原值
                    if (e.Value.ToString() == Cashier)
                    {
                        //使用获取的每一行的收银员ID查收银员的姓名
                        string cH = string.Format("select SalesmanName from Salesan where SalesmanID='{0}'", Cashier);
                        DataSet ds = db.getDataSet(cH);
                        //将查询到的收银员姓名显示到DataGridView2对应的列中
                        e.Value = ds.Tables[0].Rows[0]["SalesmanName"].ToString();
                    }
                }
            }
        }





        //--------------------------------------------------------------------------------------------------------//
        //工资核算
        //--------------------------------------------------------------------------------------------------------//

        //工资核算，选择日期区间，自动确定起始日期和结束日期
        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
             //获取系统当前日期
            DateTime dt = DateTime.Now;
            //获取今天是本月的第几天
            int dayOfMonth = dt.Day;
            switch (this.comboBox11.Text)
            {
                case "本日":
                    this.dtp_start3.Value = dt;
                    this.dtp_end3.Value = dt;
                    break;
                case "本周":
                    //获取今天是本周的第几天
                    int dayOfWeek = (int)dt.DayOfWeek;
                    if (dayOfWeek == 0)
                    {
                        dayOfWeek = 7;
                    }
                    this.dtp_start3.Value = dt.AddDays(-dayOfWeek + 1);
                    this.dtp_end3.Value = dt.AddDays(-dayOfWeek + 1 + 6);
                    break;
                case "本月":
                    this.dtp_start3.Value = dt.AddDays(-dayOfMonth + 1);
                    this.dtp_end3.Value = dt.AddDays(-dayOfMonth + 1).AddMonths(1).AddDays(-1);
                    break;
                case "上月":
                    this.dtp_start3.Value = dt.AddDays(-dayOfMonth + 1).AddMonths(-1);
                    this.dtp_end3.Value = dt.AddDays(-dayOfMonth + 1).AddDays(-1);
                    break;
                case "本年":
                    int dayOfYear = dt.DayOfYear;
                    this.dtp_start3.Value = dt.AddDays(-dayOfYear + 1);
                    this.dtp_end3.Value = dt.AddDays(-dayOfYear + 1).AddYears(1).AddDays(-1);
                    break;
                default:
                    break;
            }
        }
        //工资核算，查询单击事件
        private void button3_Click(object sender, EventArgs e)
        {
            DateTime startTime = this.dtp_start3.Value;
            DateTime endTime = this.dtp_end3.Value;
            string Sql = string.Format(@"select SalesmanName,Role,Mobile,BaseSalary,CommissionRate from Salesan");
            DataSet dsc = db.getDataSet(Sql);
            this.dataGridView3.AutoGenerateColumns = false;
            this.dataGridView3.DataSource = dsc.Tables["nike"];

            //遍历DataGridView3
            for (int i = 0; i < this.dataGridView3.Rows.Count; i++)
            {
                //获取员工类型
                string Role = this.dataGridView3.Rows[i].Cells[2].Value.ToString();

                //获取联系方式
                string Mobile = this.dataGridView3.Rows[i].Cells[1].Value.ToString();
                string SqlStr = string.Format("select SalesmanID from Salesan where Mobile='{0}'", Mobile);
                ds = db.getDataSet(SqlStr);
                //通过联系方式获取导购员ID
                string Salesman = ds.Tables[0].Rows[0]["SalesmanID"].ToString();

                //计算 导购员月销售额 = 本月度导购员经手的销售金额总和 
                string sumShopAmount = string.Format(@"select sum(Sales.Amount) from Sales,Salesan 
                                     where Sales.SalesmanID=Salesan.SalesmanID and 
                                     SalesDate between '{0}' and '{1}' and Sales.SalesmanID='{2}'", startTime, endTime, Salesman);
                DataSet dsShop = db.getDataSet(sumShopAmount);

                //若查询的月销售额为空 就在DataGridView中显示“无”
                if (dsShop.Tables[0].Rows[0][0].ToString() == "")
                {
                    this.dataGridView3.Rows[i].Cells[5].Value = "无";
                }
                else
                {
                    //把导购员月销售额赋值给DataGridView中月销售额对应的列
                    this.dataGridView3.Rows[i].Cells[5].Value = dsShop.Tables[0].Rows[0][0].ToString();
                }

                //计算 店长月度销售额 = 店内所有导购员月度销售总和 - 当月销售额考核保底
                string sumManagerAmount = string.Format(@"select sum(Sales.Amount) from Sales,Salesan 
                                     where Sales.SalesmanID=Salesan.SalesmanID and 
                                     SalesDate between '{0}' and '{1}'", startTime, endTime);
                DataSet dsManager = db.getDataSet(sumManagerAmount);
                //当等于遍历的行为店长信息时进入此判断
                if (Role == "店长")
                {
                    if (dsManager.Tables[0].Rows[0][0].ToString() == "")
                    {
                        this.dataGridView3.Rows[i].Cells[5].Value = "无";
                    }
                    else
                    {
                        //把店长月销售额赋值给DataGridView中月销售额对应的列
                        this.dataGridView3.Rows[i].Cells[5].Value = Convert.ToDouble(dsManager.Tables[0].Rows[0][0].ToString())-Settings.BaseSaleroom;
                    }
                }
                //获取基本工资列
                string basic = this.dataGridView3.Rows[i].Cells[3].Value.ToString();
                //获取提成比例列
                string deductionWage = this.dataGridView3.Rows[i].Cells[4].Value.ToString();
                if (Role == "收银员")
                {
                    //应发工资 = 基本工资
                    this.dataGridView3.Rows[i].Cells[6].Value = (Convert.ToDecimal(basic)).ToString("F2");
                }
                else if (Role == "导购员")
                {
                    //判断有没有销售额
                    if (this.dataGridView3.Rows[i].Cells[5].Value.ToString() != "无")
                    {
                        //应发工资 = 基本工资 + 本人月销售额 * 提成比率
                        this.dataGridView3.Rows[i].Cells[6].Value = ((Convert.ToDecimal(basic) + (Convert.ToDecimal(this.dataGridView3.Rows[i].Cells[5].Value)) * Convert.ToDecimal(deductionWage))).ToString("F2");
                    }
                    else
                    {
                        this.dataGridView3.Rows[i].Cells[6].Value = (Convert.ToDecimal(basic)).ToString("F2");
                    }
                }
                else if (Role == "店长")
                {
                    if (this.dataGridView3.Rows[i].Cells[5].Value.ToString() != "无")
                    {
                        //应发工资 = 基本工资 + （全店月度销售总额 - 月度考核保底销售额） * 提成比率
                        this.dataGridView3.Rows[i].Cells[6].Value = ((Convert.ToDecimal(basic)) + (Convert.ToDecimal(Convert.ToDouble(dsManager.Tables[0].Rows[0][0].ToString())-Settings.BaseSaleroom) * Convert.ToDecimal(deductionWage))).ToString("F2");
                    }
                    else
                    {
                        this.dataGridView3.Rows[i].Cells[6].Value = (Convert.ToDecimal(basic)).ToString("F2");
                    }
                }
            }

        }
        //工资核算，收银员提成比例显示值
        private void dataGridView3_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                if (e.Value.ToString() == "")
                {
                    e.Value = "无";
                }
            }
        }




        //--------------------------------------------------------------------------------------------------------//
        //商品分类管理
        //--------------------------------------------------------------------------------------------------------//

        //商品分类管理,选中一行时发生
        private void dataGridView4_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            this.button4.Text = "修改分类";
            //获取DataGridView选中行的单元格内容到文本框
            this.textBox3.Text = this.dataGridView4.SelectedRows[0].Cells[1].Value.ToString();
            this.comboBox14.Text = this.dataGridView4.SelectedRows[0].Cells[2].Value.ToString();
            if (this.comboBox14.Text == "1")
            {
                this.comboBox14.Text = "鞋类";
                return;
            }
            else if (this.comboBox14.Text == "2")
            {
                this.comboBox14.Text = "服装";
                return;
            }
            else if (this.comboBox14.Text == "3")
            {
                this.comboBox14.Text = "户外装备";
                return;
            }
        }
        //“新增分类”“修改分类”按钮单击事件
        private void button4_Click(object sender, EventArgs e)
        {
            string type = this.textBox3.Text;
            string ParID = this.comboBox14.Text;
            if (ParID == "鞋类")
            {
                ParID = "1";
            }
            else if (ParID == "服装")
            {
                ParID = "2";
            }
            else if (ParID == "户外装备")
            {
                ParID = "3";
            }
            //根据按钮中的文本执行对应的SQL语句
            if (this.button4.Text == "新增分类")
            {
                string Sqlinsert = string.Format("insert into Type(TypeName,ParentID) values ('{0}','{1}');", type, ParID);
                int rows = db.zsg(Sqlinsert);
                if (rows > 0)
                {
                    MessageBox.Show("新增成功");
                    string Sqldv4 = string.Format("Select TypeID,TypeName,ParentID from Type;");
                    ds = db.getDataSet(Sqldv4);
                    this.dataGridView4.AutoGenerateColumns = false;
                    this.dataGridView4.DataSource = ds.Tables["nike"];
                    dataGridView4.ClearSelection();
                }
                else
                {
                    MessageBox.Show("新增失败！");
                }
            }
            else if (this.button4.Text == "修改分类")
            {
                string TypeID = this.dataGridView4.SelectedRows[0].Cells[0].Value.ToString();
                string Sqlupdate = string.Format("update Type set TypeName='{0}',ParentID='{1}' where TypeID='{3}'", type, ParID, TypeID);
                int rows = db.zsg(Sqlupdate);
                if (rows > 0)
                {
                    MessageBox.Show("修改成功");
                    string Sqldv4 = string.Format("Select TypeID,TypeName,ParentID from Type;");
                    ds = db.getDataSet(Sqldv4);
                    this.dataGridView4.AutoGenerateColumns = false;
                    this.dataGridView4.DataSource = ds.Tables["nike"];
                    dataGridView4.ClearSelection();
                }
                else
                {
                    MessageBox.Show("修改失败！");
                }
            }

        }
        //设置商品分类管理单元格内容显示格式
        private void dataGridView4_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                if (e.Value.ToString() == "")
                {
                    e.Value = "无";
                }
                else if (e.Value.ToString() == "1")
                {
                    e.Value = "鞋类";
                }
                else if (e.Value.ToString() == "2")
                {
                    e.Value = "服装";
                }
                else if (e.Value.ToString() == "3")
                {
                    e.Value = "户外装备";
                }
            }
        }




        //--------------------------------------------------------------------------------------------------------//
        //员工管理
        //--------------------------------------------------------------------------------------------------------//
        //员工管理,选中一行时发生
        private void dataGridView5_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //隐藏确认密码不一致提示信息
            this.label37.Visible = false;
            //禁止修改密码，设置文本框为不可编辑状态
            this.textBox8.ReadOnly = true;
            this.textBox9.ReadOnly = true;
            //在密码文本框中提示用户怎样 新增员工
            //改变文本框的背景颜色
            this.textBox8.BackColor = Color.Gainsboro;
            this.textBox9.BackColor = Color.Gainsboro;
            //设置文本框的字体的样式
            this.textBox8.Font = new Font("宋体", 9);
            this.textBox8.PasswordChar = '\0';
            this.textBox8.Text = "点击上方“员工管理”图标新增员工！";
            this.textBox8.ForeColor = Color.Red;
            this.textBox9.Text = "";
            //显示按钮文本为“修改员工”
            this.button5.Text = "修改员工";
            //获取DataGridView选中行的单元格内容到文本框
            this.textBox4.Text = this.dataGridView5.SelectedRows[0].Cells[1].Value.ToString();
            this.textBox5.Text = this.dataGridView5.SelectedRows[0].Cells[2].Value.ToString();
            this.comboBox15.Text = this.dataGridView5.SelectedRows[0].Cells[3].Value.ToString();
            this.textBox6.Text = this.dataGridView5.SelectedRows[0].Cells[4].Value.ToString();
            this.textBox7.Text = this.dataGridView5.SelectedRows[0].Cells[5].Value.ToString();
            this.comboBox16.Text = this.dataGridView5.SelectedRows[0].Cells[6].Value.ToString();
        }
        //“修改员工”“新增员工”按钮单击事件
        private void button5_Click(object sender, EventArgs e)
        {
            string name = this.textBox4.Text;
            string phone = this.textBox5.Text;
            string gender = this.comboBox15.Text;
            string basesalar = this.textBox6.Text;
            string deduct = this.textBox7.Text;
            string pwd = this.textBox8.Text;
            string affirmPwd = this.textBox9.Text;
            string role = this.comboBox16.Text;
            if (this.button5.Text == "修改员工")
            {
                string ID = this.dataGridView5.SelectedRows[0].Cells[0].Value.ToString();
                string Sqlupdate = "";
                if (deduct == "")
                {
                    deduct = "null";
                    Sqlupdate = string.Format(@"update Salesan set SalesmanName='{0}',Mobile='{1}',Gender='{2}',BaseSalary='{3}',CommissionRate={4},Role='{5}' where SalesmanID='{6}'",
                                                          name, phone, gender, basesalar, deduct, role, ID);
                }
                else
                {
                    Sqlupdate = string.Format(@"update Salesan set SalesmanName='{0}',Mobile='{1}',Gender='{2}',BaseSalary='{3}',CommissionRate='{4}',Role='{5}' where SalesmanID='{6}'",
                                                          name, phone, gender, basesalar, deduct, role, ID);
                }
                int rows = db.zsg(Sqlupdate);
                if (rows > 0)
                {
                    MessageBox.Show("修改成功！");
                    //DataGridView5刷新
                    string Sqldv5 = string.Format("select * from Salesan;");
                    ds = db.getDataSet(Sqldv5);
                    this.dataGridView5.AutoGenerateColumns = false;
                    this.dataGridView5.DataSource = ds.Tables["nike"];
                    this.dataGridView5.ClearSelection();
                }
                else
                {
                    MessageBox.Show("修改失败！");
                }
            }
            else if(this.button5.Text == "新增员工")
            {
                if (this.textBox4.Text == null || this.textBox5.Text == null || this.comboBox15.Text == "--请选择--" || this.comboBox15.Text == null || this.textBox6.Text == null || this.textBox8.Text == null || this.textBox9.Text == "" || this.comboBox16.Text == "--请选择--" || this.comboBox16.Text == null)
                {
                    MessageBox.Show("请完善员工信息！");
                }
                else if (affirmPwd != pwd)
                {
                    this.label37.Visible = true;
                    this.label37.Text = "两次输入的密码不一致，请确认！";
                    //设置提示的字体颜色为红色
                    //this.label37.ForeColor = Color.Red;
                    this.label37.ForeColor = ColorTranslator.FromHtml("#ff0000");
                }
                else
                {
                    string Sqlinsert = "";
                    //判断是否有提成比例
                    if (deduct == "")
                    {
                        deduct = "null";
                        Sqlinsert = string.Format(@"insert into Salesan(SalesmanName,Mobile,Pwd,Gender,BaseSalary,CommissionRate,Role) Values ('{0}','{1}','{2}','{3}','{4}',{5},'{6}')",
                    name, phone, pwd, gender, basesalar, deduct, role);
                    }
                    else
                    {
                        Sqlinsert = string.Format(@"insert into Salesan(SalesmanName,Mobile,Pwd,Gender,BaseSalary,CommissionRate,Role) Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                        name, phone, pwd, gender, basesalar, deduct, role);
                    }
                    int rows = db.zsg(Sqlinsert);
                    if (rows > 0)
                    {
                        //隐藏确认密码不一致提示信息
                        this.label37.Visible = false;
                        MessageBox.Show("新增成功！");
                        //DataGridView5刷新
                        string Sqldv5 = string.Format("select SalesmanID,SalesmanName,Mobile,Pwd,Gender,BaseSalary,CommissionRate,Role from Salesan;");
                        ds = db.getDataSet(Sqldv5);
                        this.dataGridView5.AutoGenerateColumns = false;
                        this.dataGridView5.DataSource = ds.Tables["nike"];
                        this.dataGridView5.ClearSelection();
                    }
                    else
                    {
                        MessageBox.Show("新增失败！");
                    }
                }
            }
        }

        //新增员工确认密码判断
        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if (this.textBox8.Text != this.textBox9.Text)
            {
                this.label37.Visible = true;
                this.label37.Text = "两次输入的密码不一致，请确认！";
                //设置提示的字体颜色为红色
                //this.label37.ForeColor = Color.Red;
                this.label37.ForeColor = ColorTranslator.FromHtml("#ff0000");
            }
            else
            {
                this.label37.Visible = false;
            }
        }




        //点击关闭窗体，弹出提示框
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult Result = MessageBox.Show("确定要关闭窗体吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (Result == DialogResult.Yes)
            {
                e.Cancel = false;
                //Application.Exit();
            }
            else
            {
                e.Cancel = true;
            }
        }

    }
}
