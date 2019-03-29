using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace RealtimeOCR
{
    public partial class Home : Form
    {
        private VideoCapture capture;
        private Image<Bgr, Byte> _imageInput;
        private Image<Gray, Byte> _imageOutput;
        private Thread streamVideo;
        private Thread threadOcr;

        public Home()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            capture = new VideoCapture();
            streamVideo = new Thread(StreamVideo);
            streamVideo.IsBackground = true;
            streamVideo.Start();
            threadOcr = new Thread(OCR);
            threadOcr.IsBackground = true;
            threadOcr.Start();
        }

        private void StreamVideo()
        {
            while (true)
            {
                _imageInput = capture.QueryFrame().ToImage<Bgr, Byte>();
                imageBox1.Image = _imageInput;

                Image<Gray, Byte> _gray = _imageInput.Convert<Gray, byte>();

                _imageOutput = new Image<Gray, Byte>(_gray.Width, _gray.Height, new Gray(0));
                CvInvoke.Threshold(_gray, _imageOutput, 120, 255, Emgu.CV.CvEnum.ThresholdType.Binary);

                imageBox2.Image = _imageOutput;
                Thread.Sleep(10);
            }
        }

        private void OCR()
        {
            while (true)
            {
                using (var ocr = new TesseractEngine("./tessdata", "eng", EngineMode.TesseractAndCube))
                {
                    using (var page = ocr.Process(_imageOutput.Bitmap))
                    {
                        SetTextBox(page.GetText());
                    }
                }
            }
        }

        private void SetTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(SetTextBox), new object[] { value });
                return;
            }
            textBox1.Text = value;
        }
    }
}
