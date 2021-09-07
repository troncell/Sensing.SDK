using Sensing.SDK.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppPod.DataAccess.Scheduling
{
    public class AdOrAppPlayItem
    {
        public long AdOrAppId { get; set; }
        public int PlayDuration { get; set; }
        public int Type { get; set; }
        public string Transition { get; set; }
        public bool Unstoppable { get; set; }
    }
    public abstract class PlayListChooserBase
    {
        public AdAndAppTimelineScheduleViewModel _currentSchedule;
        public DateTime _timelineStartedTime;
        public ProgramItem _currentPlayerList;
        public AdOrAppPlayItem _currentPlayItem;

        public PlayListChooserBase(AdAndAppTimelineScheduleViewModel adSchedulingContent,DateTime timelineStartedTimeOnPC)
        {
            _timelineStartedTime = timelineStartedTimeOnPC;
            _currentSchedule = adSchedulingContent;
        }

        public abstract AdOrAppPlayItem FindNowPlayItem();
    }
}
