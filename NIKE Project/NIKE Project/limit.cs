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
    public partial class limit : Form
    {
        public limit()
        {
            InitializeComponent();
        }

        //引用DBHelper类
        DBHelper db=new DBHelper();


        private void limit_Load(object sender, EventArgs e)
        {
            this.textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string password = this.textBox1.Text;
            if (password == "")
            {
                MessageBox.Show("用户名或密码不能为空");
                return;
            }
            else
            {
                string Sql = string.Format("select SalesmanID,SalesmanName,Mobile,Pwd,Gender,BaseSalary,CommissionRate,Role from Salesan;", password);
                DataSet ds = db.getDataSet(Sql);

                if (ds.Tables["nike"].Rows.Count == 0)
                {
                    MessageBox.Show("该用户不存在！", "授权失败：", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (ds.Tables[0].Rows[0]["Pwd"].ToString() != password)
                {
                    MessageBox.Show("密码错误！", "授权失败：", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    MessageBox.Show("授权成功");
                    //打开注册窗体
                    Register zc = new Register();
                    zc.Show();
                    this.Close();
                }
            }
        }
    }
}
