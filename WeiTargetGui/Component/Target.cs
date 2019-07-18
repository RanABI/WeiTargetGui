using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WeiTargetGui.Component
{
    public class Target
    {
        public int currentHitCount;
        public int totalHitCount;
        public int id;
        public DateTime lastHitTime;
        private static Target instance = new Target();
        private Target()
        {
            lastHitTime = DateTime.Now;
            this.currentHitCount = 0;
            this.totalHitCount = 0;
        }

        public static Target Instance
        {
            get
            {
                return instance;
            }
        }

        public void setId(int id)
        {
            this.id = id;
        }
        public void addToTotalCount(int hitCount)
        {
            this.totalHitCount = hitCount;
        }

        public void setCurrentCount(int hitCount)
        {
            this.currentHitCount += hitCount - this.totalHitCount;
        }
        public void resetCurrentHitCount()
        {
            this.currentHitCount = 0;
        }

    }
}
