using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace Slide_Kinect {
    public partial class MainWindow : Window {
        private KinectSensor kinectSensor;
        private MultiSourceFrameReader kinectReader;
        private static bool isOpen = false;
        private cameraMode camera = cameraMode.color;

        public MainWindow() {
            InitializeComponent();
        }

        public enum cameraMode { color, infrared }

        private void btn_Switch_Click(object sender, RoutedEventArgs e) {
            if (isOpen == false) {
                kinectStart();
                interfaceStatus(1);
            } else {
                kinectStop();
                interfaceStatus(2);
            }
        }

        private void rdb_RGBColor_Checked(object sender, RoutedEventArgs e) {
            camera = cameraMode.color;
        }

        private void rdb_Infrared_Checked(object sender, RoutedEventArgs e) {
            camera = cameraMode.infrared;
        }

        private void frm_Main_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (kinectSensor != null) {
                kinectSensor.Close();
                interfaceStatus(2);
            }
        }

        private void kinectStart() {
            kinectSensor = KinectSensor.GetDefault();

            if (kinectSensor != null) {
                kinectReader = kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                kinectReader.MultiSourceFrameArrived += KinectReader_MultiSourceFrameArrived; ;

                kinectSensor.Open();
            }

            isOpen = true;
        }

        private void kinectStop() {
            kinectSensor.Close();

            isOpen = false;
        }

        private void KinectReader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e) {
            var reference = e.FrameReference.AcquireFrame();

            interfaceStatus(0);

            using (var frame = reference.ColorFrameReference.AcquireFrame()) {
                if (frame != null) {
                    if (camera == cameraMode.color) {
                        img_Videos.Source = frame.kinectOutput();
                    }
                }
            }

            using (var frame = reference.InfraredFrameReference.AcquireFrame()) {
                if (frame != null) {
                    if (camera == cameraMode.infrared) {
                        img_Videos.Source = frame.kinectOutput();
                    }
                }
            }

            using (var frame = reference.BodyFrameReference.AcquireFrame()) {
                if (frame != null) {
                    handPosition(frame);
                }
            }
        }

        private void handPosition(BodyFrame frame) {
            IList<Body> bodies = new Body[frame.BodyFrameSource.BodyCount];
            IReadOnlyDictionary<JointType, Joint> joints;
            Dictionary<JointType, Point> joinPoints;
            Joint rightHand, leftHand;

            frame.GetAndRefreshBodyData(bodies);

            foreach (Body body in bodies) {
                if (body != null && body.IsTracked) {
                    joints = body.Joints;
                    joinPoints = new Dictionary<JointType, Point>();

                    rightHand = joints[JointType.HandTipRight];
                    leftHand = joints[JointType.HandTipLeft];

                    lbl_WorldXRight.Content = (rightHand.Position.X).ToString("0.00");
                    lbl_WorldYRight.Content = (rightHand.Position.Y).ToString("0.00");
                    lbl_WorldZRight.Content = (rightHand.Position.Z).ToString("0.00");

                    lbl_WorldXLeft.Content = (leftHand.Position.X).ToString("0.00");
                    lbl_WorldYLeft.Content = (leftHand.Position.Y).ToString("0.00");
                    lbl_WorldZLeft.Content = (leftHand.Position.Z).ToString("0.00");
                }                
            }
        }

        private void interfaceStatus(int status) {
            switch (status) {
                case 0:
                    lbl_Status.Foreground = Brushes.Green;
                    lbl_Status.Content = "Connected";
                    lbl_KinectID.Foreground = Brushes.Black;
                    lbl_KinectID.Content = kinectSensor.UniqueKinectId;
                    lbl_WorldXRight.Foreground = Brushes.Black;
                    lbl_WorldXRight.Content = "0.00";
                    lbl_WorldYRight.Foreground = Brushes.Black;
                    lbl_WorldYRight.Content = "0.00";
                    lbl_WorldZRight.Foreground = Brushes.Black;
                    lbl_WorldZRight.Content = "0.00";
                    lbl_WorldXLeft.Foreground = Brushes.Black;
                    lbl_WorldXLeft.Content = "0.00";
                    lbl_WorldYLeft.Foreground = Brushes.Black;
                    lbl_WorldYLeft.Content = "0.00";
                    lbl_WorldZLeft.Foreground = Brushes.Black;
                    lbl_WorldZLeft.Content = "0.00";
                    lbl_WorldXRight.Foreground = Brushes.Black;
                    lbl_WorldXRight.Content = "0.00";
                    lbl_WorldYRight.Foreground = Brushes.Black;
                    lbl_WorldYRight.Content = "0.00";
                    lbl_WorldZRight.Foreground = Brushes.Black;
                    lbl_WorldZRight.Content = "0.00";
                    lbl_WorldXLeft.Foreground = Brushes.Black;
                    lbl_WorldXLeft.Content = "0.00";
                    lbl_WorldYLeft.Foreground = Brushes.Black;
                    lbl_WorldYLeft.Content = "0.00";
                    lbl_WorldZLeft.Foreground = Brushes.Black;
                    lbl_WorldZLeft.Content = "0.00";
                    lbl_RelativeXRight.Foreground = Brushes.Black;
                    lbl_RelativeXRight.Content = "0.00";
                    lbl_RelativeYRight.Foreground = Brushes.Black;
                    lbl_RelativeYRight.Content = "0.00";
                    lbl_RelativeZRight.Foreground = Brushes.Black;
                    lbl_RelativeZRight.Content = "0.00";
                    lbl_RelativeXLeft.Foreground = Brushes.Black;
                    lbl_RelativeXLeft.Content = "0.00";
                    lbl_RelativeYLeft.Foreground = Brushes.Black;
                    lbl_RelativeYLeft.Content = "0.00";
                    lbl_RelativeZLeft.Foreground = Brushes.Black;
                    lbl_RelativeZLeft.Content = "0.00";
                    img_Videos.Stretch = Stretch.UniformToFill;

                    break;
                case 1:
                    btn_Switch.Content = "Stop";
                    lbl_Status.Foreground = Brushes.Orange;
                    lbl_Status.Content = "Connecting";

                    break;
                case 2:
                    btn_Switch.Content = "Start";
                    lbl_Status.Foreground = Brushes.Red;
                    lbl_Status.Content = "Disconnected";
                    lbl_KinectID.Foreground = Brushes.Red;
                    lbl_KinectID.Content = "Not provided";
                    lbl_WorldXRight.Foreground = Brushes.Red;
                    lbl_WorldXRight.Content = "NULL";
                    lbl_WorldYRight.Foreground = Brushes.Red;
                    lbl_WorldYRight.Content = "NULL";
                    lbl_WorldZRight.Foreground = Brushes.Red;
                    lbl_WorldZRight.Content = "NULL";
                    lbl_WorldXLeft.Foreground = Brushes.Red;
                    lbl_WorldXLeft.Content = "NULL";
                    lbl_WorldYLeft.Foreground = Brushes.Red;
                    lbl_WorldYLeft.Content = "NULL";
                    lbl_WorldZLeft.Foreground = Brushes.Red;
                    lbl_WorldZLeft.Content = "NULL";
                    lbl_RelativeXRight.Foreground = Brushes.Red;
                    lbl_RelativeXRight.Content = "NULL";
                    lbl_RelativeYRight.Foreground = Brushes.Red;
                    lbl_RelativeYRight.Content = "NULL";
                    lbl_RelativeZRight.Foreground = Brushes.Red;
                    lbl_RelativeZRight.Content = "NULL";
                    lbl_RelativeXLeft.Foreground = Brushes.Red;
                    lbl_RelativeXLeft.Content = "NULL";
                    lbl_RelativeYLeft.Foreground = Brushes.Red;
                    lbl_RelativeYLeft.Content = "NULL";
                    lbl_RelativeZLeft.Foreground = Brushes.Red;
                    lbl_RelativeZLeft.Content = "NULL";
                    img_Videos.Stretch = Stretch.None;
                    img_Videos.Source = new BitmapImage(new Uri(@"/Resources/Camera.png", UriKind.Relative));

                    break;
            }
        }
    }

    public static class CameraReader {
        public static ImageSource kinectOutput(this ColorFrame frame) {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            byte[] data = new byte[width * height * ((PixelFormats.Bgr32.BitsPerPixel + 7) / 8)];

            if (frame.RawColorImageFormat == ColorImageFormat.Bgra) {
                frame.CopyRawFrameDataToArray(data);
            } else {
                frame.CopyConvertedFrameDataToArray(data, ColorImageFormat.Bgra);
            }

            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr32, null, data, width * PixelFormats.Bgr32.BitsPerPixel / 8);
        }

        public static ImageSource kinectOutput(this InfraredFrame frame) {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            byte[] data = new byte[width * height * (PixelFormats.Bgr32.BitsPerPixel + 7) / 8];
            ushort[] frameData = new ushort[width * height];
            int position = 0;

            frame.CopyFrameDataToArray(frameData);

            for (int i = 0; i < frameData.Length; i++) {
                data[position++] = (byte)(frameData[i] >> 7);
                data[position++] = (byte)(frameData[i] >> 7);
                data[position++] = (byte)(frameData[i] >> 7);

                position++;
            }

            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr32, null, data, width * PixelFormats.Bgr32.BitsPerPixel / 8);
        }
    }
}
