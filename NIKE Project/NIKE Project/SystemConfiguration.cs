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
    public partial class SystemConfiguration : Form
    {
        public SystemConfiguration()
        {
            InitializeComponent();
        }

        //调用DBHelper类
        DBHelper db = new DBHelper();

        //系统配置窗体加载事件
        private void SystemConfiguration_Load(object sender, EventArgs e)
        {
            //加载基本配置信息 从数据库查出来并显示到相应的文本框中
            string Base = string.Format("select * from BasicConfiguration");
            DataSet Bs = db.getDataSet(Base);
            this.textBox1.Text = Bs.Tables["nike"].Rows[0]["DoorShopName"].ToString();
            this.textBox2.Text = Bs.Tables["nike"].Rows[0]["Skin"].ToString();
            this.textBox3.Text = Bs.Tables["nike"].Rows[0]["PicturePath"].ToString();
            this.textBox4.Text = Bs.Tables["nike"].Rows[0]["Assess"].ToString();
        }

        //基本配置“确定”按钮单击事件
        private void button1_Click(object sender, EventArgs e)
        {
            //向数据库更新基本配置信息
            string sqlStr = string.Format(@"update BasicConfiguration set DoorShopName='{0}',Skin='{1}',PicturePath='{2}',Assess='{3}' 
                                          where ConfigurationID='1'", this.textBox1.Text, this.textBox2.Text, this.textBox3.Text, this.textBox4.Text);
            int Rows = db.zsg(sqlStr);
            if (Rows > 0)
            {
                //更新成功，记录更新的数据，并赋值给Settings类中相应的变量临时保存，以方便调用
                string Base = string.Format("select * from BasicConfiguration");
                DataSet Bs = db.getDataSet(Base);
                Settings.ShopName = Bs.Tables["nike"].Rows[0]["DoorShopName"].ToString();
                Settings.SkinName = Bs.Tables["nike"].Rows[0]["Skin"].ToString();
                Settings.AdImagePath = Bs.Tables["nike"].Rows[0]["PicturePath"].ToString();
                Settings.BaseSaleroom = Convert.ToDouble(Bs.Tables["nike"].Rows[0]["Assess"].ToString());
                this.Hide();
            }
            else
            {
                MessageBox.Show("配置失败！");
            }
        }
        //进本配置
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
