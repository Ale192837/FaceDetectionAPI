using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Emgu.CV;
using Emgu.Util;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using Emgu.CV.Structure;
using System.Data;
using Emgu.CV.UI;
using System.Windows.Forms;
using System.IO;

/*  Code By Alexandre Cremer Fonseca
    Robot vision: Face detection;
*/

namespace FaceDetectionCS
{
    class FaceDetector
    {
        public delegate void OnDetect();
        public OnDetect OnDetectFace;
        public int deviceFilterIndex = 0;
        public bool showDisplay = true;
        private bool closeDisplayBool = false, DeleteDisplayBool = false;
        private bool stop = false, continueBool = true;
        static readonly CascadeClassifier cascade = new CascadeClassifier("FaceDetectionCS/haarcascade_frontalface_alt.xml");
        Form f;
        PictureBox pic = new PictureBox();
        VideoCaptureDevice device;
        FilterInfoCollection filter;
        bool show = true;

        /// <summary>
        /// Return Cam devices in Hardware setup, set array of devices Filter;
        /// </summary>
        /// <returns>Devices Names</returns>
        public string ShowDevices()
        {
            filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            string devices = "";
            foreach (FilterInfo deviceS in filter)
            {
                devices += deviceS.Name + "\n";
            }
            return devices;
        }

        private void Device_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (!stop)
            {
                Bitmap bitmap = eventArgs.Frame.Clone() as Bitmap;
                ImageViewer imageDisplay = new ImageViewer();
                Image<Bgr, byte> grayImage = bitmap.ToImage<Bgr, byte>();
                Rectangle[] rectangles = cascade.DetectMultiScale(grayImage, 1.2, 1);

                //CallBack
                if (rectangles.Length > 0)
                {
                    if (OnDetectFace != null) OnDetectFace();
                }

                if (showDisplay)
                {
                    foreach (Rectangle rectangle in rectangles)
                    {
                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            using (Pen pen = new Pen(Color.Green, 1))
                            {
                                graphics.DrawRectangle(pen, rectangle);
                            }
                        }
                    }
                    if (closeDisplayBool)
                    {
                        f.Hide();
                    }
                    if (DeleteDisplayBool) f.Close();
                    pic.Image = bitmap;
                    f.Refresh();
                    if (show)
                    {
                        f.Controls.Add(pic);
                        f.Size = new Size(1000, 2000);
                        f.Show();
                        show = false;
                    }
                }
            }
        }

        /// <summary>
        /// Start cam device and face detection;
        /// </summary>
        public void Start()
        {
            if (showDisplay) show = true;
            f = new Form();
            pic.Size = new Size(1000, 1200);
            ShowDevices();
            device = new VideoCaptureDevice(filter[deviceFilterIndex].MonikerString);
            device.NewFrame += Device_NewFrame;
            device.Start();
        }

        public void CloseDisplay() => closeDisplayBool = true;
        public void OpenDisplay()
        {
            closeDisplayBool = false;
            show = true;
        }

        public void Stop()
        {
            closeDisplayBool = true;
            device.NewFrame -= Device_NewFrame;
            device.Stop();
        }
    }
}
