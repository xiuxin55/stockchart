using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;

namespace WpfApplication1
{
    public class ThreadDoWork
    {
        public static void AsyCall(Action action)
        {
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(
                    e =>
                    {
                        action();
                    }));
        }
    }

  
}
