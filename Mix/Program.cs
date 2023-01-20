using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Mix
{
  class Program
  {
    const string fileName = @"C:\Users\virek.maharaj\Downloads\Recruitment-master\Recruitment-master\VehiclePositions\VehiclePositions_DataFile\VehiclePositions.dat";
    public class Point
    {
      public float longitude { get; set; }
      public float latitude { get; set; }
    }
    static void Main(string[] args)
    {
      //Init for test data
      var testPoints = new List<Point>() {
                new Point(){ latitude = 34.544909f, longitude = -102.100843f},
                new Point(){ latitude = 32.345544f, longitude=-99.123124f},
                new Point(){ latitude =  33.234235f, longitude=-100.214124f},
                new Point(){ latitude = 35.195739f, longitude=-95.348899f},
                new Point(){ latitude = 31.895839f, longitude=-97.789573f},
                new Point(){ latitude =  32.895839f, longitude=-101.789573f},
                new Point(){ latitude = 34.115839f, longitude=-100.225732f},
                new Point(){ latitude = 32.335839f, longitude=-99.992232f},
                new Point(){ latitude =  33.535339f, longitude=-94.792232f},
                new Point(){ latitude = 32.234235f, longitude=-100.222222f}
            };

      //Stop watch for total time
      Stopwatch stopWatchTotal = new Stopwatch();
      stopWatchTotal.Start();
      //Stop watch for data file read
      Stopwatch stopWatchDataRead = new Stopwatch();
      stopWatchDataRead.Start();

      var extractedList = ExtractData();
      //Stop watch for data file read end
      stopWatchDataRead.Stop();
      TimeSpan elapsedTotalDataRead = stopWatchDataRead.Elapsed;
      var totalTimeDataRead = elapsedTotalDataRead.TotalMilliseconds;

      Console.WriteLine("Data file read execution time:" + totalTimeDataRead + "ms");

      //Stop watch for closest position
      Stopwatch stopWatchClosestPos = new Stopwatch();
      stopWatchClosestPos.Start();

      //Find closest postion
      foreach (Point pt in testPoints)
      {
        var index = FindClosest(pt, extractedList);
        Console.WriteLine("Latitude: "+ extractedList[index].latitude +" Longitude:"+ extractedList[index].longitude);
      }

      //Stop watch for closest position end
      stopWatchClosestPos.Stop();
      TimeSpan elapsedTotalClostestPos = stopWatchClosestPos.Elapsed;
      var totalTimeClosest = elapsedTotalClostestPos.TotalMilliseconds;

      Console.WriteLine("Closest position calculation execution time:" + totalTimeClosest + "ms");

      stopWatchTotal.Stop();
      TimeSpan elapsedTotalFullTotal = stopWatchTotal.Elapsed;
      var totalTimeFullTotal = elapsedTotalFullTotal.TotalMilliseconds;

      Console.WriteLine("Total execution time:" + totalTimeFullTotal + "ms");

      Console.ReadLine();
    }

    public static List<Point> ExtractData()
    {
      var listofPts = new List<Point>();
      var pts = new Point();
      // Read the binary file into a memory stream
      byte[] data;
      using (FileStream fileStream = File.Open(fileName, FileMode.Open))
      {
        data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
      }

      // Read the data from the memory stream
      using (MemoryStream stream = new MemoryStream(data))
      using (BinaryReader reader = new BinaryReader(stream))
      {
          while (reader.BaseStream.Position < reader.BaseStream.Length)
          {
            // Read the PositionId field
            int positionId = reader.ReadInt32();

            // Read the VehicleRegistration field
            int length = 0;
            while (reader.ReadByte() != 0)
            {
              length++;
            }
            stream.Seek(-length - 1, SeekOrigin.Current);
            string vehicleRegistration = "";
            char ch;
            while ((ch = reader.ReadChar()) != 0)
              vehicleRegistration +=  ch;

            // Read the Latitude field
            float latitude = reader.ReadSingle();

            // Read the Longitude field
            float longitude = reader.ReadSingle();

            // Read the RecordedTimeUTC field
            UInt64 recordedTimeUTC = reader.ReadUInt64();

            pts = new Point() {latitude= latitude, longitude = longitude };

            listofPts.Add(pts);
          }
      }

      return listofPts;
    }

    public static float FindDistance(Point a, Point b)
    {
      float distance = 0;
      //Pythagoras - using math class is expensive.
      distance = (float)Math.Sqrt(((a.latitude - b.latitude) * (a.latitude - b.latitude) + (a.longitude - b.longitude) * (a.longitude - b.longitude)));

      return distance;
    }

    public static int FindClosest(Point testPoint, List<Point> allPoints)
    {
      float distance = 999999999;
      var index = -1;
      //Degrees around the hemisphere - can be changed dependent on how far you want to search
      var hemiDegree = 1;
      int count = 0;

      Console.WriteLine("For Test point:" + testPoint.latitude +" "+ testPoint.longitude);
      for (int a = 0; a < allPoints.Count; a++)
      {
        if((testPoint.latitude + hemiDegree > allPoints[a].latitude) && (testPoint.latitude - hemiDegree < allPoints[a].latitude) 
          && (testPoint.longitude + hemiDegree > allPoints[a].longitude) && (testPoint.longitude - hemiDegree < allPoints[a].longitude))
        {
          count++;
          var distanceBetweenTwoPoints = FindDistance(testPoint, allPoints[a]);
          if (distanceBetweenTwoPoints < distance)
          {
            distance = distanceBetweenTwoPoints;
            index = a;
          }
        }
      }
      Console.WriteLine("Count:" + count);

      return index;
    }
  }
    
  }

