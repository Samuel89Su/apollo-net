﻿using System;

namespace Com.Ctrip.Framework.Apollo.Core.Schedule
{
    public class ExponentialSchedulePolicy : ISchedulePolicy
    {
        private readonly int delayTimeLowerBound;
        private readonly int delayTimeUpperBound;
        private int lastDelayTime;

        public ExponentialSchedulePolicy(int delayTimeLowerBound, int delayTimeUpperBound)
        {
            this.delayTimeLowerBound = delayTimeLowerBound;
            this.delayTimeUpperBound = delayTimeUpperBound;
        }

        public int Fail()
        {
            int delayTime = lastDelayTime;

            if (delayTime == 0)
            {
                delayTime = delayTimeLowerBound;
            }
            else
            {
                delayTime = Math.Min(lastDelayTime << 1, delayTimeUpperBound);
            }

            lastDelayTime = delayTime;

            return delayTime;
        }

        public void Success()
        {
            lastDelayTime = 0;
        }

    }
}
