using System;
using System.IO;

namespace FittsExercise
{
    class Logger
    {
        private int[] startX;
        private int[] startY;
        private int[] stopX;
        private int[] stopY;
        private uint[] targetW;
        private uint[] targetH;
        private uint[] error;
        private DateTime[] startTime;
        private DateTime[] stopTime;

        public Logger(uint nbrOfTasks)
        {
            this.startTime = new DateTime[nbrOfTasks];
            this.stopTime = new DateTime[nbrOfTasks];
            this.startX = new int[nbrOfTasks];
            this.startY = new int[nbrOfTasks];
            this.stopX = new int[nbrOfTasks];
            this.stopY = new int[nbrOfTasks];
            this.targetW = new uint[nbrOfTasks];
            this.targetH = new uint[nbrOfTasks];
            this.error = new uint[nbrOfTasks];
        }
        public void BeginLogEntry(uint counter, double x, double y, uint w, uint h)
        {
            // Start measurement of time
            this.startTime[counter] = DateTime.Now;
            // Record mouse coordinates, calucation happens in HitButtonClicked
            this.startX[counter] = (int)x;
            this.startY[counter] = (int)y;
            // Record width and height
            this.targetW[counter] = w;
            this.targetH[counter] = h;
        }
        public void EndLogEntry(uint counter, double x, double y, uint taskErrorCount)
        {
            // store current time
            this.stopTime[counter] = DateTime.Now;
            // calculate distance
            this.stopX[counter] = (int)x;
            this.stopY[counter] = (int)y;
            // store number errors counted so far
            this.error[counter] = taskErrorCount;
        }
        public void Save(string fileName, uint experimentId, bool resetMousePos, bool precuing)
        {
            // save document
            using (StreamWriter file = new StreamWriter(fileName))
            {
                // iterate measurements
                for (int i = 0; i < this.startX.Length; i++)
                {
                    // calculate milliseconds of the current measurement
                    int dt = (stopTime[i] - startTime[i]).Seconds * 1000 + (stopTime[i] - startTime[i]).Milliseconds;
                    int dx = stopX[i] - startX[i];
                    int dy = stopY[i] - startY[i];
                    // write configuration data
                    file.Write(experimentId + ";" + resetMousePos + ";" + precuing);
                    // add measurements
                    file.WriteLine(";" + dt + ";" + dx + ";" + dy + ";" + targetW[i] + ";" + targetH[i] + ";" + error[i].ToString());
                }
            }
        }
    }
}