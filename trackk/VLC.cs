using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge;
using AForge.Video.DirectShow;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;

namespace trackk
{
    public partial class f21 : Form
    {
        string d = "";
        private FilterInfoCollection videoDevices;
        EuclideanColorFiltering filter = new EuclideanColorFiltering();
        Color color = Color.Black;
        GrayscaleBT709 grayscaleFilter = new GrayscaleBT709();
        BlobCounter blobCounter = new BlobCounter();
        int range = 120;
        int _y, _x;
        float m = 0;
        float count = 0,count1=0,count2=0;
        float cnt, cnt1, cnt2;
        float cls1, cls2, cls;
        float max1,max2,max;
        float maxy1, maxy2, maxy;
        float cont, cont1, cont2;
        public f21()
        {
            InitializeComponent();
           
            blobCounter.MinWidth = 2;
            blobCounter.MinHeight = 2;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
            try
            {
                // enumerate video devices
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                // add all devices to combo
                foreach (FilterInfo device in videoDevices)
                {
                    camerasCombo.Items.Add(device.Name);
                }

                camerasCombo.SelectedIndex = 0;
            }
            catch (ApplicationException)
            {
                camerasCombo.Items.Add("No local capture devices");
                videoDevices = null;
            }

            Bitmap b = new Bitmap(320, 240);
           // Rectangle a = (Rectangle)r;
            Pen pen1 = new Pen(Color.FromArgb(160, 255, 160), 3);
            Graphics g2 = Graphics.FromImage(b);
            pen1 = new Pen(Color.FromArgb(255, 0, 0), 3);
            g2.Clear(Color.White);
            g2.DrawLine(pen1, b.Width / 2, 0, b.Width / 2, b.Width);
            g2.DrawLine(pen1, b.Width, b.Height / 2, 0, b.Height / 2); 
            pictureBox1.Image = (System.Drawing.Image)b;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           
        
        }

        private void videoSourcePlayer1_NewFrame(object sender, ref Bitmap image)
        {
            Bitmap objectsImage = null;
            Bitmap mImage = null;
            mImage=(Bitmap)image.Clone();            
           filter.CenterColor = Color.FromArgb(color.ToArgb());
            filter.Radius =(short)range;
           
            objectsImage = image;
            filter.ApplyInPlace(objectsImage);

            BitmapData objectsData = objectsImage.LockBits(new Rectangle(0, 0, image.Width, image.Height),
            ImageLockMode.ReadOnly, image.PixelFormat);
            UnmanagedImage grayImage = grayscaleFilter.Apply(new UnmanagedImage(objectsData));
            objectsImage.UnlockBits(objectsData);

            
            blobCounter.ProcessImage(grayImage);
            Rectangle[] rects = blobCounter.GetObjectRectangles();
           
            if (rects.Length > 0)
            {

                foreach (Rectangle objectRect in rects)
                {
                    Graphics g = Graphics.FromImage(mImage);
                    using (Pen pen = new Pen(Color.FromArgb(160, 255, 160), 5))
                    {
                        g.DrawRectangle(pen, objectRect);
                    }

                    g.Dispose();
                }
            }

            image = mImage;
        }
        public void videoSourcePlayer3_NewFrame(object sender, ref Bitmap image)
        {
            Bitmap objectsImage = null;
      
                
                  // set center colol and radius
                  filter.CenterColor = Color.FromArgb(color.ToArgb());
                  filter.Radius = (short)range;
                  // apply the filter
                  objectsImage = image;
                  filter.ApplyInPlace(image);

            // lock image for further processing
            BitmapData objectsData = objectsImage.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            // grayscaling
            UnmanagedImage grayImage = grayscaleFilter.Apply(new UnmanagedImage(objectsData));

            // unlock image
            objectsImage.UnlockBits(objectsData);

            // locate blobs 
            blobCounter.ProcessImage(grayImage);
            Rectangle[] rects = blobCounter.GetObjectRectangles();
          
            if (rects.Length > 0)
            {
                Rectangle objectRect = rects[0];

                // draw rectangle around detected object
                Graphics g = Graphics.FromImage(image);

                using (Pen pen = new Pen(Color.FromArgb(160, 255, 160), 5))
                {
                    g.DrawRectangle(pen, objectRect);
                }
              g.Dispose();
                int objectX = objectRect.X + objectRect.Width / 2 - image.Width / 2;
                int objectY = image.Height / 2 - (objectRect.Y + objectRect.Height / 2);
                ParameterizedThreadStart t = new ParameterizedThreadStart(p);
                           Thread aa = new Thread(t);
                
               aa.Start(rects[0]);
               
            }
            Graphics g1 = Graphics.FromImage(image);
            Pen pen1 = new Pen(Color.FromArgb(160, 255, 160), 3);
            g1.DrawLine(pen1,image.Width/2,0,image.Width/2,image.Width);
            g1.DrawLine(pen1, image.Width , image.Height / 2, 0, image.Height / 2);
            g1.Dispose();
       }
       
         

      
  
