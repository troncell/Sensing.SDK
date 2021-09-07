using Sensing.SDK.Contract;
using System;
using System.Linq;

namespace AppPod.DataAccess.Scheduling
{
    public class TimePlaysListChooser : PlayListChooserBase
    {
        public TimePlaysListChooser( AdAndAppTimelineScheduleViewModel adSchedulingContent, DateTime timelineStartedTimeOnPC) : base(adSchedulingContent, timelineStartedTimeOnPC)
        {
            adSchedulingContent.StartTime =  adSchedulingContent.StartTime ?? new TimeSpan();
        }

        public override AdOrAppPlayItem FindNowPlayItem()
        {

            var nowTimeSpan = DateTime.Now.TimeOfDay;
            //是否当前事件在播放节目单的时间段
            if (nowTimeSpan <= _currentSchedule.StartTime || nowTimeSpan >= _currentSchedule.EndTime) return null;

            double timeSinceStart = nowTimeSpan.Subtract(_currentSchedule.StartTime.Value).TotalSeconds;

            _currentPlayerList = _currentSchedule.AdAndApps.FirstOrDefault(a => a.ScheduleStartTime <= timeSinceStart && timeSinceStart < a.ScheduleEndTime);
            if (_currentPlayerList == null) return null;

            var abSpanTotal = timeSinceStart - _currentPlayerList.ScheduleStartTime;
            var sumTime = _currentPlayerList.Children.Sum(c => c.Duration);
            if (sumTime == 0)
                return null;
            //可能在循环播放了,需要时间求余
            var abSpan = abSpanTotal % sumTime;
            var listSum = 0;
            for (int i = 0; i < _currentPlayerList.Children.Count; i++)
            {
                var item = _currentPlayerList.Children[i];
                var itemSpan = item.Duration;
                listSum += itemSpan;
                if (listSum > abSpan)
                {
                    var playItem = new AdOrAppPlayItem();
                    playItem.Type = _currentPlayerList.Type;
                    playItem.AdOrAppId = item.Id;
                    playItem.PlayDuration = itemSpan;
                    playItem.Transition = item.Transition;
                    _currentPlayItem = playItem;
                    return playItem;
                }
                continue;
            }
            return null;
        }
    }
}
