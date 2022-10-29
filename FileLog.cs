using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace csharplog
{
  internal class FileLog
  {
    private static Timer changeFileTimer = new Timer(ChangeFile);
    private static object logLock = new object();
    private static StreamWriter logFile = null;
    private static void ChangeFile(object state)
    {
      try
      {
        lock (logLock)
        {
          logFile?.Dispose();
          var dnstr = DateTime.Now.ToString("yyyy-MM-dd_HH");
          var proc = Process.GetCurrentProcess();
          var logFN = $"C:\\temp\\debug\\{proc.ProcessName}-{dnstr}-{proc.Id}-log.txt";
          var dir = Path.GetDirectoryName(logFN);
          Directory.CreateDirectory(dir);
          logFile = new StreamWriter(logFN);
          logFile.AutoFlush = true;
        }
      }
      finally
      {
        var n = DateTime.Now;
        var nn = new DateTime(n.Year, n.Month, n.Day, n.Hour, 0, 0);
        nn = nn.AddHours(1);
        var wd = (nn - n).TotalMilliseconds;
        if (wd <= 0) wd = 10;
        var w = (int)wd;
        changeFileTimer.Change(w, 3600000);
      }
    }

    static FileLog()
    {
      ChangeFile(null);
      Console.WriteLine("FileLog initialized");
    }
    internal static void Write(string message)
    {
      var dts = DateTime.Now.ToString("HH:mm:ss.ffff");
      var sf = new StackFrame(1, true);
      var fn = Path.GetFileName(sf.GetFileName());
      var m = sf.GetMethod();
      var cn = m.DeclaringType.Name;
      var ct = Thread.CurrentThread;
      
      lock (logLock)
      {
        logFile.WriteLine($"{dts} {fn}:{sf.GetFileLineNumber()}: [{cn}.{m.Name}]({ct.ManagedThreadId}) {message}");
        Console.WriteLine($"{dts} {fn}:{sf.GetFileLineNumber()}: [{cn}.{m.Name}]({ct.ManagedThreadId}) {message}");
      }
    }
  }
}