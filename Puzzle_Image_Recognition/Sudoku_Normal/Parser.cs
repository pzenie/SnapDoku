using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Reflection;
using System.Text;
using OpenCvSharp;

namespace Puzzle_Image_Recognition.Sudoku_Normal
{
    public static class Parser
    {
        public static List<int> Solve(string filePath)
        {
            Mat sudoku = Cv2.ImDecode(Properties.Resources.test, ImreadModes.GrayScale);
            //Mat sudoku = new Mat(filePath, ImreadModes.GrayScale);
            Mat border = new Mat(sudoku.Size(), MatType.CV_8UC1);
            Mat kernal = new Mat(3, 3, MatType.CV_8UC1, new byte[] { 0, 1, 0, 1, 1, 1, 0, 1, 0 });

            Cv2.GaussianBlur(sudoku, sudoku, new Size(11, 11), 0);
            Cv2.AdaptiveThreshold(sudoku, border, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 5, 2);
            Cv2.BitwiseNot(border, border);
            Cv2.Dilate(sudoku, border, kernal);

            int count = 0;
            int max = -1;

            Point maxPt = new Point();
            for (int y = 0; y < border.Size().Height; y++)
            {
                for (int x = 0; x < border.Size().Width; x++)
                {
                    if (border.At<int>(y, x) >= 128)
                    {

                        int area = Cv2.FloodFill(border, new Point(x, y), new Scalar(0, 0, 64));

                        if (area > max)
                        {
                            maxPt = new Point(x, y);
                            max = area;
                        }
                    }
                }
            }
            Cv2.FloodFill(border, maxPt, new Scalar(255, 255, 255));
            for (int y = 0; y < border.Size().Height; y++)
            {
                for (int x = 0; x < border.Size().Width; x++)
                {
                    if (border.At<int>(y, x) == 64 && x != maxPt.X && y != maxPt.Y)
                    {
                        int area = Cv2.FloodFill(border, new Point(x, y), new Scalar(0, 0, 0));
                    }
                }
            }
            Cv2.Erode(border, border, kernal);

            var lines = Cv2.HoughLines(border, 1, Cv2.PI / 180, 200);
            MergeRelatedLines(lines, sudoku);

            Vector2 topEdge = new Vector2(1000, 1000);
            Vector2 bottomEdge = new Vector2(-1000, -1000);
            Vector2 leftEdge = new Vector2(1000, 1000);
            Vector2 rightEdge = new Vector2(-1000, -1000);
            double topYIntercept = 1000;
            double topXIntercept = 0;
            double bottomYIntercept = 0;
            double bottomXIntercept = 0;
            double leftXIntercept = 1000;
            double leftYIntercept = 0;
            double rightXIntercept = 0;
            double rightYIntercept = 0;
            for (int i = 0; i < lines.Length; i++)
            {

                LineSegmentPolar current = lines[i];

                float p = current.Rho;

                float theta = current.Theta;

                if (p == 0 && theta == -100)
                    continue;
                double xIntercept, yIntercept;
                xIntercept = p / Math.Cos(theta);
                yIntercept = p / (Math.Cos(theta) * Math.Sin(theta));
                if (theta > Cv2.PI * 80 / 180 && theta < Cv2.PI * 100 / 180)
                {
                    if (p < topEdge.X)
                    {
                        topEdge.X = current.Rho;
                        topEdge.Y = current.Theta;
                    }

                    if (p > bottomEdge.Y)
                    {
                        bottomEdge.X = current.Rho;
                        bottomEdge.Y = current.Theta;
                    }
                }
                else if (theta < Cv2.PI * 10 / 180 || theta > Cv2.PI * 170 / 180)
                {
                    if (xIntercept > rightXIntercept)
                    {
                        rightEdge.X = current.Rho;
                        rightEdge.Y = current.Theta;
                        rightXIntercept = xIntercept;
                    }
                    else if (xIntercept <= leftXIntercept)
                    {
                        leftEdge.X = current.Rho;
                        leftEdge.Y = current.Theta;
                        leftXIntercept = xIntercept;
                    }
                }
            }

            Point left1, left2, right1, right2, bottom1, bottom2, top1, top2;

            int height = border.Size().Height;

            int width = border.Size().Width;

            if (leftEdge.Y != 0)
            {
                left1.X = 0; left1.Y = Convert.ToInt32(leftEdge.X / Math.Sin(leftEdge.Y));
                left2.X = width; left2.Y = Convert.ToInt32(-left2.X / Math.Tan(leftEdge.Y) + left1.Y);
            }
            else
            {
                left1.Y = 0; left1.X = Convert.ToInt32(leftEdge.X / Math.Cos(leftEdge.Y));
                left2.Y = height; left2.X = Convert.ToInt32(left1.X - height * Math.Tan(leftEdge.Y));

            }

            if (rightEdge.Y != 0)
            {
                right1.X = 0; right1.Y = Convert.ToInt32(rightEdge.X / Math.Sin(rightEdge.Y));
                right2.X = width; right2.Y = Convert.ToInt32(-right2.X / Math.Tan(rightEdge.Y) + right1.Y);
            }
            else
            {
                right1.Y = 0; right1.X = Convert.ToInt32(rightEdge.X / Math.Cos(rightEdge.Y));
                right2.Y = height; right2.X = Convert.ToInt32(right1.X - height * Math.Tan(rightEdge.Y));

            }

            bottom1.X = 0; bottom1.Y = Convert.ToInt32(bottomEdge.X / Math.Sin(bottomEdge.Y));

            bottom2.X = width; bottom2.Y = Convert.ToInt32(-bottom2.X / Math.Tan(bottomEdge.Y) + bottom1.Y);

            top1.X = 0; top1.Y = Convert.ToInt32(topEdge.X / Math.Sin(topEdge.Y));
            top2.X = width; top2.Y = Convert.ToInt32(-top2.X / Math.Tan(topEdge.Y) + top1.Y);

            double leftA = left2.Y - left1.Y;
            double leftB = left1.X - left2.X;

            double leftC = leftA * left1.X + leftB * left1.Y;

            double rightA = right2.Y - right1.Y;
            double rightB = right1.X - right2.X;

            double rightC = rightA * right1.X + rightB * right1.Y;

            double topA = top2.Y - top1.Y;
            double topB = top1.X - top2.X;

            double topC = topA * top1.X + topB * top1.Y;

            double bottomA = bottom2.Y - bottom1.Y;
            double bottomB = bottom1.X - bottom2.X;

            double bottomC = bottomA * bottom1.X + bottomB * bottom1.Y;

            // Intersection of left and top
            double detTopLeft = leftA * topB - leftB * topA;

            Point ptTopLeft = new Point((topB * leftC - leftB * topC) / detTopLeft, (leftA * topC - topA * leftC) / detTopLeft);

            // Intersection of top and right
            double detTopRight = rightA * topB - rightB * topA;

            Point ptTopRight = new Point((topB * rightC - rightB * topC) / detTopRight, (rightA * topC - topA * rightC) / detTopRight);

            // Intersection of right and bottom
            double detBottomRight = rightA * bottomB - rightB * bottomA;
            Point ptBottomRight = new Point((bottomB * rightC - rightB * bottomC) / detBottomRight, (rightA * bottomC - bottomA * rightC) / detBottomRight);// Intersection of bottom and left
            double detBottomLeft = leftA * bottomB - leftB * bottomA;
            Point ptBottomLeft = new Point((bottomB * leftC - leftB * bottomC) / detBottomLeft, (leftA * bottomC - bottomA * leftC) / detBottomLeft);

            int maxLength = (ptBottomLeft.X - ptBottomRight.X) * (ptBottomLeft.X - ptBottomRight.X) + (ptBottomLeft.Y - ptBottomRight.Y) * (ptBottomLeft.Y - ptBottomRight.Y);
            int temp = (ptTopRight.X - ptBottomRight.X) * (ptTopRight.X - ptBottomRight.X) + (ptTopRight.Y - ptBottomRight.Y) * (ptTopRight.Y - ptBottomRight.Y);

            if (temp > maxLength) maxLength = temp;

            temp = (ptTopRight.X - ptTopLeft.X) * (ptTopRight.X - ptTopLeft.X) + (ptTopRight.Y - ptTopLeft.Y) * (ptTopRight.Y - ptTopLeft.Y);

            if (temp > maxLength) maxLength = temp;

            temp = (ptBottomLeft.X - ptTopLeft.X) * (ptBottomLeft.X - ptTopLeft.X) + (ptBottomLeft.Y - ptTopLeft.Y) * (ptBottomLeft.Y - ptTopLeft.Y);

            if (temp > maxLength) maxLength = temp;

            maxLength = Convert.ToInt32(Math.Sqrt((double)maxLength));

            Point2f[] src = new Point2f[4];
            Point2f[] dst = new Point2f[4];
            src[0] = ptTopLeft; dst[0] = new Point2f(0, 0);
            src[1] = ptTopRight; dst[1] = new Point2f(maxLength - 1, 0);
            src[2] = ptBottomRight; dst[2] = new Point2f(maxLength - 1, maxLength - 1);
            src[3] = ptBottomLeft; dst[3] = new Point2f(0, maxLength - 1);

            Mat undistorted = new Mat(new Size(maxLength, maxLength), MatType.CV_8UC1);
            Cv2.WarpPerspective(sudoku, undistorted, Cv2.GetPerspectiveTransform(src, dst), new Size(maxLength, maxLength));

            Mat undistortedThreshed = undistorted.Clone();
            Cv2.AdaptiveThreshold(undistorted, undistortedThreshed, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, 101, 1);

            
            MemoryStream s = new MemoryStream(Properties.Resources.digits);
            ZipArchive z = new ZipArchive(s);
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                z.ExtractToDirectory(path);
            }
            catch(Exception e)
            { /*Prlly just means the file already exists */ }

