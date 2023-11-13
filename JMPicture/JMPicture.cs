using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;


namespace JMPicture
{
    public partial class JMPicture: UserControl
    {
        bool imageChanged = false;

        public JMPicture()
        {
            InitializeComponent();
            this.Resize += JMPicture_Resize;

            this.ResizeItems = new ResizeItem[] { 
                new ResizeItem(this,ResizeItem.ItemDirection.LeftTop),//左上
                new ResizeItem(this,ResizeItem.ItemDirection.CenterTop),//上中
                new ResizeItem(this,ResizeItem.ItemDirection.RightTop),//右上
                new ResizeItem(this,ResizeItem.ItemDirection.RigthMiddle),//右中
                new ResizeItem(this,ResizeItem.ItemDirection.RightBottom),//右下
                new ResizeItem(this,ResizeItem.ItemDirection.CenterBottom), //下中
                new ResizeItem(this,ResizeItem.ItemDirection.LeftBottom), //下左
                new ResizeItem(this,ResizeItem.ItemDirection.LeftMiddle) //左中
            };

            this.MouseWheel += JMPicture_MouseWheel;
            this.MouseMove += JMPicture_MouseMove;
            this.MouseDown += JMPicture_MouseDown;
            this.MouseUp += JMPicture_MouseUp;
        }

        #region 图片鼠标事件

        bool mouseInImage = false;
        bool moveImaging = false;
        Point mousePosition;
        void JMPicture_MouseUp(object sender, MouseEventArgs e)
        {
            if (moveImaging)
            {
                moveImaging = false;
                this.Cursor = Cursors.Default;
            }
        }

        void JMPicture_MouseDown(object sender, MouseEventArgs e)
        {
            if (mouseInImage && 
                e.Button == System.Windows.Forms.MouseButtons.Left && 
                this.Tag != "1")
            {
                this.Cursor = Cursors.Cross;
                moveImaging = true;
                mousePosition = e.Location;
            }
        }
        
        void JMPicture_MouseMove(object sender, MouseEventArgs e)
        {
            mouseInImage = this.Rect.Contains(e.Location);
            if (moveImaging)
            {
                this.Rect.X += e.X - mousePosition.X;
                this.Rect.Y += e.Y - mousePosition.Y;

                foreach (var item in this.ResizeItems)
                {
                    item.Reset(this.Rect);
                }
                mousePosition = e.Location;
                this.Refresh();
            }
        }

        void JMPicture_MouseWheel(object sender, MouseEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("wheel:" + e.Delta);
            var s = e.Delta > 0 ? 4 : -4;
            ResizeImage(s, s);
            this.Rect.X -= s / 2;
            this.Rect.Y -= s / 2;
            this.Refresh();
        }

        #endregion


        void JMPicture_Resize(object sender, EventArgs e)
        {
            //this.GetRect(true);
            this.Refresh();
        }

        /// <summary>
        /// 当前图片
        /// </summary>
        protected Bitmap SourceImage { get; set; }

        /// <summary>
        /// 修改后的目标文件
        /// </summary>
        protected Bitmap DstImage { get; set; }

        ///// <summary>
        ///// 当前角度
        ///// </summary>
        //protected float Angle { get; set; }

        /// <summary>
        /// 图片最外层边框
        /// </summary>
        protected Rectangle Rect;

