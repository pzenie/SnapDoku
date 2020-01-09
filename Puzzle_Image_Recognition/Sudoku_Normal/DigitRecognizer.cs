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
                    for (int j = 0; j < size; j++)
                    {
                        trainingData.Set<float>(counter, j, img.At<float>(j));
                    }
                    //Cv2.Resize(trainingData, trainingData, new Size(500, 500));
                    //Cv2.ImShow("test", trainingData);
                    //Cv2.WaitKey();
                    responses.Set<float>(counter, number);
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
    }
}