            DigitRecognizer dr = new DigitRecognizer();
            dr.Train(path + "/digits");

            List<int> test = new List<int>();
            //Mat final = new Mat(new Size(size, 81), MatType.CV_32FC1);
            //int counter = 0;
            /*foreach(Mat box in boxes)
            {
                for(int i = 0; i < size; i++)
                {
                    final.Set(counter, i, box.At<float>(i));
                }
            }
            Mat results = new Mat();
            int wat = dr.Clasify(final, results);
            for (int i =0; i < 81; i++)
            {
                test.Add(results.At<int>(i));
            }*/

            List<Mat> boxes = new List<Mat>();
            int boxSize = maxLength / 9;
            int size = 16 * 16;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int x = boxSize * i;
                    int y = boxSize * j;
                    Mat numberBox = new Mat(undistortedThreshed, new Rect(x, y, boxSize, boxSize));

                    //if (Cv2.CountNonZero(numberBox) < 200)
                    //{
                        Mat threshold = numberBox.Clone();
                        Cv2.FindContours(threshold, out Point[][] contours2, out HierarchyIndex[] h, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
                        int areaPrv = 0;
                        Rect prevB = new Rect();
                        for(int k = 0; k < contours2.Length; k++)
                        {
                            Rect bnd = Cv2.BoundingRect(contours2[k]);
                            int area = bnd.Height * bnd.Width;
                            if (area > areaPrv)
                            {
                                prevB = bnd;
                                areaPrv = area;
                            }
                        }

                        Mat regionOfInterest = new Mat(numberBox, prevB);
                        Mat resized = new Mat();
                        Cv2.Resize(regionOfInterest, resized, new Size(16, 16), 0, 0, InterpolationFlags.Nearest);
                        resized.ConvertTo(resized, MatType.CV_32FC1, 1 / 255);
                        resized.Reshape(1, 1);
                        Mat final1 = new Mat(new Size(size, 1), MatType.CV_32FC1);
                        for (int k = 0; k < size; k++)
                        {
                            final1.Set(k, resized.At<float>(k));
                        }
                        int resultPLZ = dr.Clasify(final1.Reshape(1, 1), new Mat());
                        test.Add(resultPLZ);
                    //}
                }
            }
            return test;
        }

        private static void MergeRelatedLines(LineSegmentPolar[] lines, Mat img)
        {
            for(int i =0; i < lines.Length; i++)
            {
                LineSegmentPolar line = lines[i];
                if (line.Rho == 0 && line.Theta == -100) continue;

                Point pt1current = new Point();
                Point pt2current = new Point();
                if (line.Theta > Cv2.PI * 45 / 180 && line.Theta < Cv2.PI * 135 / 180)
                {
                    pt1current.X = 0;

                    pt1current.Y = Convert.ToInt32(line.Rho / Math.Sin(line.Theta));

                    pt2current.X = img.Size().Width;
                    pt2current.Y = Convert.ToInt32(-pt2current.X / Math.Tan(line.Theta) + line.Rho / Math.Sin(line.Theta));
                }
                else if(line.Theta != 0)
                {
                    pt1current.Y = 0;

                    pt1current.X = Convert.ToInt32(line.Rho / Math.Cos(line.Theta));

                    pt2current.Y = img.Size().Height;
                    pt2current.X = Convert.ToInt32(-pt2current.Y / Math.Tan(line.Theta) + line.Rho / Math.Cos(line.Theta));
                }
                for(int j = 0; j < lines.Length; j++)
                {
                    LineSegmentPolar nestedLine = lines[j];
                    if (line == nestedLine) continue;
                    if (Math.Abs(nestedLine.Rho - line.Rho) < 20 && Math.Abs(nestedLine.Theta - line.Theta) < Cv2.PI * 10 / 180)
                    {
                        float p = nestedLine.Rho;
                        float theta = nestedLine.Theta;
                        Point pt1 = new Point();
                        Point pt2 = new Point();
                        if (theta > Cv2.PI * 45 / 180 && theta < Cv2.PI * 135 / 180)
                        {
                            pt1.X = 0;
                            pt1.Y = Convert.ToInt32(p / Math.Sin(theta));
                            pt2.X = img.Size().Width;
                            pt2.Y = Convert.ToInt32(-pt2.X / Math.Tan(theta) + p / Math.Sin(theta));
                        }
                        else if (theta != 0)
                        {
                            pt1.Y = 0;
                            pt1.X = Convert.ToInt32(p / Math.Cos(theta));
                            pt2.Y = img.Size().Height;
                            pt2.X = Convert.ToInt32(-pt2.Y / Math.Tan(theta) + p / Math.Cos(theta));
                        }
                        if ((((double)(pt1.X - pt1current.X) * (pt1.X - pt1current.X)) + (pt1.Y - pt1current.Y) * (pt1.Y - pt1current.Y) < 64 * 64)
                            && (((double)(pt2.X - pt2current.X) * (pt2.X - pt2current.X)) + (pt2.Y - pt2current.Y) * (pt2.Y - pt2current.Y) < 64 * 64))
                        {
                            // Merge the two
                            line.Rho = (line.Rho + nestedLine.Rho) / 2;

                            line.Theta = (line.Theta + nestedLine.Theta) / 2;

                            nestedLine.Rho = 0;
                            nestedLine.Theta = -100;
                        }
                    }
                }
            }
        }
    }
}