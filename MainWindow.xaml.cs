using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace Slide_Kinect {
    public partial class MainWindow : Window {
        private KinectSensor kinectSensor;
        private MultiSourceFrameReader kinectReader;
        private static bool isOpen = false;
        private cameraMode camera = cameraMode.color;
        private bool depurationMode = true;

        public MainWindow() {
            InitializeComponent();
        }

        private enum cameraMode { color, infrared, depth }

        private void frm_Main_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (Keyboard.IsKeyDown(Key.LeftAlt) && (Keyboard.IsKeyDown(Key.D))) {
                depurationMode = !depurationMode;

                if (depurationMode == true) {
                    stb_Depuration.Visibility = Visibility.Visible;
                } else {
                    stb_Depuration.Visibility = Visibility.Hidden;
                }
            }
        }

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

        private void rdb_Depth_Checked(object sender, RoutedEventArgs e) {
            camera = cameraMode.depth;
        }

        private void frm_Main_Closed(object sender, EventArgs e) {
            if (kinectReader != null) {
                kinectReader.Dispose();
            }

            if (kinectSensor != null) {
                kinectSensor.Close();
            }

            interfaceStatus(2);
        }

        private void kinectStart() {
            kinectSensor = KinectSensor.GetDefault();

            if (kinectSensor != null) {
                kinectReader = kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Infrared | FrameSourceTypes.Depth | FrameSourceTypes.Body);
                kinectReader.MultiSourceFrameArrived += KinectReader_MultiSourceFrameArrived; ;

                kinectSensor.Open();
            }

            isOpen = true;
        }

        private void kinectStop() {
            kinectSensor.Close();

            isOpen = false;
        }

        private void sendMessage(string content) {
            txt_Output.Text = content;
        }

        private void KinectReader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e) {
            var reference = e.FrameReference.AcquireFrame();

            interfaceStatus(0);

            using (var frame = reference.ColorFrameReference.AcquireFrame()) {
                if (frame != null) {
                    if (camera == cameraMode.color) {
                        img_Video.Source = frame.kinectOutput();
                    }
                }
            }

            using (var frame = reference.InfraredFrameReference.AcquireFrame()) {
                if (frame != null) {
                    if (camera == cameraMode.infrared) {
                        img_Video.Source = frame.kinectOutput();
                    }
                }
            }

            using (var frame = reference.DepthFrameReference.AcquireFrame()) {
                if (frame != null) {
                    if (camera == cameraMode.depth) {
                        img_Video.Source = frame.kinectOutput();
                    }
                }
            }

            using (var frame = reference.BodyFrameReference.AcquireFrame()) {
                if (frame != null) {
                    cnv_Video.Children.Clear();
                    cnv_Video.bodyPosition(frame, lbl_WorldXRight, lbl_WorldYRight, lbl_WorldZRight, lbl_WorldXLeft, lbl_WorldYLeft, lbl_WorldZLeft, cbx_Skeleton, cbx_NextSlide, cbx_PreviousSlide);
                }
            }
        }

        private void interfaceStatus(int status) {
            switch (status) {
                case 0:
                    sendMessage("Kinnect connected");
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
                    img_Video.Stretch = Stretch.UniformToFill;

                    break;
                case 1:
                    sendMessage("Connecting Kinect");
                    btn_Switch.Content = "Stop";
                    lbl_Status.Foreground = Brushes.Orange;
                    lbl_Status.Content = "Connecting";

                    break;
                case 2:
                    sendMessage("Kinect disconnected");
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
                    img_Video.Stretch = Stretch.None;
                    img_Video.Source = new BitmapImage(new Uri(@"/Resources/Camera.png", UriKind.Relative));
                    img_Video.Stretch = Stretch.None;
                    cnv_Video.Children.Clear();

                    break;
            }
        }
    }

    public static class CameraReader {
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        public static bool changeSlide = false;

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

        public static ImageSource kinectOutput(this DepthFrame frame) {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            byte[] data = new byte[width * height * (PixelFormats.Bgr32.BitsPerPixel + 7) / 8];
            ushort[] frameData = new ushort[width * height];
            ushort minDepth = frame.DepthMinReliableDistance;
            ushort maxDepth = frame.DepthMaxReliableDistance;
            int position = 0;

            frame.CopyFrameDataToArray(frameData);

            for (int i = 0; i < frameData.Length; i++) {
                data[position++] = (byte)(frameData[i] >= minDepth && frameData[i] <= maxDepth ? frameData[i] : 0);
                data[position++] = (byte)(frameData[i] >= minDepth && frameData[i] <= maxDepth ? frameData[i] : 0);
                data[position++] = (byte)(frameData[i] >= minDepth && frameData[i] <= maxDepth ? frameData[i] : 0);

                position++;
            }

            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr32, null, data, width * PixelFormats.Bgr32.BitsPerPixel / 8);
        }

        public static void bodyPosition(this Canvas cnv_Video, BodyFrame frame, Label lbl_WorldXRight, Label lbl_WorldYRight, Label lbl_WorldZRight, Label lbl_WorldXLeft, Label lbl_WorldYLeft, Label lbl_WorldZLeft, CheckBox cbx_Skeleton, CheckBox cbx_NextSlide, CheckBox cbx_PreviousSlide) {
            IList<Body> bodies = new Body[frame.BodyFrameSource.BodyCount];
            IReadOnlyDictionary<JointType, Joint> joints;

            frame.GetAndRefreshBodyData(bodies);

            foreach (Body body in bodies) {
                if (body != null && body.IsTracked) {
                    joints = body.Joints;

                    lbl_WorldXRight.Content = (joints[JointType.HandTipRight].Position.X).ToString("0.00");
                    lbl_WorldYRight.Content = (joints[JointType.HandTipRight].Position.Y).ToString("0.00");
                    lbl_WorldZRight.Content = (joints[JointType.HandTipRight].Position.Z).ToString("0.00");

                    lbl_WorldXLeft.Content = (joints[JointType.HandTipLeft].Position.X).ToString("0.00");
                    lbl_WorldYLeft.Content = (joints[JointType.HandTipLeft].Position.Y).ToString("0.00");
                    lbl_WorldZLeft.Content = (joints[JointType.HandTipLeft].Position.Z).ToString("0.00");

                    readHands(joints, cnv_Video, cbx_NextSlide, cbx_PreviousSlide);

                    if (cbx_Skeleton.IsChecked == true) {
                        foreach (Joint joint in body.Joints.Values) {
                            cnv_Video.drawNode(joint);
                        }

                        cnv_Video.drawLine(body.Joints[JointType.Head], body.Joints[JointType.Neck]);
                        cnv_Video.drawLine(body.Joints[JointType.Neck], body.Joints[JointType.SpineShoulder]);
                        cnv_Video.drawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderLeft]);
                        cnv_Video.drawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderRight]);
                        cnv_Video.drawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.SpineMid]);
                        cnv_Video.drawLine(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft]);
                        cnv_Video.drawLine(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight]);
                        cnv_Video.drawLine(body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft]);
                        cnv_Video.drawLine(body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight]);
                        cnv_Video.drawLine(body.Joints[JointType.WristLeft], body.Joints[JointType.HandLeft]);
                        cnv_Video.drawLine(body.Joints[JointType.WristRight], body.Joints[JointType.HandRight]);
                        cnv_Video.drawLine(body.Joints[JointType.HandLeft], body.Joints[JointType.HandTipLeft]);
                        cnv_Video.drawLine(body.Joints[JointType.HandRight], body.Joints[JointType.HandTipRight]);
                        cnv_Video.drawLine(body.Joints[JointType.HandTipLeft], body.Joints[JointType.ThumbLeft]);
                        cnv_Video.drawLine(body.Joints[JointType.HandTipRight], body.Joints[JointType.ThumbRight]);
                        cnv_Video.drawLine(body.Joints[JointType.SpineMid], body.Joints[JointType.SpineBase]);
                        cnv_Video.drawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipLeft]);
                        cnv_Video.drawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipRight]);
                        cnv_Video.drawLine(body.Joints[JointType.HipLeft], body.Joints[JointType.KneeLeft]);
                        cnv_Video.drawLine(body.Joints[JointType.HipRight], body.Joints[JointType.KneeRight]);
                        cnv_Video.drawLine(body.Joints[JointType.KneeLeft], body.Joints[JointType.AnkleLeft]);
                        cnv_Video.drawLine(body.Joints[JointType.KneeRight], body.Joints[JointType.AnkleRight]);
                        cnv_Video.drawLine(body.Joints[JointType.AnkleLeft], body.Joints[JointType.FootLeft]);
                        cnv_Video.drawLine(body.Joints[JointType.AnkleRight], body.Joints[JointType.FootRight]);
                    }
                }
            }
        }

        public static void readHands(IReadOnlyDictionary<JointType, Joint> joints, Canvas cnv_Video, CheckBox cbx_NextSlide, CheckBox cbx_PreviousSlide) {
            CameraSpacePoint leftHand, leftElbow;
                
            leftHand = joints[JointType.HandLeft].Position;
            leftElbow = joints[JointType.ElbowLeft].Position;

            if ((Math.Abs(leftHand.X - leftElbow.X) <= 0.05) && (leftHand.Y > leftElbow.Y) && (Math.Abs(leftHand.Z - leftElbow.Z) <= 0.1)) {
                shakeHand(joints[JointType.HandRight].Position, joints[JointType.ElbowRight].Position, cbx_NextSlide, cbx_PreviousSlide);
                cnv_Video.Background = Brushes.Green;
                cnv_Video.Opacity = 0.2;
            } else {
                cnv_Video.Background = Brushes.Red;
                cnv_Video.Opacity = 0.2;
            }
        }

        

        public static void shakeHand(CameraSpacePoint rightHand, CameraSpacePoint rightElbow, CheckBox cbx_NextSlide, CheckBox cbx_PreviousSlide) {
            if ((rightElbow.X - rightHand.X) >= 0.2) {
                if (changeSlide == false) {
                    if (cbx_NextSlide.IsChecked == true) {
                        keybd_event((byte)0x27, 0, 0x0001 | 0, 0);
                    }
                    changeSlide = true;
                }
            } else if ((rightElbow.X - rightHand.X) <= -0.2) {
                if (changeSlide == false) {
                    if (cbx_PreviousSlide.IsChecked == true) {
                        keybd_event((byte)0x25, 0, 0x0001 | 0, 0);
                    }
                    changeSlide = true;
                }
            } else {
                if (changeSlide == true) {
                    changeSlide = false;
                }
            }
        }
    }

    public static class drawSkeleton {
        public static void drawNode(this Canvas cnv_Video, Joint joint) {
            joint = joint.scaleTo(cnv_Video.ActualWidth, cnv_Video.ActualHeight);

            Ellipse ellipse = new Ellipse {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush(Colors.LightBlue)
            };

            Canvas.SetLeft(ellipse, joint.Position.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, joint.Position.Y - ellipse.Height / 2);

            cnv_Video.Children.Add(ellipse);
        }

        public static void drawLine(this Canvas canvas, Joint first, Joint second) {
            if (first.TrackingState == TrackingState.NotTracked || second.TrackingState == TrackingState.NotTracked) return;

            first = first.scaleTo(canvas.ActualWidth, canvas.ActualHeight);
            second = second.scaleTo(canvas.ActualWidth, canvas.ActualHeight);

            Line line = new Line {
                X1 = first.Position.X,
                Y1 = first.Position.Y,
                X2 = second.Position.X,
                Y2 = second.Position.Y,
                StrokeThickness = 6,
                Stroke = new SolidColorBrush(Colors.LightBlue)
            };

            canvas.Children.Add(line);
        }

        public static Joint scaleTo(this Joint joint, double width, double height, float skeletonMaxX, float skeletonMaxY) {
            joint.Position = new CameraSpacePoint {
                X = scale(width, skeletonMaxX, joint.Position.X),
                Y = scale(height, skeletonMaxY, -joint.Position.Y),
                Z = joint.Position.Z
            };

            return joint;
        }

        public static Joint scaleTo(this Joint joint, double width, double height) {
            return scaleTo(joint, width, height, 1.0f, 1.0f);
        }

        private static float scale(double maxPixel, double maxSkeleton, float position) {
            float value = (float)((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));

            if (value > maxPixel) {
                return (float)maxPixel;
            }

            if (value < 0) {
                return 0;
            }

            return value;
        }
    }
}