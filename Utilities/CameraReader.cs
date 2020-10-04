using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace Slide_Kinect {
    internal static class CameraReader {
        [DllImport("user32.dll")] // Library used to create a virtual mouse
        public static extern void SetCursorPos(int X, int Y); // Virtual mouse

        [DllImport("user32.dll")] // Library used to create a virtual keyboard
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo); // Virtual keyboard

        public static bool changeSlide = false; // Activated when slide is changing
        public static bool cursorMode = false; // Activated when cursor mode is on

        // Virtual position properties
        public struct virtualProperties {
            public static double[] widthRange = new double[] { -0.3, 0.4 };
            public static double[] heightRange = new double[] { -0.2, 0.4 };
            public static int screenWidth = (int)SystemParameters.PrimaryScreenWidth;
            public static int screenHeight = (int)SystemParameters.PrimaryScreenHeight;
            public static int X;
            public static int Y;
        };

        // Returns a color frame
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

        // Returns an infrared frame
        public static ImageSource kinectOutput(this InfraredFrame frame) {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            byte[] data = new byte[width * height * (PixelFormats.Bgr32.BitsPerPixel + 7) / 8];
            ushort[] frameData = new ushort[width * height];
            int position = 0;

            frame.CopyFrameDataToArray(frameData);

            for (int i = 0; i < frameData.Length; i -= -1) {
                data[position++] = (byte)(frameData[i] >> 7);
                data[position++] = (byte)(frameData[i] >> 7);
                data[position++] = (byte)(frameData[i] >> 7);

                position++;
            }

            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr32, null, data, width * PixelFormats.Bgr32.BitsPerPixel / 8);
        }

        // Returns a depth frame
        public static ImageSource kinectOutput(this DepthFrame frame) {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            byte[] data = new byte[width * height * (PixelFormats.Bgr32.BitsPerPixel + 7) / 8];
            ushort[] frameData = new ushort[width * height];
            ushort minDepth = frame.DepthMinReliableDistance;
            ushort maxDepth = frame.DepthMaxReliableDistance;
            int position = 0;

            frame.CopyFrameDataToArray(frameData);

            for (int i = 0; i < frameData.Length; i -= -1) {
                data[position++] = (byte)(frameData[i] >= minDepth && frameData[i] <= maxDepth ? frameData[i] : 0);
                data[position++] = (byte)(frameData[i] >= minDepth && frameData[i] <= maxDepth ? frameData[i] : 0);
                data[position++] = (byte)(frameData[i] >= minDepth && frameData[i] <= maxDepth ? frameData[i] : 0);

                position++;
            }

            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr32, null, data, width * PixelFormats.Bgr32.BitsPerPixel / 8);
        }

        // Gets the position of the skeleton
        public static void bodyPosition(this Canvas cnv_Video, BodyFrame frame, Label lbl_WorldXRight, Label lbl_WorldYRight, Label lbl_WorldZRight, Label lbl_WorldXLeft, Label lbl_WorldYLeft, Label lbl_WorldZLeft, Label lbl_RelativeXRight, Label lbl_RelativeYRight, CheckBox cbx_Skeleton, CheckBox cbx_NextSlide, CheckBox cbx_PreviousSlide, CheckBox cbx_CursorMode) {
            IList<Body> bodies = new Body[frame.BodyFrameSource.BodyCount]; // Create a body class
            IReadOnlyDictionary<JointType, Joint> joints; // Create the joints

            frame.GetAndRefreshBodyData(bodies); // Get and refresh the skeleton

            foreach (Body body in bodies) {
                if (body != null && body.IsTracked) {
                    joints = body.Joints;
                    
                    // Display world position on labels
                    lbl_WorldXRight.Content = (joints[JointType.HandTipRight].Position.X).ToString("0.00");
                    lbl_WorldYRight.Content = (joints[JointType.HandTipRight].Position.Y).ToString("0.00");
                    lbl_WorldZRight.Content = (joints[JointType.HandTipRight].Position.Z).ToString("0.00");

                    // Display world position on labels
                    lbl_WorldXLeft.Content = (joints[JointType.HandTipLeft].Position.X).ToString("0.00");
                    lbl_WorldYLeft.Content = (joints[JointType.HandTipLeft].Position.Y).ToString("0.00");
                    lbl_WorldZLeft.Content = (joints[JointType.HandTipLeft].Position.Z).ToString("0.00");

                    // Display relative position on labels
                    if ((joints[JointType.HandTipRight].Position.X >= virtualProperties.widthRange[0]) && (joints[JointType.HandTipRight].Position.X <= virtualProperties.widthRange[1])) {
                        virtualProperties.X = (int)(virtualProperties.screenWidth * (-virtualProperties.widthRange[0] + joints[JointType.HandTipRight].Position.X) / (virtualProperties.widthRange[1] - virtualProperties.widthRange[0]));
                        lbl_RelativeXRight.Content = (virtualProperties.X).ToString("0");
                    }

                    // Display relative position on labels
                    if ((joints[JointType.HandTipRight].Position.Y >= virtualProperties.heightRange[0]) && (joints[JointType.HandTipRight].Position.Y <= virtualProperties.heightRange[1])) {
                        virtualProperties.Y = virtualProperties.screenHeight - (int)(virtualProperties.screenHeight * (-virtualProperties.heightRange[0] + joints[JointType.HandTipRight].Position.Y) / (virtualProperties.heightRange[1] - virtualProperties.heightRange[0]));
                        lbl_RelativeYRight.Content = (virtualProperties.Y).ToString("0");
                    }

                    // Read hands movement
                    readHands(joints, cnv_Video, cbx_NextSlide, cbx_PreviousSlide, cbx_CursorMode);

                    // Draw the skeleton on the display
                    if (cbx_Skeleton.IsChecked == true) {
                        foreach (Joint joint in body.Joints.Values) {
                            cnv_Video.drawNode(joint); // Draw nodes
                        }

                        // Draw joints
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

        // Read hands movement
        public static void readHands(IReadOnlyDictionary<JointType, Joint> joints, Canvas cnv_Video, CheckBox cbx_NextSlide, CheckBox cbx_PreviousSlide, CheckBox cbx_CursorMode) {
            CameraSpacePoint leftHand, leftElbow, rightHand, rightElbow;

            // Set joints position
            leftHand = joints[JointType.HandLeft].Position;
            leftElbow = joints[JointType.ElbowLeft].Position;
            rightHand = joints[JointType.HandRight].Position;
            rightElbow = joints[JointType.ElbowRight].Position;

            // Check if activation mode is ready (secondary arm on 90 degrees)
            if ((Math.Abs(leftHand.X - leftElbow.X) <= 0.05) && (leftHand.Y > leftElbow.Y) && (Math.Abs(leftHand.Z - leftElbow.Z) <= 0.1)) {
                shakeHand(rightHand, rightElbow, cbx_NextSlide, cbx_PreviousSlide, cbx_CursorMode); // Detect the movement of the main hand
            } else {
                cursorMode = false;
            }
        }

        // Detect movement of the primary hand
        public static void shakeHand(CameraSpacePoint rightHand, CameraSpacePoint rightElbow, CheckBox cbx_NextSlide, CheckBox cbx_PreviousSlide, CheckBox cbx_CursorMode) {
            if ((cursorMode == true) && (cbx_CursorMode.IsChecked == true)) { // If cursor mode is on
                SetCursorPos(virtualProperties.X, virtualProperties.Y); // Set cursor position
            } else {
                if ((rightElbow.X - rightHand.X) >= 0.2) { // Move the right hand to the left
                    if (changeSlide == false) {
                        if (cbx_NextSlide.IsChecked == true) {
                            keybd_event(0x27, 0, 0x0001 | 0, 0); // Right arrow key
                        }
                        changeSlide = true;
                    }
                } else if ((rightElbow.X - rightHand.X) <= -0.2) { // Move the right hand to the right
                    if (changeSlide == false) {
                        if (cbx_PreviousSlide.IsChecked == true) {
                            keybd_event(0x25, 0, 0x0001 | 0, 0); // Left arrow key
                        }
                        changeSlide = true;
                    }
                } else if ((Math.Abs(rightElbow.Z - rightHand.Z) >= 0.3) && (Math.Abs(rightHand.Y - rightElbow.Y) <= 0.1) && (Math.Abs(rightHand.X - rightElbow.X) <= 0.1)) { // Move hand forward
                    if (changeSlide == false) {
                        if (cbx_CursorMode.IsChecked == true) {
                            cursorMode = true;
                        }
                        changeSlide = true;
                    }
                } else { // It is at rest
                    if (changeSlide == true) {
                        changeSlide = false;
                    }
                }
            }
        }
    }
}