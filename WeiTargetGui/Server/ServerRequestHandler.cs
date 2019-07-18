using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeiTargetGui.Component;
using WeiTargetGui.EventArgs;

namespace WeiTargetGui.Server
{
    class ServerRequestHandler
    {
        public const string msgPrefix = "WEI";
        public const int _ID = 1;
        public const int _HITCOUNT = 2;

        DateTime lastMessage;
        Semaphore sem;
        public delegate void changeButtonEventHandler(object sender, ChangeButtonEventArgs e);
        public static event changeButtonEventHandler changeButtonEvent;

        public ServerRequestHandler()
        {
            this.sem = new Semaphore(1, 1);
        }

        public void HandleClientRequest(string dataReceived)
        {

            //Message template : 
            // WEI,ID,hitCount
            if (!dataReceived.StartsWith(msgPrefix))
            {
                Console.WriteLine("Invalid message");
                return;
            }
            string[] input = dataReceived.Split(',');
            Target target = Target.Instance;
            int hitCount = Int32.Parse(input[_HITCOUNT]);


            //Thread-safe - critical section , update target's fields
            sem.WaitOne();
            if (hitCount != target.totalHitCount)
            {
                target.lastHitTime = DateTime.Now;
                target.setId(Int32.Parse(input[_ID]));
                target.setCurrentCount(hitCount);
                target.addToTotalCount(hitCount);
                changeButtonEvent(this, new ChangeButtonEventArgs());
            }
            else
            {
                target.lastHitTime = DateTime.Now;
            }
            sem.Release();

            

        }
    }
}
