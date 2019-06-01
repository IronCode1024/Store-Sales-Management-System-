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
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        //引用DBHelper类
        DBHelper db = new DBHelper();

        //注册窗体加载事件 
        private void register_Load(object sender, EventArgs e)
        {
            //默认选中男
            this.radioButton1.Checked = true;
            this.label13.Visible = false;
        }

        //注册单击事件
        private void button1_Click(object sender, EventArgs e)
        {
            string SalesmanName=this.textBox1.Text;
            string Mobile=this.textBox2.Text;
            string Pwd=this.textBox3.Text;
            string Pwd1 = this.textBox4.Text;
            string Role=this.comboBox1.Text;
            string BaseSalary = this.textBox5.Text;
            string CommissionRate = this.textBox6.Text;
            string Gender="";
            //判断单选按钮的值
            if(this.radioButton1.Checked)
            {
                Gender="男";
            }
            else if(this.radioButton2.Checked)
            {
                Gender="女";
            }
            //判断文本框是否有输入值，向数据库添加用户
            if (SalesmanName == "" || Mobile == "" || Pwd == "" || Role == "" || BaseSalary == "")
            {
                    MessageBox.Show("员工信息不能为空！");
            }
            else if (Pwd != Pwd1)
            {
                this.label13.Visible = true;
            }
            //else if (Pwd == Pwd1)
            //{
            //    this.label13.Visible = false;
            //}
            else if (SalesmanName != "" || Mobile != "" || Pwd != "" || Role != "" || BaseSalary != "")
            {
                if (this.comboBox1.Text == "店长")
                {
                    //注册店长SQl（店长有初始密码，所以这里是直接根据初始密码修改店长信息）
                    string Sql;
                        Sql = string.Format(@"update Salesan set SalesmanName='{0}',Mobile='{1}',Pwd='{2}',Gender='{3}',Role='{4}',
                    BaseSalary='{5}',CommissionRate='{6}' where Role='店长'", SalesmanName, Mobile, Pwd, Gender, Role, BaseSalary, CommissionRate);

                    int rows = db.zsg(Sql);
                    if (rows > 0)
                    {
                        this.label13.Visible = false;
                        DialogResult Result = MessageBox.Show("店长注册成功","提示",MessageBoxButtons.OK);
                        if (Result == DialogResult.OK)
                        {
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("注册失败，请重新注册！");
                    }
                }
                else
                {
                    MessageBox.Show("此注册只能为店长!");
                }
            }
        }

        //确认密码判断
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (this.textBox3.Text != this.textBox4.Text)
            {
                this.label13.Visible = true;
                this.label13.Text = "两次输入的密码不一致，请确认！";
                //设置提示的字体颜色为红色
                //this.label37.ForeColor = Color.Red;
                this.label13.ForeColor = ColorTranslator.FromHtml("#ff0000");
            }
            else
            {
                this.label13.Visible = false;
            }
        }

        //取消单击事件
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}
