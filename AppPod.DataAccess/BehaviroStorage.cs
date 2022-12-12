using AppPod.DataAccess.Models;
using Sensing.SDK;
using Sensing.SDK.Contract;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AppPod.DataAccess
{
    public interface IBehaviorDataUploader
    {
        void AddBehavoirData(string thingId, string thingName, string category, string action, DateTime collectionTime, DateTime collectEndTime, string softwareName = "", string pageName = "", string previousPage = "", string previousPageArea = "", long productId = 0);

        void AddBehavoirData(string thingId, string thingName, string category, string action, string softwareName = "", string pageName = "", string previousPage = "", string previousPageArea = "", long productId = 0);
        void AddClick(AdsSdkModel ads, string softwareName, string pageName, string previousPage = "", string previousPageArea = "");
        void AddLike(ShowProductInfo productInfo, string softwareName, string pageName, string previousPage = "", string previousPageArea = "");
        void AddClick(ShowProductInfo productInfo, string softwareName, string pageName, string previousPage = "", string previousPageArea = "");
        List<ClickInfo> ReadClickData();
        List<ClickInfo> ReadLikeClickData();
        List<ClickInfo> ReadAllClickData();
        void LogDeviceStatus(int secondInterval);
        void LogDeviceNetworkStatusRecords(int pingSeed, int secondInterval);

        void AddFaceRecord(string face, string softwareName = "", string pageName = "");

    }

    public class BehaviroStorage : IBehaviorDataUploader
    {
        private SensingWebClient sesingWebClient;

        private string DBPath = "Behavior.db";
        private SQLite.SQLiteConnection m_db;
        private object lockobject = new object();
        private string mMac;
        private DateTime mLastUploadTime;

        public BehaviroStorage(SensingWebClient webClient, string mac)
        {
            sesingWebClient = webClient;
            mMac = mac;
            var appPodFolder = SensingDataAccess.FindAppPodRootFolder();
            if (appPodFolder == null)
                appPodFolder = "";
            m_db = new SQLite.SQLiteConnection(Path.Combine(appPodFolder,DBPath));
            m_db.CreateTable<SqlLiteBehaviorRecord>();
            m_db.CreateTable<SqliteDeviceStatus>();
            m_db.CreateTable<SqlLiteFaceRecord>();
            m_db.CreateTable<SqliteDeviceNetworkStatus>();
        }

        public void AddClick(AdsSdkModel ads, string softwareName, string pageName, string previousPage = "", string previousPageArea = "")
        {
            if (ads == null) return;
            Task.Factory.StartNew(() =>
            {
                AddBehavoirData(ads.Id.ToString(), "ads", "click", softwareName, pageName,previousPage,previousPageArea);
            });
        }

        public void AddLike(ShowProductInfo productInfo, string softwareName, string pageName, string previousPage = "", string previousPageArea = "")
        {
            if (productInfo == null) return;
            Task.Factory.StartNew(() => 
            {
                AddBehavoirData(productInfo.Id.ToString(), productInfo.Name, productInfo.Type.ToString(), "like", softwareName, pageName, previousPage, previousPageArea);
            });
        }

        public void AddClick(ShowProductInfo productInfo, string softwareName, string pageName, string previousPage = "", string previousPageArea = "")
        {
            if (productInfo == null) return;
            Task.Factory.StartNew(() =>
            {
                AddBehavoirData(productInfo.Id.ToString(), productInfo.Name, productInfo.Type.ToString(), "click", softwareName, pageName, previousPage, previousPageArea, productInfo.Product?.Id??0);
            });
        }

        public void AddBehavoirData(string thingId, string thingName, string category, string action, DateTime collectionTime,DateTime collectEndTime, string softwareName = "", string pageName = "", string previousPage = "", string previousPageArea = "", long productId = 0)
        {
            //todo:william.
            Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(action)) return;
                var updateRecord = m_db.Table<SqlLiteBehaviorRecord>().FirstOrDefault(r => r.ThingId == thingId && r.Category == category && r.Action == action && r.IsSynced == false);
                if (updateRecord == null)
                {
                    SqlLiteBehaviorRecord record = new SqlLiteBehaviorRecord();
                    record.Action = action;
                    record.CollectionTime = collectionTime;
                    record.CollectEndTime = collectEndTime;
                    record.Increment = 1;
                    record.ThingId = thingId;
                    record.Name = thingName;
                    record.SoftwareName = softwareName;
                    record.PageName = pageName;
                    record.Category = category;
                    record.PreviousPageArea = previousPageArea;
                    record.PreviousPageName = previousPage;
                    record.IsSynced = false;
                    record.ProductId = productId;
                    m_db.Insert(record);
                }
                else
                {
                    updateRecord.Increment++;
                    m_db.Update(updateRecord);
                }
                UploadData();
            });

        }


        public void AddBehavoirData(string thingId, string thingName, string category,string action,string softwareName = "", string pageName ="",string previousPage= "",string previousPageArea = "",long productId = 0)
        {
            //todo:william.
            Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(action)) return;
                var updateRecord = m_db.Table<SqlLiteBehaviorRecord>().FirstOrDefault(r => r.ThingId == thingId && r.Category == category && r.Action == action && r.IsSynced == false);
                if(updateRecord == null)
                {
                    SqlLiteBehaviorRecord record = new SqlLiteBehaviorRecord();
                    record.Action = action;
                    record.CollectionTime = DateTime.Now;
                    record.CollectEndTime = DateTime.Now;
                    record.Increment = 1;
                    record.ThingId = thingId;
                    record.Name = thingName;
                    record.SoftwareName = softwareName;
                    record.PageName = pageName;
                    record.Category = category;
                    record.PreviousPageArea = previousPageArea;
                    record.PreviousPageName = previousPage;
                    record.IsSynced = false;
                    record.ProductId = productId;
                    m_db.Insert(record);
                }
                else
                {
                    updateRecord.Increment++;
                    m_db.Update(updateRecord);
                }
                UploadData();
            });
           
        }

        public List<ClickInfo> ReadClickData()
        {
            var query = m_db.Query<ClickInfo>("select ThingId,sum(Increment) ClickCount from SqlLiteBehaviorRecord where Action='click' group by ThingId");
            return query;
        }

        public List<ClickInfo> ReadLikeClickData()
        {
            var query = m_db.Query<ClickInfo>("select ThingId,sum(Increment) ClickCount from SqlLiteBehaviorRecord where Action='like' group by ThingId");
            return query;
        }

        public List<ClickInfo> ReadAllClickData()
        {
            var query = m_db.Query<ClickInfo>("select ThingId,sum(Increment) ClickCount from SqlLiteBehaviorRecord group by ThingId");
            return query;
        }

        public List<ClickInfo> ReadMatchClickData()
        {
            var query = m_db.Query<ClickInfo>("select ThingId,count(*) ClickCount from SqlLiteBehaviorRecord where Action='match' group by ThingId");
            return query;
        }

        private void UploadData()
        {
            //if (DateTime.Now.Subtract(mLastUploadTime).TotalMinutes < 30)
            //    return;
            var records = m_db.Table<SqlLiteBehaviorRecord>().Where(r => r.IsSynced == false).Take(15).ToList();
            if (records.Count() > 0)
            {
                bool success = sesingWebClient.PostBehaviorRecordsAsync(records).GetAwaiter().GetResult();
                if (success)
                {
                    foreach (var r in records)
                    {
                        r.IsSynced = true;
                    }
                    m_db.UpdateAll(records);
                    mLastUploadTime = DateTime.Now;
                }
                int deletedCount = m_db.Execute("delete from SqlLiteBehaviorRecord where IsSynced =? and  CollectionTime < ?", true, DateTime.Today.AddDays(-15));
            }
        }

        private void UploadFaceData()
        {
            if (DateTime.Now.Subtract(mLastUploadTime).TotalMinutes < 30)
                return;
            var records = m_db.Table<SqlLiteFaceRecord>().Where(r => r.IsSynced == false).Take(50).ToList();
            for (int i = 0; i < records.Count; i++)
            {
                var record = records[i];
                bool success = sesingWebClient.PostFaceRecordAsync(record).GetAwaiter().GetResult();
                if (success)
                {
                    record.IsSynced = true;
                }
            }
            m_db.UpdateAll(records);
            int deletedCount = m_db.Execute("delete from SqlLiteFaceRecord where IsSynced =? and  CollectionTime < ?", true, DateTime.Today.AddDays(-15));


        }



        public void LogDeviceStatus(int secondInterval)
        {
            var now = DateTime.Now;
            var record = m_db.Table<SqliteDeviceStatus>().Where(r => r.IsSynced == false).OrderByDescending(d => d.EndTime).Take(1).FirstOrDefault();
            //var cpu = GetCpuUsage();
            //MEMORY_INFO memInfo = new MEMORY_INFO();
            //GlobalMemoryStatus(ref memInfo);
            //var memory = memInfo.dwMemoryLoad;
            if (record != null && record.EndTime.Date.Subtract(now.Date).Days == 0)
            {
                if(now.Subtract(record.EndTime).TotalSeconds > secondInterval)
                {
                    var newRecord = new SqliteDeviceStatus { StartTime = now, EndTime = now, IsSynced = false, Cpu = 0, Memory = 0 };
                    m_db.Insert(newRecord);
                }
                else
                {
                    record.EndTime = DateTime.Now;
                    //if (cpu > record.Cpu) record.Cpu = cpu;
                    //if (memory > record.Memory) record.Memory = memory;
                    bool success = sesingWebClient.PostDeviceStatusRecordAsync(new List<SqliteDeviceStatus> { record }).GetAwaiter().GetResult();
                    m_db.Update(record);
                }
            }
            else
            {
                var newRecord = new SqliteDeviceStatus { StartTime = now, EndTime = now, IsSynced = false, Cpu = 0, Memory = 0 };
                m_db.Insert(newRecord);
            }
            //todo: update to cloud.
            SyncToCloud();
        }

        public void LogDeviceNetworkStatusRecords(int pingSeed, int secondInterval)
        {
            if (pingSeed == 0)
                return;
            var now = DateTime.Now;
            var record = m_db.Table<SqliteDeviceNetworkStatus>().Where(r => r.IsSynced == false).OrderByDescending(d => d.CollectionEndTime).Take(1).FirstOrDefault();
            if (record != null && record.CollectionEndTime.Date.Subtract(now.Date).Days == 0)
            {
                if (now.Subtract(record.CollectionEndTime).TotalSeconds > secondInterval)
                {
                    var newRecord = new SqliteDeviceNetworkStatus { CollectionTime = now, CollectionEndTime = now, IsSynced = false,PingSeed = pingSeed,MaxPingSeed = pingSeed };
                    m_db.Insert(newRecord);
                }
                else
                {
                    record.CollectionEndTime = DateTime.Now;
                    record.PingSeed = (record.PingSeed + pingSeed) / 2;
                    record.MaxPingSeed = Math.Max(record.PingSeed, pingSeed);
                    bool success = sesingWebClient.PostDeviceNetworkStatusRecords(new List<SqliteDeviceNetworkStatus> { record }).GetAwaiter().GetResult();
                    m_db.Update(record);
                }
            }
            else
            {
                var newRecord = new SqliteDeviceNetworkStatus { CollectionTime = now,CollectionEndTime = now, IsSynced = false, PingSeed = pingSeed ,MaxPingSeed = pingSeed};
                m_db.Insert(newRecord);
            }
            //todo: update to cloud.
            SyncDeviceNetworkStatusToCloud();
        }

        public void AddFaceRecord(string facePath, string softwareName = "", string pageName = "")
        {
            Task.Factory.StartNew(() =>
            {
                byte[] bytesImage = File.ReadAllBytes(facePath);
                string base64Image = Convert.ToBase64String(bytesImage);
                SqlLiteFaceRecord record = new SqlLiteFaceRecord
                {
                    SoftwareName = softwareName,
                    PageName = pageName,
                    IsSynced = false,
                    CollectionTime = DateTime.Now,
                    CollectEndTime = DateTime.Now,
                    Face = base64Image
                };
                m_db.Insert(record);

                UploadFaceData();
            });
        }

        public void SyncToCloud()
        {
            var now = DateTime.Now;
            var records = m_db.Table<SqliteDeviceStatus>().Where(r => r.IsSynced == false).OrderByDescending(d => d.EndTime).Skip(1).OrderBy(d => d.EndTime).Take(10).ToList();
            if (records.Count() > 0)
            {
                bool success = sesingWebClient.PostDeviceStatusRecordAsync(records).GetAwaiter().GetResult();
                if (success)
                {
                    foreach (var r in records)
                    {
                        r.IsSynced = true;
                    }
                    m_db.UpdateAll(records);
                }
            }

            //POST the lastest record for updating,but not set IsSynced = true. next time will be updated again.
            var lastRecord = m_db.Table<SqliteDeviceStatus>().Where(r => r.IsSynced == false).OrderByDescending(d => d.EndTime).Take(1).FirstOrDefault();
            if (lastRecord != null)
            {
                bool success = sesingWebClient.PostDeviceStatusRecordAsync(new DeviceStatusInput[] { lastRecord }).GetAwaiter().GetResult();
                if (success)
                {
                    //todo
                }
            }
        }

        public void SyncDeviceNetworkStatusToCloud()
        {
            var now = DateTime.Now;
            var records = m_db.Table<SqliteDeviceNetworkStatus>().Where(r => r.IsSynced == false).OrderByDescending(d => d.CollectionEndTime).Skip(1).OrderBy(d => d.CollectionEndTime).Take(10).ToList();
            if (records.Count() > 0)
            {
                bool success = sesingWebClient.PostDeviceNetworkStatusRecords(records).GetAwaiter().GetResult();
                if (success)
                {
                    foreach (var r in records)
                    {
                        r.IsSynced = true;
                    }
                    m_db.UpdateAll(records);
                }
            }
        }
        public int GetCpuUsage()
        {
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", "MyComputer");
            cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            return (int)cpuCounter.NextValue();
        }

        [DllImport("kernel32")]
        public static extern void GlobalMemoryStatus(ref MEMORY_INFO meminfo);

    }
    //定义内存的信息结构    
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_INFO
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public uint dwTotalPhys;
        public uint dwAvailPhys;
        public uint dwTotalPageFile;
        public uint dwAvailPageFile;
        public uint dwTotalVirtual;
        public uint dwAvailVirtual;
    }
    public  class SqlLiteBehaviorRecord : BehaviorRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public bool IsSynced { get; set; }
    }

    public class SqliteDeviceStatus : DeviceStatusInput
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public bool IsSynced { get; set; }
    }

    public class SqlLiteFaceRecord : FaceRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public bool IsSynced { get; set; }
    }

    public class SqliteDeviceNetworkStatus : DeviceNetworkStatusInput
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int MaxPingSeed { get; set; }

        public bool IsSynced { get; set; }
    }

}
