using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;

namespace trackk
{
    public partial class Form1 : Form
    {
        private Capture cap;
        public Form1()
        {
            InitializeComponent();
            cap = new Capture(0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
          using (Image<Bgr, byte> nextFrame = cap.QueryFrame())
            {
                if (nextFrame != null)
                {
                   
                    Image<Gray, byte> grayframe = nextFrame.Convert<Gray, byte>();
                    Image<Bgr, Byte> img = nextFrame;



     //convert to grayscale

     Image<Gray, Byte> gray = img.Convert<Gray, Byte>();

 

     //convert to binary image using the threshold

     gray = gray.ThresholdBinary(new Gray(128), new Gray(128));

 

      // copy pixels from the original image where pixels in 

       // mask image is nonzero

     Image<Bgr, Byte> newimg = img.Copy(gray);



      // display result
                      pictureBox2.Image = newimg.ToBitmap();
                    pictureBox1.Image = nextFrame.ToBitmap();
                    


                    /////////////////////




                    //Image<Gray, Byte> grayscale = nextFrame.Convert<Gray, Byte>();
                    //grayscale = grayscale.Canny(new Gray(0), new Gray(255)).Not(); //invert with Not()
                    //img = nextFrame.And(grayscale.Convert<Bgr, Byte>(), grayscale); //And function in action

                    //pictureBox3.Image = img.ToBitmap();
                
///////////////////////////////

                    Image<Bgr, Byte> ori = nextFrame;
                    Image<Gray, Byte> grayscale = ori.Convert<Gray, Byte>();

                    Image<Gray, Byte> thresh = grayscale.ThresholdToZero(new Gray(128));//(new Gray(50), new Gray(255));


                    StructuringElementEx ex = new StructuringElementEx(8, 8, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
                  
                    thresh._MorphologyEx(ex, CV_MORPH_OP.CV_MOP_OPEN, 1);

                    pictureBox3.Image = thresh.ToBitmap();


                    
                }
            }

        
        }
    }
}
