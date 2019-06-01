using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace NIKE_Project
{
    public partial class homePage : Form
    {
        public homePage()
        {
            InitializeComponent();
        }

        //创建线程
        Thread th;
        private void ChangeImage(Image img, int millisecondsTimeOut)
        {

            //this.Invoke(new Action(() =>
            //{

            //    pictureBox1.Image = img;

            //})

            //    );
            pictureBox1.Image = img;//显示图片到pictureBox1控件中
            Thread.Sleep(millisecondsTimeOut);//切换图片间隔时间

        }

        private void homePage_Load(object sender, EventArgs e)
        {
            th = new Thread

               (
                   delegate()
                   {

                       //设置循环轮数 （这里设置为无限循环）

                       for (int i = 1; i > 0; i++)
                       {

                           //调用方法

                           //设置图片的位置和显示时间（1000 为1秒）

                           //string picPath = Application.StartupPath + "\\pic01.jpg";

                           ChangeImage(Image.FromFile("Ggimg/pic01.jpg"), 3000);
                           //ChangeImage(Image.FromFile("Ggimg/pic02.jpg"), 3000);
                           ChangeImage(Image.FromFile("Ggimg/pic03.jpg"), 3000);
                           ChangeImage(Image.FromFile("Ggimg/pic03.jpg"), 3000);
                           ChangeImage(Image.FromFile("Ggimg/pic04.jpg"), 3000);
                           ChangeImage(Image.FromFile("Ggimg/pic05.jpg"), 3000);
                           ChangeImage(Image.FromFile("Ggimg/pic06.jpg"), 3000);
                           ChangeImage(Image.FromFile("Ggimg/pic07.jpg"), 3000);
                           ChangeImage(Image.FromFile("Ggimg/pic08.jpg"), 3000);
                           ChangeImage(Image.FromFile("Ggimg/pic09.jpg"), 3000);

                       }

                   }
               );

            th.IsBackground = true;

            th.Start();
        }

    }
}
