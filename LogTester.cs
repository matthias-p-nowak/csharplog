using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace csharplog
{
  class LogTester
  {
    private static Timer timer;

    static LogTester()
    {
      Console.WriteLine("LogTester initialized");
      FileLog.Write("initialized");
    }
    static void Main(string[] args)
    {
      Console.WriteLine("Main");
      var sw = new Stopwatch();
      sw.Start();
      for(int i=0;i<10;++i)
      FileLog.Write("hello");
      sw.Stop();
      Console.WriteLine($"got {sw.ElapsedMilliseconds} ms");
      var n = DateTime.Now;
      var n1 = new DateTime(n.Year, n.Month, n.Day, n.Hour, 0, 0);
      n1 = n1.AddHours(1);
      var td = n1 - n;
      Console.WriteLine($"n1= {n1:HH:mm:ss} in {td.TotalMilliseconds} ms");
      timer = new Timer(WriteTimer,null,100,2000);
      Console.ReadKey();
    }

    private static void WriteTimer(object state)
    {
      FileLog.Write($"timer {state} {Thread.CurrentThread.Name}");
      var n = DateTime.Now;
      var nn = new DateTime(n.Year, n.Month, n.Day, n.Hour, n.Minute, 0);
      nn=nn.AddMinutes(1);
      var wd =(nn - n).TotalMilliseconds;
      if (wd <= 0) wd = 60000;
      var w = (int)wd;
      timer.Change(w, 2000);
    }
  }
}
