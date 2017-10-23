using System;
using System.Threading;

namespace TimerTest
{
    class RepeatableProcess
    {
        private Timer  processTimer;
        private int delay;
        private CancellationTokenSource source;
        private CancellationToken token;
        private Action processToRun;
        private bool canStart = true;
        

        public RepeatableProcess(int delaySeconds,Action process)
        {
            delay = delaySeconds;
            processToRun = process;
        }

        public void Start()
        {
            if (canStart)
            {
                canStart = false;
                source = new CancellationTokenSource();
                token = source.Token;
                processTimer = new Timer(TimedProcess, token, Timeout.Infinite, Timeout.Infinite);
                processTimer.Change(0, Timeout.Infinite);
            }
            
        }

        public void Stop()
        {
            source.Cancel();
        }

        public void TimedProcess(object state)
        {
            
            CancellationToken ct = (CancellationToken)state;
            if (ct.IsCancellationRequested)
            {
                Console.WriteLine("Timer Stopped");
                processTimer.Dispose();
                canStart = true;
            }
            else
            {
                processToRun.Invoke();
                processTimer.Change(delay, Timeout.Infinite);
            }
        }
        
    }
    
    class Program
    {
        private static void DoStuff1()
        {
            Console.Write($"Running Process 1 on thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
            Int64 xx = 0;
            for (int i = 0; i < Int32.MaxValue; i++)
            {
                xx = i * 1;
            }
            Console.WriteLine($" Result one is {xx}");
        }


        static void Main(string[] args)
        {
            var process = new RepeatableProcess(2000, DoStuff1);
            Console.WriteLine($"Hello timer on thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}!");
            process.Start();
            process.Start();
            Console.ReadLine();
            process.Stop();
            Console.ReadLine();
            process.Start();
            Console.ReadLine();
            process.Stop();
            Console.ReadLine();


        }
    }
}