   void p(object r)
   {
       try
       {
          
       Bitmap b = new Bitmap(pictureBox1.Image);
       Rectangle a = (Rectangle)r;
       Pen pen1 = new Pen(Color.FromArgb(160, 255, 160), 3);
       Graphics g2 = Graphics.FromImage(b);
       pen1 = new Pen(color, 3);
       // Brush b5 = null;
       SolidBrush b5 = new SolidBrush(color);
       //   g2.Clear(Color.Black);


       Font f = new Font(Font, FontStyle.Bold);
       

       g2.DrawString("o", f, b5, a.Location);
       g2.Dispose();
       pictureBox1.Image = (System.Drawing.Image)b;
       this.Invoke((MethodInvoker)delegate
           {
               richTextBox1.Text = a.Location.ToString() + "\n" + richTextBox1.Text + "\n";   // this gives the co o

               textBox1.Text = a.Location.X.ToString();// this gives the co ords
               textBox2.Text = a.Location.Y.ToString();

                   if (a.Location.Y - _y <= 3 && a.Location.Y - _y >= -3 && a.Location.X<_x)//right to left & next
                   {

                       richTextBox1.Text = "1" + "\n" + richTextBox1.Text + "\n";
                       //  m = m + 1;
                       count1 = count1 + 1;
                   }
                   else
                   {
                       richTextBox1.Text = "0" + "\n" + richTextBox1.Text + "\n";
                       // m=m+0;
                       count2 = count2 + 1;
                   }
                   float min;
                   count = count1 + count2;
                   if (count > 8)
                   {
                       min = count1 / count;
                       if (min >= 0.5)
                       {

                           SendKeys.Send("n");
                       }

                       count = 0;
                       count1 = 0;
                       count2 = 0;


                   }

                   if (a.Location.Y - _y <= 3 && a.Location.Y - _y >= -3 && a.Location.X > _x)//left to right previous
                   {

                       richTextBox1.Text = "1" + "\n" + richTextBox1.Text + "\n";
                       //  m = m + 1;
                       cont1 = cont1 + 1;
                   }
                   else
                   {
                       richTextBox1.Text = "0" + "\n" + richTextBox1.Text + "\n";
                       // m=m+0;
                       cont2 = cont2 + 1;
                   }
                   float cat;
                   cont = cont1 + cont2;
                   if (cont > 8)
                   {
                       cat = cont1 / cont;
                       if (cat >= 0.5)
                       {

                           SendKeys.Send("p");
                       }

                       cont = 0;
                       cont1 = 0;
                       cont2 = 0;


                   }


                   if (a.Location.Y - _y <= 3 && a.Location.Y - _y >= -3 && a.Location.X - _x <= 3 && a.Location.X - _x >= -3)//pause,stop
                   {

                       richTextBox1.Text = "1" + "\n" + richTextBox1.Text + "\n";
                       //  m = m + 1;
                       cnt1 = cnt1 + 1;
                   }
                   else
                   {
                       richTextBox1.Text = "0" + "\n" + richTextBox1.Text + "\n";
                       // m=m+0;
                       cnt2 = cnt2 + 1;
                   }
                   float min2;
                   cnt = cnt1 + cnt2;
                   if (cnt >20)
                   {
                       min2 = cnt1 / cnt;
                       if (min2 >= 0.5)
                       {

                           SendKeys.Send("{F2}");
                       }

                       cnt = 0;
                       cnt1 = 0;
                       cnt2 = 0;


                   }
                   if (a.Location.X - _x <= 3 && a.Location.X -_x >= -3 && a.Location.Y > _y)//top to bottom & volume reduce
                   {

                       richTextBox1.Text = "1" + "\n" + richTextBox1.Text + "\n";
                       //  m = m + 1;
                       cls1 = cls1 + 1;
                   }
                   else
                   {
                       richTextBox1.Text = "0" + "\n" + richTextBox1.Text + "\n";
                       // m=m+0;
                       cls2 = cls2 + 1;
                   }
                   float close;
                   cls = cls1 + cls2;
                   if (cls > 2)
                   {
                       close = cls1 / cls;
                       if (close >= 0.5)
                       {

                           SendKeys.Send("^{DOWN}");
                       }

                       
                       cls = 0;
                       cls1 = 0;
                       cls2 = 0;
                       

                   }
                   if (a.Location.X - _x <= 3 && a.Location.X - _x >= -3 && a.Location.Y < _y)//bottom to top & volume increase
                   {

                       richTextBox1.Text = "1" + "\n" + richTextBox1.Text + "\n";
                       //  m = m + 1;
                       max1++;
                   }

                   else
                   {
                       richTextBox1.Text = "0" + "\n" + richTextBox1.Text + "\n";
                       // m=m+0;
                       max2++;
                   }
                   float m1;
                   max = max1 + max2;
                   if (max > 2)
                   {
                       m1=max1 / max;
                       if (m1 >= 0.5)
                       {

                           SendKeys.Send("^{UP}");
                       }


                       max1 = 0;
                       max2 = 0;
                       max = 0;


                   }
                   /*if (a.Location.X<_x && a.Location.Y > _y)//bottom to top
                   {

                       richTextBox1.Text = "1" + "\n" + richTextBox1.Text + "\n";
                       //  m = m + 1;
                       maxy1++;
                   }

                   else
                   {
                       richTextBox1.Text = "0" + "\n" + richTextBox1.Text + "\n";
                       // m=m+0;
                       maxy2++;
                   }
                   float mx1;
                   maxy = maxy1 + maxy2;
                   if (maxy > 30)
                   {
                       mx1 = maxy1 / maxy;
                       if (mx1 >= 0.5)
                       {

                           SendKeys.Send("%{F4}");
                       }


                       maxy1 = 0;
                       maxy2 = 0;
                       maxy = 0;


                   }*/
               
               
                   textBox4.Text = count.ToString();
               textBox3.Text =count1.ToString();
               _x = a.Location.X;
               _y = a.Location.Y;
               
           });

       

       }
       catch (Exception faa)
       {
           Thread.CurrentThread.Abort();
       }


       Thread.CurrentThread.Abort();
   }
        