        /// <summary>
        /// 改变大小方块
        /// </summary>
        public ResizeItem[] ResizeItems = null;


        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="filename"></param>
        public void Load(string filename)
        {
            var data = System.IO.File.ReadAllBytes(filename);
            this.Load(data);
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="data"></param>
        public void Load(byte[] data)
        {
            var s = new System.IO.MemoryStream(data);
            SourceImage = new Bitmap(s);
            Load(SourceImage);
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="img"></param>
        public void Load(Bitmap img)
        {
            SourceImage = img;
            Reset();
        }

        /// <summary>
        /// 复位图片
        /// </summary>
        public void Reset()
        {
            DstImage = SourceImage;
            //Angle = 0;
            imageChanged = true;
            //this.Rect = new Rectangle((this.Width - this.DstImage.Width) / 2,(this.Height-this.DstImage.Height)/2,this.DstImage.Width,this.DstImage.Height);
            this.GetRect(true);            
            this.Refresh();
        }

        /// <summary>
        /// 从大小改变方块中获取当前矩型
        /// </summary>
        public void ChangeSizeFromItem()
        {
            var minx = this.Rect.Right;
            var miny = this.Rect.Bottom;
            var maxx = 0;
            var maxy = 0;
            foreach (var item in this.ResizeItems)
            {
                minx = Math.Min(minx, item.Center.X);
                miny = Math.Min(miny, item.Center.Y);

                maxx = Math.Max(maxx, item.Center.X);
                maxy = Math.Max(maxy, item.Center.Y);
            }
            this.Rect.X = minx;
            this.Rect.Y = miny;

            var offw = maxx - minx - this.Rect.Width;
            var offh = maxy - miny - this.Rect.Height;

            //如果图片大小发生改变，则重新生成图片
            if (offw != 0 || offh != 0)
            {
                ResizeImage(offw, offh);
            }

            this.Refresh();
        }

        /// <summary>
        /// 改变图片大小
        /// </summary>
        /// <param name="offw"></param>
        /// <param name="offh"></param>
        private void ResizeImage(int offw, int offh)
        {
            this.Rect.Width += offw;
            this.Rect.Height += offh;

            this.Rect.Width = Math.Max(this.Rect.Width, 10);
            this.Rect.Height = Math.Max(this.Rect.Height, 10);
            //var newimg = new Bitmap(this.Rect.Width, this.Rect.Height);
            //var g = Graphics.FromImage(newimg);
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            //g.DrawImage(this.DstImage, new Rectangle(0, 0, this.Rect.Width, this.Rect.Height), 0, 0, this.DstImage.Width, this.DstImage.Height, GraphicsUnit.Pixel);

            //g.Dispose();
            //this.DstImage = newimg;

            foreach (var item in this.ResizeItems)
            {
                item.Reset(this.Rect);
            }
        }

        /// <summary>
        /// 设为满屏
        /// </summary>
        public void FullImage()
        {
            this.Rect.X = 5;
            this.Rect.Y = 5;
            ResizeImage(this.Width - this.Rect.Width - 15, this.Height - this.Rect.Height - 15);

            this.Refresh();
        }

        /// <summary>
        /// 放大缩小
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ScaleImage(float x, float y)
        {
            var offw = (int)(this.Rect.Width * x - this.Rect.Width);
            var offy = (int)(this.Rect.Height * y - this.Rect.Height);
            this.Rect.X -= offw / 2;
            this.Rect.Y -= offy / 2;

            ResizeImage(offw, offy);
            this.Refresh();
        }

        /// <summary>
        /// 获取当前画图矩形
        /// </summary>
        /// <param name="reset"></param>
        /// <returns></returns>
        private Rectangle GetRect(bool reset=false)
        {
            if (reset && this.DstImage != null)
            {
                var center = new Point(this.Width / 2, this.Height / 2);
                this.Rect = new Rectangle(center.X - this.DstImage.Width / 2, center.Y - this.DstImage.Height / 2, this.DstImage.Width, this.DstImage.Height);
                if (this.Rect.Width > this.Width)
                {
                    //rect.Width = this.Width;
                    this.Rect.X = 0;
                }
                if (this.Rect.Height > this.Height)
                {
                    //rect.Height = this.Height;
                    this.Rect.Y = 0;
                }
                foreach (var item in this.ResizeItems)
                {
                    item.Reset(this.Rect);
                }
            }
            return this.Rect;
        }

        /// <summary>
        /// 重写刷新
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.DstImage != null)
            {
                var rect = GetRect(); 
                e.Graphics.DrawImage(this.DstImage, rect);                  

                //画小方块
                ResizeItem preItem = null;
                foreach (var item in this.ResizeItems)
                {
                    item.Draw(e.Graphics, rect);
                    if (preItem != null)
                    {
                        e.Graphics.DrawLine(item.curPen, preItem.Center, item.Center);
                        if (this.ResizeItems[this.ResizeItems.Length - 1] == item)
                        {
                            e.Graphics.DrawLine(item.curPen, item.Center, this.ResizeItems[0].Center);
                        }
                    }
                    preItem = item;
                }
            }
        }

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="angle"></param>
        public void Rotation(int angle)
        {
            if (angle != 90 && angle != -90)
            {
                throw new Exception("只支持90或-90的角度旋转");
            }
            //Angle += angle;

            //旋转角度
            if (angle != 0)
            {
                //var center = new Point(this.DstImage.Width / 2, this.DstImage.Height / 2);    
                //var p1 = new Point(0, 0);
                //var p2 = new Point(this.SourceImage.Width, p1.Y);
                //var p3 = new Point(p2.X, this.SourceImage.Height);
                //var p4 = new Point(p1.X, p3.Y);

                ////平移坐标移，以中心为坐标系原点
                //p1.Offset(-center.X, -center.Y);
                //p2.Offset(-center.X, -center.Y);
                //p3.Offset(-center.X, -center.Y);
                //p4.Offset(-center.X, -center.Y);

                //var cos = Math.Cos(Angle);
                //var sin = Math.Sin(Angle);
                //p1.X = (int)(cos * p1.X + sin * p1.Y);
                //p1.Y = (int)(p1.Y * cos + p1.X * sin);
                //p2.X = (int)(cos * p2.X + sin * p2.Y);
                //p2.Y = (int)(p2.Y * cos + p2.X * sin);
                //p3.X = (int)(cos * p3.X + sin * p3.Y);
                //p3.Y = (int)(p3.Y * cos + p3.X * sin);
                //p4.X = (int)(cos * p4.X + sin * p4.Y);
                //p4.Y = (int)(p4.Y * cos + p4.X * sin);                

                ////复原坐标系
                //p1.Offset(center);
                //p2.Offset(center);
                //p3.Offset(center);
                //p4.Offset(center);

                //var minx = Math.Min(Math.Min(Math.Min(p1.X, p2.X), p3.X), p4.X);
                //var maxx = Math.Max(Math.Max(Math.Max(p1.X, p2.X), p3.X), p4.X);
                //var miny = Math.Min(Math.Min(Math.Min(p1.Y, p2.Y), p3.Y), p4.Y);
                //var maxy = Math.Max(Math.Max(Math.Max(p1.Y, p2.Y), p3.Y), p4.Y);

                //var w = maxx - minx;
                //var h = maxy - miny;

                var newimg = new Bitmap(this.DstImage.Height, this.DstImage.Width);
                var g = Graphics.FromImage(newimg);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

                var center = new Point(newimg.Width / 2, newimg.Height / 2);    
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(angle);//Angle是要旋转的角度
                g.TranslateTransform(-center.X, -center.Y);

                //g.DrawImage(this.SourceImage, new Rectangle(0, 0, this.SourceImage.Width, this.SourceImage.Height), 0, 0, this.SourceImage.Width, this.SourceImage.Height, GraphicsUnit.Pixel);
                g.DrawImage(this.DstImage,(newimg.Width - this.DstImage.Width) / 2, (newimg.Height - this.DstImage.Height) / 2);

                g.ResetTransform();
                g.Dispose();

                this.DstImage = newimg;

                imageChanged = true;
                //this.GetRect(true);

                var rectw = this.Rect.Height;
                var recth = this.Rect.Width;
                this.Rect.X -= (rectw - this.Rect.Width) / 2;
                this.Rect.Y -= (recth - this.Rect.Height) / 2;
                ResizeImage(rectw - this.Rect.Width, recth - this.Rect.Height);//重置大小
            }

            this.Refresh();
        }
    }

