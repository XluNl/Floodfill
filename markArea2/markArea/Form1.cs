using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace markArea
{
    public partial class Form1 : Form
    {
        getArea m_gArea = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //读取原始图片各像素点的像素值，黑色点标为1，白色点标为0。
            Bitmap bimg = (Bitmap)Bitmap.FromFile("1.png");
            
            if (m_gArea == null)
            {
                
                m_gArea = new getArea(bimg.Width, bimg.Height);
                for (int i = 0; i < bimg.Height; ++i)
                {
                    for (int j = 0; j < bimg.Width; ++j)
                    {
                        Color cr = bimg.GetPixel(j, i);
                        if (cr.R == ((byte)255)) { m_gArea.setPixelColor(j, i, ((byte)0)); }
                        else
                        {
                            m_gArea.setPixelColor(j, i, ((byte)1));
                        }

                    }
                }
                //将原始图片绘制在窗口中。
                g.DrawImage(bimg, new Point(0, 0));
            }
            

            //更新界面显示内容
            for (int i = 0; i < bimg.Height; ++i)
            {
                for (int j = 0; j < bimg.Width; ++j)
                {
                    if (m_gArea.getPixColor(j,i) == 2)
                    {
                        bimg.SetPixel(j, i, Color.Red);
                    }
                }
            }
            g.DrawImage(bimg, new Point(0, 0));
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            m_gArea.setStartPoint(x, y);
            this.Refresh();                     //更新界面显示内容后强制刷新界面
        }
    }

    //广度优先实现的渗水算法
    public class getArea
    {

        byte[] m_img_data = null;               //用于记录图片各像素点的颜色值，
        Point[] m_edge1 = null;                 //用于记录处于过程中最后更新的边缘点
        Point[] m_edge2 = null;                 //用于记录处于过程中最后更新的边缘点
        int m_width = 0;                        //图片宽度
        int m_height = 0;                       //图片高度
        int m_edge1_count = 0;                  //记录m_edge1有最后更新点的个数，以便结束一轮操作
        int m_edge2_count = 0;                  //记录m_edge2有最后更新点的个数，以便结束一轮操作


        public getArea(int width, int height)
        {
            //初始化存储数据的结构
            if (width > 0 && height > 0)
            {
                m_img_data = new byte[width * height];
                m_edge1 = new Point[width * 2 + height * 2];
                m_edge2 = new Point[width * 2 + height * 2];
                m_width = width;
                m_height = height;
            }
        }

        public int getWidth() { return m_width; }
        public int getHeight() { return m_height; }

        //设置m_img_data中的初始数据，即各像素点的颜色值，白为0，黑为1
        public void setPixelColor(int x, int y, byte color)
        {
            m_img_data[y * m_width + x] = color;
        }

        //设置初始点，并计算最大联通区域
        public void setStartPoint(int x, int y)
        {
            //检测输入值的合法性
            if (x < 0 || x >= m_width || y < 0 || y > m_height) return;

            

            m_edge1[0].X = x; m_edge1[0].Y = y;
            if (m_img_data[y * m_width + x] == 1)
            {
                //用户点中的是边框
                return;
            }
            else if (m_img_data[y * m_width + x] == 2)
            {
                //本次操作的区域与用户最后一次操作的区域相同，无须处理。
                return;
            }
            else{
                //清空上轮填充的区域，后续可考虑优化
                for (int i = 0; i < m_width * m_height; ++i)
                {
                    if (m_img_data[i] == 2) m_img_data[i] = 0;
                }
                m_img_data[y * m_width + x] = 2;
            }
            m_edge1_count = 1;
            m_edge2_count = 0;
            while (m_edge1_count > 0 || m_edge2_count > 0)
            {

                //以用户操作的初始点作为起始点，开始扩展,拓展方式为每个初始点的上下左右四个方向。
                for (int i = 0; i < m_edge1_count; ++i)
                {
                    Point p = m_edge1[i];
                    //上
                    if (p.Y - 1 >= 0 && m_img_data[(p.Y - 1) * m_width + p.X] == 0)
                    {
                        m_img_data[(p.Y - 1) * m_width + p.X] = 2;
                        m_edge2[m_edge2_count].X = (p.X);
                        m_edge2[m_edge2_count++].Y = (p.Y - 1);
                    }
                    //左
                    if (p.X - 1 >= 0 && m_img_data[(p.Y) * m_width + (p.X - 1)] == 0)
                    {
                        m_img_data[(p.Y) * m_width + (p.X - 1)] = 2;
                        m_edge2[m_edge2_count].X = (p.X - 1);
                        m_edge2[m_edge2_count++].Y = (p.Y);
                    }
                    //右
                    if (p.X + 1 < m_width && m_img_data[(p.Y) * m_width + (p.X + 1)] == 0)
                    {
                        m_img_data[(p.Y) * m_width + (p.X + 1)] = 2;
                        m_edge2[m_edge2_count].X = (p.X + 1);
                        m_edge2[m_edge2_count++].Y = (p.Y);
                    }
                    //下
                    if (p.Y + 1 < m_height && m_img_data[(p.Y + 1) * m_width + (p.X)] == 0)
                    {
                        m_img_data[(p.Y + 1) * m_width + (p.X)] = 2;
                        m_edge2[m_edge2_count].X = (p.X);
                        m_edge2[m_edge2_count++].Y = (p.Y + 1);
                    }
                }
                m_edge1_count = 0;

                for (int j = 0; j < m_edge2_count; ++j)
                {
                    Point p = m_edge2[j];
                    //上
                    if (p.Y - 1 >= 0 && m_img_data[(p.Y - 1) * m_width + p.X] == 0)
                    {
                        m_img_data[(p.Y - 1) * m_width + p.X] = 2;
                        m_edge1[m_edge1_count].X = (p.X);
                        m_edge1[m_edge1_count++].Y = (p.Y - 1);
                    }
                    //左
                    if (p.X - 1 >= 0 && m_img_data[(p.Y) * m_width + (p.X - 1)] == 0)
                    {
                        m_img_data[(p.Y) * m_width + (p.X - 1)] = 2;
                        m_edge1[m_edge1_count].X = (p.X - 1);
                        m_edge1[m_edge1_count++].Y = (p.Y);
                    }
                    //右
                    if (p.X + 1 < m_width && m_img_data[(p.Y) * m_width + (p.X + 1)] == 0)
                    {
                        m_img_data[(p.Y) * m_width + (p.X + 1)] = 2;
                        m_edge1[m_edge1_count].X = (p.X + 1);
                        m_edge1[m_edge1_count++].Y = (p.Y);
                    }
                    //下
                    if (p.Y + 1 < m_height && m_img_data[(p.Y + 1) * m_width + (p.X)] == 0)
                    {
                        m_img_data[(p.Y + 1) * m_width + (p.X)] = 2;
                        m_edge1[m_edge1_count].X = (p.X);
                        m_edge1[m_edge1_count++].Y = (p.Y + 1);
                    }
                }
                m_edge2_count = 0;
            }
        }

        public byte getPixColor(int x, int y)
        {
            return m_img_data[y * m_width + x];
        }

    }

    public class getArea_present
    {

        byte[] m_img_data = null;
        Point[] m_edge1 = null;
        Point[] m_edge2 = null;
        int m_width = 0;
        int m_height = 0;
        int m_edge1_count = 0;
        int m_edge2_count = 0;
        Graphics mg;
        Form1 mform;

        public getArea_present(int width, int height, Form1 form, Graphics g)
        {
            //初始化存储数据的结构
            if (width > 0 && height > 0)
            {
                m_img_data = new byte[width * height];
                m_edge1 = new Point[width * 2 + height * 2];
                m_edge2 = new Point[width * 2 + height * 2];
                m_width = width;
                m_height = height;
                mg = g;
                mform = form;
            }
        }

        public int getWidth() { return m_width; }
        public int getHeight() { return m_height; }
        public void setPixelColor(int x, int y, byte color)
        {
            m_img_data[y * m_width + x] = color;
        }

        public void setStartPoint(int x, int y)
        {
            int step = 0;
            if (x < 0 || x >= m_width || y < 0 || y > m_height) return;

            for (int i = 0; i < m_width * m_height; ++i)
            {
                if (m_img_data[i] == 2) m_img_data[i] = 0;
            }

            m_edge1[0].X = x; m_edge1[0].Y = y;
            if (m_img_data[y * m_width + x] == 1) return;
            else m_img_data[y * m_width + x] = 2;
            m_edge1_count = 1;
            m_edge2_count = 0;
            while (m_edge1_count > 0 || m_edge2_count > 0)
            {
                Bitmap bimg = (Bitmap)Bitmap.FromFile("1.png");
                for (int i = 0; i < m_edge1_count; ++i)
                {
                    Point p = m_edge1[i];
                    if (p.Y - 1 >= 0 && m_img_data[(p.Y - 1) * m_width + p.X] == 0)
                    {
                        m_img_data[(p.Y - 1) * m_width + p.X] = 2;
                        m_edge2[m_edge2_count].X = (p.X);
                        m_edge2[m_edge2_count++].Y = (p.Y - 1);
                    }
                    if (p.X - 1 >= 0 && m_img_data[(p.Y) * m_width + (p.X - 1)] == 0)
                    {
                        m_img_data[(p.Y) * m_width + (p.X - 1)] = 2;
                        m_edge2[m_edge2_count].X = (p.X - 1);
                        m_edge2[m_edge2_count++].Y = (p.Y);
                    }
                    if (p.X + 1 < m_width && m_img_data[(p.Y) * m_width + (p.X + 1)] == 0)
                    {
                        m_img_data[(p.Y) * m_width + (p.X + 1)] = 2;
                        m_edge2[m_edge2_count].X = (p.X + 1);
                        m_edge2[m_edge2_count++].Y = (p.Y);
                    }
                    if (p.Y + 1 < m_height && m_img_data[(p.Y + 1) * m_width + (p.X)] == 0)
                    {
                        m_img_data[(p.Y + 1) * m_width + (p.X)] = 2;
                        m_edge2[m_edge2_count].X = (p.X);
                        m_edge2[m_edge2_count++].Y = (p.Y + 1);
                    }
                }
                m_edge1_count = 0;

                for (int j = 0; j < m_edge2_count; ++j)
                {
                    Point p = m_edge2[j];
                    if (p.Y - 1 >= 0 && m_img_data[(p.Y - 1) * m_width + p.X] == 0)
                    {
                        m_img_data[(p.Y - 1) * m_width + p.X] = 2;
                        m_edge1[m_edge1_count].X = (p.X);
                        m_edge1[m_edge1_count++].Y = (p.Y - 1);
                    }
                    if (p.X - 1 >= 0 && m_img_data[(p.Y) * m_width + (p.X - 1)] == 0)
                    {
                        m_img_data[(p.Y) * m_width + (p.X - 1)] = 2;
                        m_edge1[m_edge1_count].X = (p.X - 1);
                        m_edge1[m_edge1_count++].Y = (p.Y);
                    }
                    if (p.X + 1 < m_width && m_img_data[(p.Y) * m_width + (p.X + 1)] == 0)
                    {
                        m_img_data[(p.Y) * m_width + (p.X + 1)] = 2;
                        m_edge1[m_edge1_count].X = (p.X + 1);
                        m_edge1[m_edge1_count++].Y = (p.Y);
                    }
                    if (p.Y + 1 < m_height && m_img_data[(p.Y + 1) * m_width + (p.X)] == 0)
                    {
                        m_img_data[(p.Y + 1) * m_width + (p.X)] = 2;
                        m_edge1[m_edge1_count].X = (p.X);
                        m_edge1[m_edge1_count++].Y = (p.Y + 1);
                    }
                }
                m_edge2_count = 0;
                step++;

                if (step >= 3)
                {
                    
                    mform.Refresh();
                    step = 0;
                }
            }

        }

        public byte getPixColor(int x, int y)
        {
            return m_img_data[y * m_width + x];
        }

    }
}