        private void button1_Click(object sender, EventArgs e)
        {

            videoSourcePlayer1.SignalToStop();
            videoSourcePlayer1.WaitForStop();
            videoSourcePlayer2.SignalToStop();
            videoSourcePlayer2.WaitForStop();
            videoSourcePlayer3.SignalToStop();
            videoSourcePlayer3.WaitForStop();
            // videoDevices = null;
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[camerasCombo.SelectedIndex].MonikerString);
            videoSource.DesiredFrameSize = new Size(320, 240);
            videoSource.DesiredFrameRate = 12;

            videoSourcePlayer1.VideoSource = videoSource;
            videoSourcePlayer1.Start();
            videoSourcePlayer2.VideoSource = videoSource;
            videoSourcePlayer2.Start();
            videoSourcePlayer3.VideoSource = videoSource;
            videoSourcePlayer3.Start();
            //groupBox1.Enabled = false;
        }

        private void f21_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            videoSourcePlayer1.SignalToStop();
            videoSourcePlayer1.WaitForStop();
            videoSourcePlayer2.SignalToStop();
            videoSourcePlayer2.WaitForStop();
            videoSourcePlayer3.SignalToStop();
            videoSourcePlayer3.WaitForStop();
            groupBox1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            videoSourcePlayer1.SignalToStop();
            videoSourcePlayer1.WaitForStop();
            videoSourcePlayer2.SignalToStop();
            videoSourcePlayer2.WaitForStop();
            videoSourcePlayer3.SignalToStop();
            videoSourcePlayer3.WaitForStop();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
            colorDialog1.ShowDialog();
            color = colorDialog1.Color;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            range = Convert.ToInt32(numericUpDown1.Value) ;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            blobCounter.MaxWidth = Convert.ToInt32(numericUpDown2.Value);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            blobCounter.MinWidth  = Convert.ToInt32(numericUpDown3.Value);
        }

        private void videoSourcePlayer2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void videoSourcePlayer3_Click(object sender, EventArgs e)
        {

        }

        private void f21_Load(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void textBox1_TextChanged(object sender, EventArgs e)
        {
         
            
 
        }

        public void textBox2_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        
    }
   /* public void SimulateSomeModifiedKeystrokes()
        {
            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, new[] { VirtualKeyCode.VK_ESCAPE });
        }*/
}
