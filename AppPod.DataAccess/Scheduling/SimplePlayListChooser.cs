using Sensing.SDK.Contract;
using System;

using System.Linq;

namespace AppPod.DataAccess.Scheduling
{
    public class SequencePlayListChooser : PlayListChooserBase
    {
        public SequencePlayListChooser(AdAndAppTimelineScheduleViewModel adSchedulingContent,DateTime timelineStartedTimeOnPC) : base(adSchedulingContent, timelineStartedTimeOnPC)
        {
            //重置为启动时间
            adSchedulingContent.StartTime = adSchedulingContent.StartTime?? new TimeSpan();
        }

        public override AdOrAppPlayItem FindNowPlayItem()
        {
            var nowTimeSpan = DateTime.Now.TimeOfDay;
            //是否当前事件在播放节目单的时间段
            if (nowTimeSpan <= _currentSchedule.StartTime || nowTimeSpan >= _currentSchedule.EndTime) return null;
            //启动时长, 播放任务开始之前需要，不允许播放任何东西.
            //double timeSinceStart = DateTime.Now.TimeOfDay.Subtract(_timelineStartedTime.TimeOfDay).Subtract(_currentSchedule.StartTime.Value).TotalSeconds;
            double timeSinceStart = nowTimeSpan.Subtract(_timelineStartedTime.TimeOfDay).TotalSeconds;

            timeSinceStart = Math.Abs(timeSinceStart);
            //获取节目组的总时长
            int endTime = _currentSchedule.AdAndApps.Max(a => a.ScheduleEndTime);
            //循环播放的时候，需要把之前播放的时间去除.
            timeSinceStart = timeSinceStart % endTime;

            _currentPlayerList = _currentSchedule.AdAndApps.FirstOrDefault(a => a.ScheduleStartTime <= timeSinceStart && timeSinceStart < a.ScheduleEndTime);
            if (_currentPlayerList == null) return null;

            //获取当前节目的总时长，可能也需要循环播放.
            var abSpanTotal = timeSinceStart - _currentPlayerList.ScheduleStartTime;
            var sumTime = _currentPlayerList.Children.Sum(c => c.Duration);
            if (sumTime == 0)
                return null;

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
                    playItem.Unstoppable = _currentPlayerList.Unstoppable;
                    _currentPlayItem = playItem;
                    return playItem;
                }
                continue;
            }
            return null;
        }
    }
}
