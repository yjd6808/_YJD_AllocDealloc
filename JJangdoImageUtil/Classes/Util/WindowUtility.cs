﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace JJangdoImageUtil
{
    public static class WindowUtility
    {
        public static void MoveTo(Window window, Point point, bool allowOver = true)
        {
            double Left = point.X - window.Width / 2;
            double Top = point.Y - window.Height / 2;

            if (!allowOver)
            {
                if (Left < 0)
                    Left = 0;

                if (Top < 0)
                    Top = 0;
            }

            window.Left = Left;
            window.Top = Top;
        }

        public static Point GetCenterPosition(Window window)
        {
            return new Point(
                window.Left + window.Width / 2,
                window.Top + +window.Height / 2);
        }

        public static Rect ClosestDisplayRect(Window window)
        {
            List<Rect> rectList = DisplayUtility.GetDisplayRectList();
            Point windowCenterPosition = WindowUtility.GetCenterPosition(window);

            // 현재 윈도우가 포함된 디스플레이 가져오기
            Rect closestDisplayRect = rectList.Where(x => x.Contains(windowCenterPosition)).FirstOrDefault();

            // 포함 안되있을 경우
            if (closestDisplayRect == default(Rect))
            {
                // 디스플레이중 메인 윈도우와 가장 가까운 디스플레이 가져오기
                rectList.Sort(new DisplayDistanceComparer(windowCenterPosition));
                closestDisplayRect = rectList[0];
            }

            return closestDisplayRect;
        }

        // window를 현재 window에서 가장 가까운 디스플레이의 중앙에 위치하도록 한다.
        public static void MoveToCenter(Window window)
        {
            Size closestDisplayRect = ClosestDisplayRect(window).Size;

            float width = (float)closestDisplayRect.Width;
            float height = (float)closestDisplayRect.Height;

            window.Left = width / 2 - (double.IsNaN(window.Width) ? 0 : window.Width / 2);
            window.Top = height / 2 - (double.IsNaN(window.Height) ? 0 : window.Height / 2);
        }


        // window를 현재 baseWindow의 중앙에 위치하도록 한다.
        public static void MoveToCenterFromBase(Window baseWindow, Window window)
        {
            Point baseCenter = GetCenterPosition(baseWindow);

            window.Left = baseCenter.X - window.Width / 2;
            window.Top = baseCenter.Y - window.Height / 2;
        }

        // baseWindow 윈도우 기준으로 가장 가까운 디스플레이를 찾고
        // adjustWindow 윈도우를 가장 가까운 디스플레이를 벗어나지 않는 선에서 위치를 세팅한다.
        public static void AdjustPosition(Window baseWindow, Window adjustWindow, int padding)
        {
            // 현재 윈도우가 포함된 디스플레이 가져오기
            Rect closestDisplayRect = ClosestDisplayRect(baseWindow);

            double bottom = adjustWindow.Top + adjustWindow.Height;
            double right = adjustWindow.Left + adjustWindow.Width;

            if (adjustWindow.Top <= closestDisplayRect.Top)
                adjustWindow.Top = closestDisplayRect.Top + padding;
            if (bottom >= closestDisplayRect.Bottom)
                adjustWindow.Top = closestDisplayRect.Bottom - (padding + adjustWindow.Height);
            if (adjustWindow.Left <= closestDisplayRect.Left)
                adjustWindow.Left = closestDisplayRect.Left + padding;
            if (right >= closestDisplayRect.Right)
                adjustWindow.Left = closestDisplayRect.Right - (padding + adjustWindow.Width);
        }
    }
}
