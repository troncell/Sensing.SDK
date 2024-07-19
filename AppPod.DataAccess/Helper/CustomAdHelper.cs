using Newtonsoft.Json;
using Sensing.SDK.AdsItems;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppPod.DataAccess.Helper
{
    public class CustomAdHelper
    {
        public static CustomAd Parse(string json, int id, string name, int timeSpan)
        {
            try
            {
                var ad = JsonConvert.DeserializeObject<CustomAd>(json);
                ad.Initialize();
                //ad.Id = id;
                //ad.Name = name;
                //ad.TimeSpan = timeSpan;
                return ad;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
