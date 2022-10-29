using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace csharplog
{
  /// <summary>
  /// Testing class
  /// </summary>
  class LogTester
  {
    // firing a message each minute
    private static Timer timer;

    static LogTester()
    {
      // just showing the order of log messages
      Console.WriteLine("LogTester initialized");
      FileLog.Write("initialized");
    }
    static void Main(string[] args)
    {
      Console.WriteLine("Main");
      // testing performance penalty of this detailed logging approach
      var sw = new Stopwatch();
      sw.Start();
      for(int i=0;i<10;++i)
      FileLog.Write("hello");
      sw.Stop();
      Console.WriteLine($"got {sw.ElapsedMilliseconds} ms");
      // starting the time that should adjust itself to whole minutes
      timer = new Timer(WriteTimer,null,100,2000);
      // just let the program run until user enters a key
      Console.WriteLine("press a key to end this program");
      Console.ReadKey();
    }
    /// <summary>
    /// Log a message and adjust the time to fire at next full minute
    /// </summary>
    /// <param name="state">ignored</param>
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
