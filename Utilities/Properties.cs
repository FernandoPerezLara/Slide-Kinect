using System.Windows;

namespace Slide_Kinect.Utilities {
    internal static class Properties {
        public static properties kinectProperties; // Kinect properties

        // Properties struct
        public struct properties {
            public enum statusType { connected, connecting, disconnected };
            public enum cameraType { color, infrared, depth };

            public statusType status;
            public cameraType camera;
        };

        // Virtual position properties
        public struct virtualProperties {
            public static double[] widthRange = new double[] { -0.3, 0.4 };
            public static double[] heightRange = new double[] { -0.2, 0.4 };
            public static int screenWidth = (int)SystemParameters.PrimaryScreenWidth;
            public static int screenHeight = (int)SystemParameters.PrimaryScreenHeight;
            public static int X;
            public static int Y;
        };
    }
}
