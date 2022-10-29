using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace csharplog
{
  /// <summary>
  /// Simple file logger for debugging, similar to log4j
  /// </summary>
  internal class FileLog
  {
    /// <summary>
    /// timer object for changing log-file every hour
    /// </summary>
    private static Timer changeFileTimer = new Timer(ChangeFile);

    /// <summary>
    /// lock object for synchronization
    /// </summary>
    private static object logLock = new object();

    /// <summary>
    /// the file handle of the current logfile
    /// </summary>
    private static TextWriter logFile = null;

    /// <summary>
    /// TimerCallback method that rolls over the logfile
    /// </summary>
    /// <param name="state">ignored</param>
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
          var sw = new StreamWriter(logFN);
          sw.AutoFlush = true;
          logFile = sw;
        }
      }
      catch(Exception ex)
      {
        logFile = Console.Error;
        Console.Error.WriteLine($"changing logfile got an exception {ex.Message}");
        
      }
      finally
      {
        // adjusting the next invocation
        var n = DateTime.Now;
        var nn = new DateTime(n.Year, n.Month, n.Day, n.Hour, 0, 0);
        nn = nn.AddHours(1);
        var wd = (nn - n).TotalMilliseconds;
        if (wd <= 0) wd = 10;
        var w = (int)wd;
        changeFileTimer.Change(w, 3600000);
      }
    }

    /// <summary>
    /// Lazy initialization 
    /// </summary>
    static FileLog()
    {
      // does the initial setup and enables correct timer
      ChangeFile(null);
      Console.WriteLine("FileLog initialized");
    }

    /// <summary>
    /// Simple writer to the logfile including newline, one message each line
    /// </summary>
    /// <param name="message">the message to print</param>
    internal static void Write(string message)
    {
      // for merging logfile, the logfile should start with the time
      var dts = DateTime.Now.ToString("HH:mm:ss.ffff");
      // getting filename and similar stuff
      var sf = new StackFrame(1, true);
      var fn = Path.GetFileName(sf.GetFileName());
      var m = sf.GetMethod();
      var cn = m.DeclaringType.Name;
      // for following multi threaded programs
      var ct = Thread.CurrentThread;
      lock (logLock)
      {
        logFile.WriteLine($"{dts} {fn}:{sf.GetFileLineNumber()}: [{cn}.{m.Name}]({ct.ManagedThreadId}) {message}");
        Console.WriteLine($"{dts} {fn}:{sf.GetFileLineNumber()}: [{cn}.{m.Name}]({ct.ManagedThreadId}) {message}");
      }
    }
  }
}