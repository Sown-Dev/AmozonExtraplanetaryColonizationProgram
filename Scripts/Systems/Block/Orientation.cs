using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Systems.Block{
    [Serializable]
    public enum Orientation{
        Up =0,
        Down =1,
        Left =2,
        Right=3
    }

    public static class OrientationFunction{
        public static Vector2 GetVector2(this Orientation orientation){
            switch (orientation){
                case Orientation.Up:
                    return Vector2.up;
                case Orientation.Down:
                    return Vector2.down;
                case Orientation.Left:
                    return Vector2.left;
                case Orientation.Right:
                    return Vector2.right;    
                default:
                    return Vector2.zero;
            }
        }
        public static Vector3 GetVector3(this Orientation orientation){
            switch (orientation){
                case Orientation.Up:
                    return Vector2.up;
                case Orientation.Down:
                    return Vector2.down;
                case Orientation.Left:
                    return Vector2.left;
                case Orientation.Right:
                    return Vector2.right;
                default:
                    return Vector2.zero;
            }
        }
        public static Vector2Int GetVectorInt(this Orientation orientation){
            switch (orientation){
                case Orientation.Up:
                    return Vector2Int.up;
                case Orientation.Down:
                    return Vector2Int.down;
                case Orientation.Left:
                    return Vector2Int.left;
                case Orientation.Right:
                    return Vector2Int.right;
                default:
                    return Vector2Int.zero;
            }
        }

        public static Orientation GetOrientation(this Vector2 vector){
            if (vector == Vector2.up){
                return Orientation.Up;
            }

            if (vector == Vector2.down){
                return Orientation.Down;
            }

            if (vector == Vector2.left){
                return Orientation.Left;
            }

            if (vector == Vector2.right){
                return Orientation.Right;
            }

            return Orientation.Up;
        }

        public static float GetAngle(this Orientation orientation){
            switch (orientation){
                case Orientation.Up:
                    return 0;
                case Orientation.Down:
                    return 180;
                case Orientation.Left:
                    return 90;
                case Orientation.Right: 
                    return 270;
                default:
                    return 0;
            }
        }

        public static Orientation GetOrientationInt(this int number){
            switch (number){
                case 0:
                    return Orientation.Up;
                case 1:
                    return Orientation.Down;
                case 2:
                    return Orientation.Left;
                case 3:
                    return Orientation.Right;
                default:
                    return Orientation.Up;
            }
        }
        
        public static Orientation GetOpposite(this Orientation orientation){
            switch (orientation){
                case Orientation.Up:
                    return Orientation.Down;
                case Orientation.Down:
                    return Orientation.Up;
                case Orientation.Left:
                    return Orientation.Right;
                case Orientation.Right:
                    return Orientation.Left;
                default:
                    return Orientation.Up;
            }
        }
        public static bool isVertical(this Orientation orientation){
            return orientation == Orientation.Up || orientation == Orientation.Down;
        }
        
        public static Orientation next(this Orientation orientation){
            switch (orientation){
                case Orientation.Up:
                    return Orientation.Left;
                case Orientation.Left:
                    return Orientation.Down;
                case Orientation.Down:
                    return Orientation.Right;
                case Orientation.Right:
                    return Orientation.Up;
                default:
                    return Orientation.Up;
            }
        }
        
        /// <summary>
        /// Rotates a list of Vector2 points around a given origin based on the provided orientation.
        /// </summary>
        /// <param name="points">The list of Vector2 points to rotate.</param>
        /// <param name="orientation">The orientation to rotate to (in increments of 90 degrees).</param>
        /// <param name="origin">The point around which the rotation occurs.</param>
        /// <returns>The rotated list of Vector2 points.</returns>
        public static IEnumerable<Vector2Int> RotateList(this IEnumerable<Vector2Int> points, Orientation orientation, Vector2Int origin)
        {
            float angle = orientation.GetAngle(); // Get the angle in degrees based on the orientation
            float radians = angle * Mathf.Deg2Rad; // Convert the angle to radians
        
            return points.Select(point => RotatePointAroundOrigin(point, radians, origin));
        }
        
        public static Vector2Int[] RotateArray(this Vector2Int[] points, Orientation orientation, Vector2Int origin)
        {
            float angle = orientation.GetAngle(); // Get the angle in degrees based on the orientation
            float radians = angle * Mathf.Deg2Rad; // Convert the angle to radians
        
            return points.Select(point => RotatePointAroundOrigin(point, radians, origin)).ToArray();
        }

        /// <summary>
        /// Rotates a single Vector2 point around a given origin by the specified angle.
        /// </summary>
        /// <param name="point">The Vector2 point to rotate.</param>
        /// <param name="radians">The angle in radians to rotate by.</param>
        /// <param name="origin">The origin point around which the rotation occurs.</param>
        /// <returns>The rotated Vector2 point.</returns>
        private static Vector2Int RotatePointAroundOrigin(Vector2Int point, float radians, Vector2Int origin)
        {
            // Translate point to origin
            Vector2Int translatedPoint = point - origin;

            // Rotate the point
            float cosTheta = Mathf.Cos(radians);
            float sinTheta = Mathf.Sin(radians);
            float xNew = translatedPoint.x * cosTheta - translatedPoint.y * sinTheta;
            float yNew = translatedPoint.x * sinTheta + translatedPoint.y * cosTheta;

            // Translate the point back to the original position
            Vector2Int rotatedPoint = Vector2Int.RoundToInt( new Vector2(xNew, yNew) + origin);

            return rotatedPoint;
        }

    }
}