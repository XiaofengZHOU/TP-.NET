using System;
using System.Threading;


namespace ClassLibraryPremier
{
    public class ClassPremier
    {
        
        [STAThread]

        public static void ThreadFunction()
        {
            for (int p = 1; p < 1000000; p++)
            {
                int i = 2;
                while ((p % i) != 0 && i < p)
                {
                    i++;
                }
                if (i == p)
                    Console.WriteLine(p.ToString() +'\t'+ Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(50);

            }
        }
    }
}
