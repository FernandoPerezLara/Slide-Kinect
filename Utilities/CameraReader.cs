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
        [DllImport("user32.dll")] // Library used to create a virtual keyboard

        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo); // Virtual keyboard
        public static bool changeSlide = false;
        public static int screenWidth = (int)SystemParameters.PrimaryScreenWidth;
        public static int screenHeight = (int)SystemParameters.PrimaryScreenHeight;
        public static double minVirtualWidth = -0.3;
        public static double maxVirtualWidth = 0.4;
        public static double minVirtualHeight = -0.2;
        public static double maxVirtualHeight = 0.4;

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

            for (int i = 0; i < frameData.Length; i -= -1) {
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

            for (int i = 0; i < frameData.Length; i -= -1) {
                data[position++] = (byte)(frameData[i] >= minDepth && frameData[i] <= maxDepth ? frameData[i] : 0);
                data[position++] = (byte)(frameData[i] >= minDepth && frameData[i] <= maxDepth ? frameData[i] : 0);
                data[position++] = (byte)(frameData[i] >= minDepth && frameData[i] <= maxDepth ? frameData[i] : 0);

                position++;
            }

            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr32, null, data, width * PixelFormats.Bgr32.BitsPerPixel / 8);
        }

        public static void bodyPosition(this Canvas cnv_Video, BodyFrame frame, Label lbl_WorldXRight, Label lbl_WorldYRight, Label lbl_WorldZRight, Label lbl_WorldXLeft, Label lbl_WorldYLeft, Label lbl_WorldZLeft, Label lbl_RelativeXRight, Label lbl_RelativeYRight, CheckBox cbx_Skeleton, CheckBox cbx_NextSlide, CheckBox cbx_PreviousSlide, CheckBox cbx_CursorMode) {
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

                    if ((joints[JointType.HandTipRight].Position.X >= minVirtualWidth) && (joints[JointType.HandTipRight].Position.X <= maxVirtualWidth)) {
                        lbl_RelativeXRight.Content = (screenWidth * (-minVirtualWidth + joints[JointType.HandTipRight].Position.X) / (maxVirtualWidth - minVirtualWidth)).ToString("0");
                    }

                    if ((joints[JointType.HandTipRight].Position.Y >= minVirtualHeight) && (joints[JointType.HandTipRight].Position.Y <= maxVirtualHeight)) {
                        lbl_RelativeYRight.Content = (screenHeight * (-minVirtualHeight + joints[JointType.HandTipRight].Position.Y) / (maxVirtualHeight - minVirtualHeight)).ToString("0");
                    }

                    readHands(joints, cnv_Video, cbx_NextSlide, cbx_PreviousSlide, cbx_CursorMode);

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

        public static void readHands(IReadOnlyDictionary<JointType, Joint> joints, Canvas cnv_Video, CheckBox cbx_NextSlide, CheckBox cbx_PreviousSlide, CheckBox cbx_CursorMode) {
            CameraSpacePoint leftHand, leftElbow;

            leftHand = joints[JointType.HandLeft].Position;
            leftElbow = joints[JointType.ElbowLeft].Position;

            if ((Math.Abs(leftHand.X - leftElbow.X) <= 0.05) && (leftHand.Y > leftElbow.Y) && (Math.Abs(leftHand.Z - leftElbow.Z) <= 0.1)) {
                shakeHand(joints[JointType.HandRight].Position, joints[JointType.ElbowRight].Position, cbx_NextSlide, cbx_PreviousSlide, cbx_CursorMode);
            }
        }

        public static void shakeHand(CameraSpacePoint rightHand, CameraSpacePoint rightElbow, CheckBox cbx_NextSlide, CheckBox cbx_PreviousSlide, CheckBox cbx_CursorMode) {
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
            } else if ((Math.Abs(rightElbow.Z - rightHand.Z) >= 0.3) && (Math.Abs(rightHand.Y - rightElbow.Y) <= 0.1) && (Math.Abs(rightHand.X - rightElbow.X) <= 0.1)) {
                if (changeSlide == false) {
                    if (cbx_CursorMode.IsChecked == true) {
                        // Cursor mode activated
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
}