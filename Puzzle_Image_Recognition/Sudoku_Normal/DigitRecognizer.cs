using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenCvSharp;
using OpenCvSharp.ML;

namespace Puzzle_Image_Recognition.Sudoku_Normal
{
    class DigitRecognizer
    {
        private KNearest knn;

        public DigitRecognizer()
        {
            knn = KNearest.Create();
        }
        ~DigitRecognizer()
        {
            knn.Dispose();
        }

        public bool Train(string trainPath)
        {
            DirectoryInfo trainDirectory = new DirectoryInfo(trainPath);
            DirectoryInfo[] trainFolders = trainDirectory.GetDirectories();

            if(trainFolders.Length == 0)
            {
                return false;
            }

            int num = 797;
            int size = 16 * 16;
            int counter = 0;
            Mat trainingData = new Mat(new Size(size, num), MatType.CV_32FC1);
            Mat responses = new Mat(new Size(1, num), MatType.CV_32FC1);

            for(int i = 0; i <= 9; i++)
            {
                float number = Convert.ToInt32(trainFolders[i].Name);
                foreach (FileInfo file in trainFolders[i].GetFiles())
                {
                    Mat img = Cv2.ImRead(file.FullName, ImreadModes.Grayscale);
                    Cv2.Threshold(img, img, 200, 255, ThresholdTypes.Otsu);
                    img.ConvertTo(img, MatType.CV_32FC1, 1/255);
                    Cv2.Resize(img, img, new Size(16, 16), 0, 0, InterpolationFlags.Nearest);
                    img.Reshape(1, 1);
                    for(int j = 0; j < size; j++)
                    {
                        trainingData.Set(counter, j, img.At<float>(j));
                    }
                    responses.Set(counter, number);
                    counter++;
                }
            }

            knn.Train(trainingData, SampleTypes.RowSample, responses);
            return true;
        }

        public int Clasify(Mat box, OutputArray resultAsArray)
        {
            return (int)knn.FindNearest(box, 1, resultAsArray);
        }

        public Mat PreprocessImage(Mat img)
        {

           /* int rowTop = -1, rowBottom = -1, colLeft = -1, colRight = -1;

            Mat temp;
            int thresholdBottom = 50;
            int thresholdTop = 50;
            int thresholdLeft = 50;
            int thresholdRight = 50;
            int center = img.Rows / 2;
            for (int i = center; i < img.Rows; i++)
            {
                if (rowBottom == -1)
                {
                    temp = img.Row(i);
                    IplImage stub = temp;
                    if (cvSum(&stub).val[0] < thresholdBottom || i == img.rows - 1)
                        rowBottom = i;

                }

                if (rowTop == -1)
                {
                    temp = img.row(img.rows - i);
                    IplImage stub = temp;
                    if (cvSum(&stub).val[0] < thresholdTop || i == img.rows - 1)
                        rowTop = img.rows - i;

                }

                if (colRight == -1)
                {
                    temp = img.col(i);
                    IplImage stub = temp;
                    if (cvSum(&stub).val[0] < thresholdRight || i == img.cols - 1)
                        colRight = i;

                }

                if (colLeft == -1)
                {
                    temp = img.col(img.cols - i);
                    IplImage stub = temp;
                    if (cvSum(&stub).val[0] < thresholdLeft || i == img.cols - 1)
                        colLeft = img.cols - i;
                }
            }
            Mat newImg = new Mat();

            newImg = newImg.Zeros(img.Rows, img.Cols, MatType.CV_8UC1);

            int startAtX = (newImg.Cols / 2) - (colRight - colLeft) / 2;

            int startAtY = (newImg.Rows / 2) - (rowBottom - rowTop) / 2;

            for (int y = startAtY; y < (newImg.Rows / 2) + (rowBottom - rowTop) / 2; y++)
            {
                uchar* ptr = newImg.ptr<uchar>(y);
                for (int x = startAtX; x < (newImg.Cols / 2) + (colRight - colLeft) / 2; x++)
                {
                    ptr[x] = img.at<uchar>(rowTop + (y - startAtY), colLeft + (x - startAtX));
                }
            }
            Mat cloneImg = new Mat(16, 16, MatType.CV_8UC1);

            Cv2.Resize(newImg, cloneImg, new Size(16, 16));

            // Now fill along the borders
            for (int i = 0; i < cloneImg.Rows; i++)
            {
                Cv2.FloodFill(cloneImg, cvPoint(0, i), cvScalar(0, 0, 0));

                Cv2.FloodFill(cloneImg, cvPoint(cloneImg.cols - 1, i), cvScalar(0, 0, 0));

                Cv2.FloodFill(cloneImg, cvPoint(i, 0), cvScalar(0));
                Cv2.FloodFill(cloneImg, cvPoint(i, cloneImg.rows - 1), cvScalar(0));
            }
            cloneImg = cloneImg.Reshape(1, 1);

            return cloneImg;*/
            return null;
        }
    }
}
