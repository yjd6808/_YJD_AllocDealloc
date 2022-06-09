using System;
using System.Collections.Generic;
using System.Windows;

namespace JJangdoImageUtil
{
    public class DisplayDistanceComparer : IComparer<Rect>
    {
        private Point _position;

        public DisplayDistanceComparer(Point p)
        {
            _position = p;
        }

        private double Distance(Rect rect)
        {
            Point displayCenterLocation = new Point(rect.Location.X + rect.Width / 2, rect.Location.Y + rect.Height / 2);

            return Math.Sqrt(
                Math.Pow(_position.X - displayCenterLocation.X, 2) +
                Math.Pow(_position.Y - displayCenterLocation.Y, 2)
            );
        }

        public int Compare(Rect x, Rect y)
        {
            return Distance(x).CompareTo(Distance(y));
        }
    }
}
