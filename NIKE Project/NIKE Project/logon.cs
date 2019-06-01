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
    public partial class logon : Form
    {
        public logon()
        {
            InitializeComponent();
        }

        //引用DBHelper类
        DBHelper db = new DBHelper();

        //登录窗体加载事件
        private void logon_Load(object sender, EventArgs e)
        {
            //隐藏登录中、登录成功label控件
            this.label3.Visible = false;
        }
        //登录单击事件
        private void button1_Click(object sender, EventArgs e)
        {
            string userName = this.textBox1.Text;
            string password = this.textBox2.Text;
            if (userName == "" || password == "")
            {
                MessageBox.Show("用户名或密码不能为空");
                return;
            }
            else
            {
                string SqlUse = string.Format("select SalesmanID,SalesmanName,Mobile,Pwd,Gender,BaseSalary,CommissionRate,Role from Salesan where Mobile='{0}';", userName);

                //int rows = db.logons(SqlUse);
                DataSet ds = db.getDataSet(SqlUse);
                //判断用户名是否正确
                if (ds.Tables["nike"].Rows.Count==0)
                {
                    MessageBox.Show("该用户不存在！", "登录失败：", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (ds.Tables[0].Rows[0]["Mobile"].ToString() == userName)
                {
                    string SqlPwd = string.Format("select Mobile,Pwd from Salesan where Mobile='{0}' and Pwd='{1}';",userName, password);
                    DataSet ds1 = db.getDataSet(SqlPwd);
                    //判断密码是否正确
                    if (ds.Tables[0].Rows[0]["Pwd"].ToString() != password)
                    {
                        MessageBox.Show("密码错误！", "登录失败：", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        //禁用登录界面linkLabel1控件
                        this.linkLabel1.Enabled = false;
                        //隐藏登录界面所有控件
                        this.label1.Visible = false;
                        this.label2.Visible = false;
                        this.textBox1.Visible = false;
                        this.textBox2.Visible = false;
                        this.button1.Visible = false;
                        this.button2.Visible = false;
                        this.linkLabel1.Visible = false;


                        //Label label = new Label();
                        //label.Location = new System.Drawing.Point(0, 50);//设置位置
                        //label.Size = new Size(20, 30);//设置大小
                        //label.Text = "11111";//设置Text值
                        //this.Controls.Add(label);//在当前窗体上添加这个label控件
                        ////label.Visible = true;
                        ////label.Text = "登录成功";

                        //显示登录成功label
                        this.label3.Visible = true;
                        this.label3.Text = "登录成功";

                        //登录成功，保存登录信息
                        LoginInfo.UserID = ds.Tables[0].Rows[0]["SalesmanID"].ToString();
                        LoginInfo.LoginID = ds.Tables[0].Rows[0]["Mobile"].ToString();
                        LoginInfo.UaerName = ds.Tables[0].Rows[0]["SalesmanName"].ToString();
                        LoginInfo.RoleName = ds.Tables[0].Rows[0]["Role"].ToString();
                        //启动计时器
                        this.timer1.Enabled = true;
                    }
                }
            }
        }
        //根据计时器事件显示相应的窗体界面 
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Hide();
            //MessageBox.Show("登录成功");
            //根据角色不同，进入不同的界面
            if (LoginInfo.RoleName == "收银员")
            {
                //显示收银员界面
                new Cashier().Show();
            }
            else if (LoginInfo.RoleName == "导购员")
            {
                //显示导购员界面
                new MainForm().Show();
            }
            else
            {
                //显示店长主界面
                new MainForm().Show();
            }
            //关闭计时器
            this.timer1.Enabled = false;
        }

        //单击注册，打开注册窗体
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //打开注册权限提示窗体
            limit lm = new limit();
            lm.Show();
        }

        //窗体第一次显示是发生   显示鼠标光标在TextBox1上
        private void logon_Shown(object sender, EventArgs e)
        {
            this.textBox1.Focus();
        }

        //单击取消按钮关闭窗体
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //退出程序

            //有提示
            Application.Exit();

            //无提示
            //this.Close();
        }

        private void 系统配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemConfiguration Configuration = new SystemConfiguration();
            Configuration.ShowDialog();
        }

    }
}
