using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tuexamapi.Models;

namespace tuexamapi.Util
{
    public class FuncUtil
    {
        public static int GetMaxTimeLimit(TimeType timeLimitType, int timeLimit)
        {
            var maxs = 0;
            if (timeLimitType == TimeType.Hour)
                maxs = timeLimit * 60 * 60;
            else if (timeLimitType == TimeType.Minute)
                maxs = timeLimit * 60;
            else if (timeLimitType == TimeType.Second)
                maxs = timeLimit;

            return maxs;
        }
        public static int GetTimeRemaining(TimeType timeLimitType, int timeLimit, DateTime? starton)
        {
            var TimeRemaining = 0;
            var maxs = GetMaxTimeLimit(timeLimitType, timeLimit);

            if (starton.HasValue)
            {
                var seconds = 0;
                var diff = (DateUtil.Now() - starton).Value;
                if (diff.Days > 0)
                    seconds += diff.Days * 24 * 60 * 60;
                if (diff.Hours > 0)
                    seconds += diff.Hours * 60 * 60;
                if (diff.Minutes > 0)
                    seconds += diff.Minutes * 60;
                if (diff.Seconds > 0)
                    seconds += diff.Seconds;
                TimeRemaining = maxs - seconds;
                if (maxs - seconds < 0)
                    TimeRemaining = 0;
            }
            return TimeRemaining;
        }
    }
}
