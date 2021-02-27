using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Helper
{
    public class Functions
    {
        
        public static double DistancePointLine(Point3D P1, Point3D P2, Point3D P)
        /// <summary>
        /// Calculates the euclidean distance between a point in space and the line defined by two points.
        /// <param name="P1">The first defining point of the line.</param>
        /// <param name="P2">The second defining point of the line.</param>
        /// <param name="P">The point.</param>
        /// <returns>The distance.</returns>
        /// </summary>
        {
            Vector3D PP1 = (Vector3D)(P1 - P);
            Vector3D P1P2 = (Vector3D)(P2 - P1);

            var d = (Vector3D.CrossProduct(PP1, P1P2)).Length / P1P2.Length;
            return d;
        }
        public static double DistancePointLine(Point P1, Point P2, Point P)
        /// <summary>
        /// Returns the euclidean distance d between a point P in space and the line connecting points P1 and P2.
        /// <param name="P1">The first defining point of the line.</param>
        /// <param name="P2">The second defining point of the line.</param>
        /// <param name="P">The point.</param>
        /// </summary>
        {
            var p1 = new Point3D(P1.X, P1.Y, 0);
            var p2 = new Point3D(P2.X, P2.Y, 0);
            var p = new Point3D(P.X, P.Y, 0);

            return DistancePointLine(p1, p2, p);
        }


        public static double AngleFind(Point P1, Point P2)
        /// <summary>
        /// Given two points P1 and P2 on the cartesian plane, find the angle between the line
        /// that intersects both and the X-axis, from 0 to 360 degrees.
        /// </summary>

        {
            var DY = P2.Y - P1.Y;
            var DX = P2.X - P1.X;

            // If both points are on the same X-line
            if(DX == 0)
            {
                // If P2 is above P1, return 90°
                if (DY > 0)
                    return 90;
                // If P2 is below P1, return 270°
                else if (DY < 0)
                    return 270;
                // elsewise, return -1 (to be treated as an error)
                else
                    return -1;
            }


            var theta = Math.Atan(DY / DX);     /// The arctan's range is only between -90° and +90°. Thus, some
                                                /// extra work is needed to translate it to the [0, 360] range.
            if (DX > 0)
            {
                /// If both changes are positive, the correct result is the arctan itself (first quadrant)
                if (DY >= 0)
                    return theta;
                /// If delta-Y is negative, we do a full counter-clockwise rotation and
                /// find the equivalent point (fourth quadrant)
                else
                    return theta + 360;
            }

            /// For the second and third quadrants, we simply take the simetrically oposite value to the arctan
            /// (i.e. the other angle with the same tangent). Thus, we add 180° to the value we currently have.
            else
                return theta + 180;
        }

        public static bool CheckPointIntersection(Point3D P1, Point3D P2, Point3D P) 
        /// <summary>
        /// Checks whether point P is contained in the line segment between P1 and P2.
        /// </summary>
        {
            Vector3D p1 = (Vector3D)P1;
            Vector3D p2 = (Vector3D)P2;
            Vector3D p = (Vector3D)P;

            var tol = 1e-5;

            var V1 = p2 - p1;
            var V2 = p - p1;

            V1.Normalize();
            V2.Normalize();

            if ((V1 + V2).Length < tol || V1 != V2) return false;

            if ((p - p1).Length < (p2 - p1).Length) return true;
           
            else return false;
        }

        public static bool CheckPointIntersection(Point P1, Point P2, Point P)
        /// <summary>
        /// Checks whether point P is contained in the line segment between P1 and P2.
        /// </summary>
        {
            var p1 = new Point3D(P1.X, P1.Y, 0);
            var p2 = new Point3D(P2.X, P2.Y, 0);
            var p = new Point3D(P.X, P.Y, 0);

            return CheckPointIntersection(p1, p2, p);
        }

        public static Point3D? FindLineIntersection(Point3D P1, Point3D P2, Point3D Q1, Point3D Q2)
        {
            Vector3D V1 = (Vector3D)P1;
            Vector3D V2 = (Vector3D)P2;
            Vector3D U1 = (Vector3D)Q1;
            Vector3D U2 = (Vector3D)Q2;

            var tol = 1e-5;

            var e = V2 - V1;
            var f = U2 - U1;
            var g = U2 - V1;

            var h = Vector3D.CrossProduct(f, g);
            var k = Vector3D.CrossProduct(f, e);

            if (h.Length < tol || k.Length < tol) return null;

            var sign = Math.Sign(Vector3D.DotProduct(h, k));

            var M = V1 + sign * (h.Length / k.Length) * e;

            return (Point3D)M;
        }

        public static Point Rotate(Point Point, Point Center, double Angle, bool IsRadians = false)
        /// <summary>
        /// Rotates a point/vector around a central point by a given angle, positive counter-clockwise and starting
        /// at the X axis. Can work in both degrees and radians, as given by the IsRadians parameter, with the default
        /// being degrees (false).
        /// </summary>
        {
            // Find the length between the two points and the angle between their connecting 
            // line segment and the X axis (effectively forming the polar-coordinates vector).
            double length = (Center - Point).Length;
            double alpha = AngleFind(Center, Point);
            double theta;

            // Finds the final angle relative to the X axis by adding the rotation. Also converts everything
            // to radians in order to use the default Cos() and Sin() functions.
            if (IsRadians == true)
                theta = Angle + alpha * Math.PI / 180;
            else
                theta = (Angle + alpha) * Math.PI / 180;

            // Finds the cosine and sine of the final angle.
            double cos = Math.Cos(theta);
            double sin = Math.Sin(theta);

            // Returns the final coordinates of the point: the rotated vector (same length, angle
            // changed from alpha to theta) plus the translation to the central point of rotation.
            return new Point(Center.X + length * cos, Center.Y + length * sin);
        }
    
        public static double? GetDouble(string String, bool CanBeNegative=true, bool CanBeZero=true)
        {
            var strim = String.Trim();
            
            if (strim == "")
                return 0;

            var result = Convert.ToDouble(strim);

            if (!CanBeNegative && result < 0 || !CanBeZero && result == 0)
                return null;

            return result;
        }

        public static Point Point3DToCanvas(Point3D point, string normaldir)
        {
            var p = new Point();

            switch (normaldir)
            {
                case "X":
                    p = new Point(point.Y, point.Z);
                    break;
                case "Y":
                    p = new Point(point.X, point.Z);
                    break;
                case "Z":
                    p = new Point(point.X, point.Y);
                    break;
            }
            return p;
        }
    }
}