    /// <summary>
    /// 大小改变方块
    /// </summary>
    public class ResizeItem
    {
        const int Size = 10;
        public Pen curPen = new Pen(Brushes.Blue);
        Rectangle rect = new Rectangle(0, 0, Size, Size);
        JMPicture pictureControl = null;
        bool mouseInThis = false;
        bool mouseDown = false;
        Point mousePosition;
        Cursor[] cursors = new Cursor[] { Cursors.SizeNWSE, Cursors.SizeNS, Cursors.SizeNESW, Cursors.SizeWE, Cursors.SizeNWSE, Cursors.SizeNS, Cursors.SizeNESW, Cursors.SizeWE };



        public enum ItemDirection
        { 
            LeftTop= 0,
            CenterTop = 1,
            RightTop=2,
            RigthMiddle = 3,
            RightBottom=4,
            CenterBottom=5,
            LeftBottom=6,
            LeftMiddle=7
        }

        public ResizeItem(JMPicture control,ItemDirection dir)
        {
            pictureControl = control;
            Direction = dir;

            control.MouseMove += control_MouseMove;
            control.MouseDown += control_MouseDown;
            control.MouseUp += control_MouseUp;
            control.MouseLeave += control_MouseLeave;
        }

        void control_MouseLeave(object sender, EventArgs e)
        {
            if (mouseDown)
            {
                mouseDown = false;
                //更新当前矩型图
                this.pictureControl.ChangeSizeFromItem();
                //取消当前控件已开始拉伸动作
                this.pictureControl.Tag = "0";
            }
        }

