using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace Slide_Kinect {
    public partial class MainWindow : Window {
        private KinectSensor kinectSensor; // Kinect device
        private MultiSourceFrameReader kinectReader; // Reader for multi source frames
        private properties kinectProperties; // Kinect properties

        // Properties struct
        private struct properties {
            public enum statusType { connected, connecting, disconnected };
            public enum cameraType { color, infrared, depth };

            public statusType status;
            public cameraType camera;
        };

        public MainWindow() {
            InitializeComponent();
        }

        // When the from is loaded
        private void frm_Main_Loaded(object sender, RoutedEventArgs e) {
            kinectProperties.status = properties.statusType.disconnected; // set status to disconnected
            kinectProperties.camera = properties.cameraType.color; // Set camera type to color
        }

        // When the form closes
        private void frm_Main_Closed(object sender, EventArgs e) {
            if (kinectReader != null) {
                kinectReader.Dispose();
            }

            if (kinectSensor != null) {
                kinectSensor.Close(); // Stop the Kinect
            }
        }

        // When button is pressed
        private void btn_Switch_Click(object sender, RoutedEventArgs e) {
            if (kinectProperties.status == properties.statusType.disconnected) {
                interfaceStatus(1); // Connecting status
                kinectStart(); // Start Kinect
            } else {
                kinectStop(); // Stop Kinect
                interfaceStatus(2); // Disconnected status
            }
        }

        // Set camera mode to color
        private void rdb_RGBColor_Checked(object sender, RoutedEventArgs e) {
            kinectProperties.camera = properties.cameraType.color;
        }

        // Set camera mode to infrared
        private void rdb_Infrared_Checked(object sender, RoutedEventArgs e) {
            kinectProperties.camera = properties.cameraType.infrared;
        }

        // Set camera mode to depth
        private void rdb_Depth_Checked(object sender, RoutedEventArgs e) {
            kinectProperties.camera = properties.cameraType.depth;
        }

        // Starts the Kinect
        private void kinectStart() {
            kinectSensor = KinectSensor.GetDefault(); // Gets the default sensor

            if (kinectSensor != null) { // If Kinect exists
                kinectSensor.Open(); // Open the Kinect

                kinectReader = kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Infrared | FrameSourceTypes.Depth | FrameSourceTypes.Body); // Open the multi source frame reader
                kinectReader.MultiSourceFrameArrived += KinectReader_MultiSourceFrameArrived; // When frame is captured the program fires an event
            }
        }

        // Stops the Kinect
        private void kinectStop() {
            kinectSensor.Close(); // Stop the Kinect
        }

        // Multi source frame reader
        private void KinectReader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e) {
            var reference = e.FrameReference.AcquireFrame(); // Gets the current frame

            // If Kinect is working, change to connected status
            if (kinectProperties.status != properties.statusType.connected) {
                interfaceStatus(0);
            }

            // RGB camera
            using (var frame = reference.ColorFrameReference.AcquireFrame()) {
                if (frame != null) {
                    if (kinectProperties.camera == properties.cameraType.color) {
                        img_Video.Source = frame.kinectOutput();
                    }
                }
            }

            // Infrered camera
            using (var frame = reference.InfraredFrameReference.AcquireFrame()) {
                if (frame != null) {
                    if (kinectProperties.camera == properties.cameraType.infrared) {
                        img_Video.Source = frame.kinectOutput();
                    }
                }
            }

            // Depth camera
            using (var frame = reference.DepthFrameReference.AcquireFrame()) {
                if (frame != null) {
                    if (kinectProperties.camera == properties.cameraType.depth) {
                        img_Video.Source = frame.kinectOutput();
                    }
                }
            }

            // Body tracker
            using (var frame = reference.BodyFrameReference.AcquireFrame()) {
                if (frame != null) {
                    cnv_Video.Children.Clear();
                    cnv_Video.bodyPosition(frame, lbl_WorldXRight, lbl_WorldYRight, lbl_WorldZRight, lbl_WorldXLeft, lbl_WorldYLeft, lbl_WorldZLeft, lbl_RelativeXRight, lbl_RelativeYRight, cbx_Skeleton, cbx_NextSlide, cbx_PreviousSlide, cbx_CursorMode);
                }
            }
        }

        // Shows the status of the Kinect
        private void interfaceStatus(int status) {
            switch (status) {
                case 0: // Case connected
                    // Set status
                    kinectProperties.status = properties.statusType.connected;
                    // Metadata section
                    lbl_Status.Foreground = Brushes.Green;
                    lbl_Status.Content = "Connected";
                    lbl_KinectID.Foreground = Brushes.Black;
                    lbl_KinectID.Content = kinectSensor.UniqueKinectId;
                    // World labels
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
                    // Relative labels
                    lbl_RelativeXRight.Foreground = Brushes.Black;
                    lbl_RelativeXRight.Content = "0";
                    lbl_RelativeYRight.Foreground = Brushes.Black;
                    lbl_RelativeYRight.Content = "0";
                    lbl_RelativeXLeft.Foreground = Brushes.Black;
                    lbl_RelativeXLeft.Content = "0";
                    lbl_RelativeYLeft.Foreground = Brushes.Black;
                    lbl_RelativeYLeft.Content = "0";
                    // Image stretch
                    img_Video.Stretch = Stretch.UniformToFill;

                    break;
                case 1: // Case connecting
                    // Set status
                    kinectProperties.status = properties.statusType.connecting;
                    // Button content
                    btn_Switch.Content = "Stop";
                    // Metadata section
                    lbl_Status.Foreground = Brushes.Orange;
                    lbl_Status.Content = "Connecting";

                    break;
                case 2: // Case disconnected
                    // Set status
                    kinectProperties.status = properties.statusType.disconnected;
                    // Button content
                    btn_Switch.Content = "Start";
                    // Metadata section
                    lbl_Status.Foreground = Brushes.Red;
                    lbl_Status.Content = "Disconnected";
                    lbl_KinectID.Foreground = Brushes.Red;
                    lbl_KinectID.Content = "Not provided";
                    // World labels
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
                    // Relative labels
                    lbl_RelativeXRight.Foreground = Brushes.Red;
                    lbl_RelativeXRight.Content = "NULL";
                    lbl_RelativeYRight.Foreground = Brushes.Red;
                    lbl_RelativeYRight.Content = "NULL";
                    lbl_RelativeXLeft.Foreground = Brushes.Red;
                    lbl_RelativeXLeft.Content = "NULL";
                    lbl_RelativeYLeft.Foreground = Brushes.Red;
                    lbl_RelativeYLeft.Content = "NULL";
                    // Image stretch
                    img_Video.Stretch = Stretch.None;
                    img_Video.Source = new BitmapImage(new Uri(@"/Resources/Camera.png", UriKind.Relative));
                    img_Video.Stretch = Stretch.None;
                    cnv_Video.Children.Clear();

                    break;
            }
        }
    }
}