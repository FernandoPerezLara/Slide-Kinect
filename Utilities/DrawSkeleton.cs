using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace Slide_Kinect {
    internal static class drawSkeleton {
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