        void control_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                mouseDown = false;
                //更新当前矩型图
                this.pictureControl.ChangeSizeFromItem();
                //取消当前控件已开始拉伸动作
                this.pictureControl.Tag = "0";
            }
        }

        void control_MouseDown(object sender, MouseEventArgs e)
        {
            if (mouseInThis && e.Button == MouseButtons.Left) 
            { 
                mouseDown = true;
                mousePosition = e.Location;
                //标识当前控件已开始拉伸动作
                this.pictureControl.Tag = "1";
            }         
        }

        /// <summary>
        /// 手标移入时改变指针
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void control_MouseMove(object sender, MouseEventArgs e)
        {
            //进入改变指针，出来设为默认
            if (this.rect.Contains(e.Location))
            {
                if (!mouseInThis) MouseOver(); 
            }
            else if (mouseInThis)
            {
                MouseLeave();
            }

            //如果已在方块中按下移动，则表示拖放
            if (mouseDown)
            {
                var offx = e.X - mousePosition.X;
                var offy = e.Y - mousePosition.Y;
                mousePosition = e.Location;

                switch (this.Direction)
                {
                    case ItemDirection.CenterBottom:
                    case ItemDirection.CenterTop:
                        {
                            offx = 0;
                            break;
                        }
                    case ItemDirection.LeftMiddle:
                    case ItemDirection.RigthMiddle:
                        {
                            offy = 0;
                            break;
                        }
                }
                this.Center.Offset(offx, offy);
                if (this.Center.X < Size / 2) this.Center.X = Size / 2;
                if (this.Center.Y < Size / 2) this.Center.Y = Size / 2;

                foreach (var item in this.pictureControl.ResizeItems)
                {
                    if (item == this) continue;
                    switch (this.Direction)
                    {
                        case ItemDirection.CenterBottom:
                            {
                                if (item.Direction == ItemDirection.LeftBottom || item.Direction == ItemDirection.RightBottom)
                                {
                                    item.Center.Y = this.Center.Y;
                                }
                                break;
                            }
                        case ItemDirection.CenterTop:
                            {
                                if (item.Direction == ItemDirection.LeftTop || item.Direction == ItemDirection.RightTop)
                                {
                                    item.Center.Y = this.Center.Y;
                                }
                                break;
                            }
                        case ItemDirection.LeftBottom:
                            {
                                if (item.Direction == ItemDirection.LeftMiddle || item.Direction == ItemDirection.LeftTop)
                                {
                                    item.Center.X = this.Center.X;
                                }
                                else if (item.Direction == ItemDirection.CenterBottom || item.Direction == ItemDirection.RightBottom)
                                {
                                    item.Center.Y = this.Center.Y;
                                }
                                break;
                            }
                        case ItemDirection.LeftMiddle:
                            {
                                if (item.Direction == ItemDirection.LeftTop || item.Direction == ItemDirection.LeftBottom)
                                {
                                    item.Center.X = this.Center.X;
                                }
                                break;
                            }
                        case ItemDirection.LeftTop:
                            {
                                if (item.Direction == ItemDirection.LeftMiddle || item.Direction == ItemDirection.LeftBottom)
                                {
                                    item.Center.X = this.Center.X;
                                }
                                else if (item.Direction == ItemDirection.CenterTop || item.Direction == ItemDirection.RightTop)
                                {
                                    item.Center.Y = this.Center.Y;
                                }
                                break;
                            }
                        case ItemDirection.RightBottom:
                            {
                                if (item.Direction == ItemDirection.CenterBottom || item.Direction == ItemDirection.LeftBottom)
                                {
                                    item.Center.Y = this.Center.Y;
                                }
                                else if (item.Direction == ItemDirection.RigthMiddle || item.Direction == ItemDirection.RightTop)
                                {
                                    item.Center.X = this.Center.X;
                                }
                                break;
                            }
                        case ItemDirection.RightTop:
                            {
                                if (item.Direction == ItemDirection.LeftTop || item.Direction == ItemDirection.CenterTop)
                                {
                                    item.Center.Y = this.Center.Y;
                                }
                                else if (item.Direction == ItemDirection.RigthMiddle || item.Direction == ItemDirection.RightBottom)
                                {
                                    item.Center.X = this.Center.X;
                                }
                                break;
                            }
                        case ItemDirection.RigthMiddle:
                            {
                                if (item.Direction == ItemDirection.RightTop || item.Direction == ItemDirection.RightBottom)
                                {
                                    item.Center.X = this.Center.X;
                                }
                                break;
                            }
                    }                
                }

                this.pictureControl.Refresh();
            }
        }

        private void MouseOver()
        {
            this.pictureControl.Cursor = cursors[this.Direction.GetHashCode()];
            mouseInThis = true;
            //System.Diagnostics.Debug.WriteLine("鼠标进入" + this.Direction);
        }
        private void MouseLeave()
        {
            this.pictureControl.Cursor = Cursors.Default;
            mouseInThis = false;
            //System.Diagnostics.Debug.WriteLine("鼠标移开" + this.Direction + new Random().Next());
        }

        /// <summary>
        /// 方位
        /// </summary>
        public ItemDirection Direction
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 中心坐标
        /// </summary>
        public Point Center;

        /// <summary>
        /// 重置位置
        /// </summary>
        /// <param name="rec"></param>
        public void Reset(Rectangle rec)
        {
            switch (this.Direction)
            {
                case ItemDirection.CenterBottom:
                    {
                        this.Center.X = rec.X + rec.Width / 2;
                        this.Center.Y = rec.Bottom;
                        break;
                    }
                case ItemDirection.CenterTop:
                    {
                        this.Center.X = rec.X + rec.Width / 2;
                        this.Center.Y = rec.Y;
                        break;
                    }
                case ItemDirection.LeftBottom:
                    {
                        this.Center.X = rec.X;
                        this.Center.Y = rec.Bottom;
                        break;
                    }
                case ItemDirection.LeftMiddle:
                    {
                        this.Center.X = rec.X;
                        this.Center.Y = rec.Y + rec.Height / 2;
                        break;
                    }
                case ItemDirection.LeftTop:
                    {
                        this.Center.X = rec.X;
                        this.Center.Y = rec.Y;
                        break;
                    }
                case ItemDirection.RightBottom:
                    {
                        this.Center.X = rec.Right;
                        this.Center.Y = rec.Bottom;
                        break;
                    }
                case ItemDirection.RightTop:
                    {
                        this.Center.X = rec.Right;
                        this.Center.Y = rec.Y;
                        break;
                    }
                case ItemDirection.RigthMiddle:
                    {
                        this.Center.X = rec.Right;
                        this.Center.Y = rec.Y + rec.Height / 2;
                        break;
                    }
            }                
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g, Rectangle rec, ResizeItem preItem = null)
        {
            rect.X = this.Center.X - Size / 2;
            rect.Y = this.Center.Y - Size / 2;
            g.DrawRectangle(curPen, rect);
        }
    }
}
