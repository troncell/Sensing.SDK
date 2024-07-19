using Newtonsoft.Json;
using Sensing.SDK.Contract;
using Sensing.SDK.Contract.SensingDeviceActivityDto;
using SensingStoreCloud.Activity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK
{
    partial class SensingWebClient
    {
        /// <summary>
        /// Post device data record.
        /// </summary>
        private const string CreateQrcode4LoginQuery = "/BehaviorRecord/PostRecord";
        //private const string PostDeviceStatusQuery = "/BehaviorRecord/PostDeviceStatusRecord";
        private const string GetDeviceActivityGameInfoQuery = "services/app/SensingDeviceActivity/GetDeviceActivityGameInfo";
        private const string GetAwardsByActivityQuery ="services/app/SensingDeviceActivity/GetAwardsByActivity";
        private const string GetPlayGamesByUserQuery = "services/app/SensingDeviceActivity/GetPlayGamesByUser";
        private const string GetRankedUsersWithActionByActivityQuery = "services/app/SensingDeviceActivity/GetRankedUsersWithActionByActivity";
        private const string GetQrcode4LoginQuery = "services/app/SensingDeviceActivity/CreateQrCode4Login";
        private const string MySignInActivityQuery = "services/app/SensingDeviceActivity/MySignInActivity";
        private const string PostPlayerData4ActionQrcodeQuery = "UserAction/PostPlayerData4ActionQrcode";
        private const string ActionDataByIdQuery = "UserAction/ActionDataById";
        private const string PostPlayerDataByActionQuery = "UserAction/PostPlayerDataByAction";
        private const string PostDataByUserQuery = "UserAction/PostDataByUser";
        private const string GetScanQrCodeUsersQuery = "services/app/SensingDeviceActivity/GetScanQrCodeUsers";
        private const string GetMyPersonDataInActivityQuery = "services/app/SensingDeviceActivity/GetMyPersonDataInActivity";
        private const string GetScanQrCodeUserActionsQuery = "services/app/SensingDeviceActivity/GetScanQrCodeUserActions";
        private const string GetDataUsersQuery = "services/app/SensingDeviceActivity/GetDataUsers";
        private const string GetActivityGamesQuery = "services/app/SengsingDevice/GetActivityGames";

        private const string DoLotteryUserByAwardIdQuery = "services/app/SensingDeviceActivity/DoLotteryUserByAwardId";
        private const string SendAwardNotifyQuery = "services/app/SensingDeviceActivity/SendAwardNotify";
        private const string DoLotteryAwardByActionQuery = "services/app/SensingDeviceActivity/DoLotteryAwardByAction";
        private const string GetUserActionByIdQuery = "services/app/SensingDeviceActivity/GetUserActionById";
        private const string GetUserActionsByActivityGameQuery = "services/app/SensingDeviceActivity/GetUserActionsByActivityGame";
        private const string GetUserActionsByActivityQuery = "services/app/SensingDeviceActivity/GetUserActionsByActivity";
        private const string SendTextMessageByUserQuery = "services/app/SensingDeviceActivity/SendTextMessageByUser";
        private const string GetActivityChattingRecordsQuery = "services/app/SensingDeviceActivity/GetActivityChattingRecords";
        private const string GetDeviceActivityGameChattingRecordsQuery = "services/app/SensingDeviceActivity/GetDeviceActivityGameChattingRecords";
        private const string GetPaperQuery = "services/app/SengsingDevice/GetPapers";
        private const string GetQuestionsByPaperIdQuery = "services/app/SengsingDevice/GetQuestionsByPaperId";
        private const string GetPapersByTagsQuery = "services/app/SengsingDevice/GetPapersByTags";
        private const string AddUserPaperQuery = "services/app/SengsingDevice/AddUserPaper";
        private const string GetPaperAnswerReportQuery = "services/app/SengsingDevice/GetPaperAnswerReport";
        private const string GetDeviceActivityGameUserActionsQuery = "services/app/SensingDeviceActivity/GetDeviceActivityGameUserActions";
        public readonly static string ActivityServiceRelativePath = "g/";
        public readonly static string ActivityServiceApiHost = ServerBase + ActivityDataPath + Api_Relative_Path;
        public readonly static string ActivityServiceHost = ServerBase + ActivityDataPath;
        private const string ActivityDataPath = "g/api/";

        public async Task<QrcodeOutput> CreateQrCode4LoginAsync(CreatQrcodeInput input)
        {
            input.SecurityKey = _deviceActivityGameSecurityKey;
            // var absolutePath = $"{ServerBase}{ActivityDataPath}{GetQrcode4LoginQuery}";
             var absolutePath = $"https://g.api.troncell.com/api/{GetQrcode4LoginQuery}";

            try
            {
                var webResult = await SendRequestAsync<CreatQrcodeInput, AjaxResponse<QrcodeOutput>>(HttpMethod.Post, absolutePath, input);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetRecommendsByFaces:" + ex.InnerException);
            }
            return null;
        }

        public async Task<ActivityUserDataOutput> MySignInActivityAsync(MySignInActivityInput input)
        {
            input.SecurityKey = _deviceActivityGameSecurityKey;
            var absolutePath = $"{ServerBase}{ActivityDataPath}{MySignInActivityQuery}";
            try
            {
                var webResult = await SendRequestAsync<MySignInActivityInput, AjaxResponse<ActivityUserDataOutput>>(HttpMethod.Post, absolutePath, input);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("MySignInActivityAsync:" + ex.InnerException);
            }
            return null;
        }

        public async Task<DeviceActivityGameOutput> GetDeviceActivityGameInfoAsync()
        {
            //input.Subkey = _subKey;
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetDeviceActivityGameInfoQuery}?{GetBasicNameValuesQueryString()}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<DeviceActivityGameOutput>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetDeviceActivityGameInfoAsync:" + ex.InnerException);
            }
            return null;
        }

        public async Task<UserActionInfoOutput> PostPlayerDataByAction(PlayerActionDataInput playerActionData)
        {
            var absolutePath = $"{ServerBase}{ActivityDataPath}{PostPlayerDataByActionQuery}";
            //var absolutePath = "http://localhost:13654/api/UserAction/PostPlayerDataByAction";
            var nameValues = new NameValueCollection();
            AddBasicNameValues(nameValues);
            nameValues.Add("score", playerActionData.Score.ToString());
            nameValues.Add("params", playerActionData.Params);
            nameValues.Add("qrType", playerActionData.QrType.ToString());
            nameValues.Add("snsType", playerActionData.SnsType.ToString());
            nameValues.Add("IsSendWeChatMsg", playerActionData.IsSendWeChatMsg.ToString());
            nameValues.Add("TargetUrl", playerActionData.TargetUrl);
            nameValues.Add("ActionId", playerActionData.ActionId.ToString());

            try
            {
                var files = new List<string>();
                var names = new List<string>();
                if (!string.IsNullOrEmpty(playerActionData.PlayerImage) && File.Exists(playerActionData.PlayerImage))
                {
                    files.Add(playerActionData.PlayerImage);
                    names.Add("playerimage");
                }
                if (!string.IsNullOrEmpty(playerActionData.PlayingImage) && File.Exists(playerActionData.PlayingImage))
                {
                    files.Add(playerActionData.PlayingImage);
                    names.Add("PlayingImage");
                }
                return await SendMultipartFormRequestAsync<UserActionInfoOutput>(absolutePath, files.ToArray(), names.ToArray(), nameValues);
            }
            catch (Exception ex)
            {
                logger.Error("PostData4ScanAsync", ex);
            }
            return default(UserActionInfoOutput);
        }

        public async Task<UserActionInfoOutput> PostDataByUserAsync(PlayerDataActionInput playerActionData)
        {
            var absolutePath = $"{ServerBase}{ActivityDataPath}{PostDataByUserQuery}";
            var nameValues = new NameValueCollection();
            AddBasicNameValues(nameValues);
            nameValues.Add("score", playerActionData.Score.ToString());
            nameValues.Add("params", playerActionData.Params);
            nameValues.Add("qrType", playerActionData.QrType.ToString());
            nameValues.Add("snsType", playerActionData.SnsType.ToString());
            nameValues.Add("IsSendWeChatMsg", playerActionData.IsSendWeChatMsg.ToString());
            nameValues.Add("TargetUrl", playerActionData.TargetUrl);
            nameValues.Add("ActionId", playerActionData.ActionId.ToString());
            nameValues.Add("OpenId", playerActionData.OpenId.ToString());
            nameValues.Add("Type", playerActionData.Type?.ToString() ?? "");
            nameValues.Add("ExtensionData", playerActionData.ExtensionData?.ToString() ?? "");
            try
            {
                var files = new List<string>();
                var names = new List<string>();
                if (!string.IsNullOrEmpty(playerActionData.PlayerImage) && File.Exists(playerActionData.PlayerImage))
                {
                    files.Add(playerActionData.PlayerImage);
                    names.Add("playerimage");
                }
                if (!string.IsNullOrEmpty(playerActionData.PlayingImage) && File.Exists(playerActionData.PlayingImage))
                {
                    files.Add(playerActionData.PlayingImage);
                    names.Add("PlayingImage");
                }
                return await SendMultipartFormRequestAsync<UserActionInfoOutput>(absolutePath, files.ToArray(), names.ToArray(), nameValues);
            }
            catch (Exception ex)
            {
                logger.Error("PostDataByUserAsync", ex);
            }
            return default(UserActionInfoOutput);
        }

        public async Task<QrcodeActionOutput> PostPlayerData4ActionQrcodeAsync(PlayerDataInput playerData)
        {
            var absolutePath = $"https://g.api.troncell.com/api/{PostPlayerData4ActionQrcodeQuery}";
            var nameValues = new NameValueCollection();
            AddBasicNameValues(nameValues);
            nameValues.Add("score", playerData.Score.ToString());
            if (!string.IsNullOrEmpty(playerData.Params))
                nameValues.Add("params", playerData.Params);
            nameValues.Add("qrType", playerData.QrType.ToString());
            nameValues.Add("snsType", playerData.SnsType.ToString());
            nameValues.Add("IsSendWeChatMsg", playerData.IsSendWeChatMsg.ToString());
            nameValues.Add("IsTransferred", playerData.IsTransferred.ToString());
            nameValues.Add("type", playerData.Type?.ToString()??"");

            nameValues.Add("extensionData", playerData.ExtensionData?.ToString()??"");
            if (!string.IsNullOrEmpty(playerData.TargetUrl)) nameValues.Add("TargetUrl", playerData.TargetUrl);

            try
            {
                var files = new List<string>();
                var names = new List<string>();
                if (!string.IsNullOrEmpty(playerData.PlayerImage) && File.Exists(playerData.PlayerImage))
                {
                    files.Add(playerData.PlayerImage);
                    names.Add("playerimage");
                }
                if (!string.IsNullOrEmpty(playerData.PlayingImage) && File.Exists(playerData.PlayingImage))
                {
                    files.Add(playerData.PlayingImage);
                    names.Add("PlayingImage");
                }
                var data =  await SendMultipartFormRequestAsync<AjaxResponse<QrcodeActionOutput>>(absolutePath, files.ToArray(), names.ToArray(), nameValues);
                return data.Result;
            }
            catch (Exception ex)
            {
                logger.Error("PostData4ScanAsync", ex);
                Console.WriteLine(ex.Message);
            }
            return default(QrcodeActionOutput);
        }


        public async Task<ActionDataOutput> ActionDataById(ActionInput actionDataInput)
        {
            var absolutePath = $"{ServerBase}{ActivityDataPath}{ActionDataByIdQuery}";
            var nameValues = new NameValueCollection();
            AddBasicNameValues(nameValues);
            nameValues.Add("actionId", actionDataInput.ActionId.ToString());        
            try
            {             
                var data = await SendRequestAsync<ActionInput, AjaxResponse<ActionDataOutput>>(HttpMethod.Post, absolutePath, actionDataInput);
                return data.Result;
            }
            catch (Exception ex)
            {
                logger.Error("ActionDataById", ex);
            }
            return default(ActionDataOutput);
        }

        //todo.. GetRankedUsersWithActionByActivity
        public async Task<PagedResultDto<UserRankDto>> GetRankedUsersWithActionByActivity(GetRankedUsersWithActionByActivityInput input)
        {
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetRankedUsersWithActionByActivityQuery}?{GetBasicNameValuesQueryString()}&rankColumn={input.RankColumn}&startTime={input.StartTime}&endTime={input.EndTime}&isGameLevel={input.IsGameLevel}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<UserRankDto>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetRankedUsersWithActionByActivity:" + ex.InnerException);
            }
            return null;
        }

        public async Task<PagedResultDto<PlayedGamesOutput>> GetPlayGamesByUserAsync(PlayGameInput input)
        {
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetPlayGamesByUserQuery}?{GetBasicNameValuesQueryString()}&openId={input.OpenId}&snsType={input.SnsType}&maxResultCount={input.MaxResultCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<PlayedGamesOutput>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetPlayGamesByUserAsync:" + ex.InnerException);
            }
            return null;
        }



        public async Task<PagedResultDto<SnsUserInfoOutput>> GetScanQrCodeUsers(Qrcode4UsersInput input)
        {
            input.SecurityKey = _deviceActivityGameSecurityKey;
            var absolutePath = $"https://g.api.troncell.com/api/{GetScanQrCodeUsersQuery}?{GetBasicNameValuesQueryString()}&qrcodeId={input.QrcodeId}&{MaxResultCount}={input.MaxResultCount}&{SkipCount}={input.SkipCount}&{Query}={input.Filter}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<SnsUserInfoOutput>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetScanQrCodeUsers:" + ex.InnerException);
            }
            return null;
        }

        //todo
        public async Task<PersonDataOutput> GetMyPersonDataInActivity(PersonDataInput input)
        {
            input.SecurityKey = _deviceActivityGameSecurityKey;
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetMyPersonDataInActivityQuery}?{GetBasicNameValuesQueryString()}&openID={input.OpenId}&snsType={input.SnsType}&actionId={input.ActionId}";

            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PersonDataOutput>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetMyPersonDataInActivity:" + ex.InnerException);
            }
            return null;
        }

        public async Task<PagedResultDto<UserActionInfoOutput>> GetScanQrCodeUserActions(Qrcode4UsersInput input)
        {
            input.SecurityKey = _deviceActivityGameSecurityKey;
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetScanQrCodeUserActionsQuery}?{GetBasicNameValuesQueryString()}&qrcodeId={input.QrcodeId}&{MaxResultCount}={input.MaxResultCount}&{SkipCount}={input.SkipCount}&{Query}={input.Filter}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<UserActionInfoOutput>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetScanQrCodeUsers:" + ex.InnerException);
            }
            return null;
        }

        //todo
        public async Task<PagedResultDto<UserDataOutput>> GetDataUsers(DataUsersInput input)
        {
            input.SecurityKey = _deviceActivityGameSecurityKey;
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetDataUsersQuery}?{GetBasicNameValuesQueryString()}&securityKey={input.SecurityKey}&maxResultCount={input.MaxResultCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<UserDataOutput>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetDataUsers:" + ex.InnerException);
            }
            return null;
        }


        public async Task<List<ActivityGameDto>> GetActivityGames()
        {
              var absolutePath = $"{ServerBase}{ActivityDataPath}{GetActivityGamesQuery}?{GetBasicNameValuesQueryString()}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<List<ActivityGameDto>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetActivityGames:" + ex.InnerException);
            }
            return null;
        }


        public async Task<SnsUserAwardOuput> DoLotteryUserByAwardId(AwardDataInput input)
        {
            input.SecurityKey = _deviceActivityGameSecurityKey;
            var absolutePath = $"{ServerBase}{ActivityDataPath}{DoLotteryUserByAwardIdQuery}";
            try
            {
                var webResult = await SendRequestAsync<AwardDataInput, AjaxResponse<SnsUserAwardOuput>>(HttpMethod.Post, absolutePath, input);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DoLotteryByAwardId:" + ex.InnerException);
            }
            return null;
        }

        public async Task<int> SendAwardNotify(NotifyInput input)
        {
            input.SecurityKey = _deviceActivityGameSecurityKey;
            var absolutePath = $"{ServerBase}{ActivityDataPath}{SendAwardNotifyQuery}";
            try
            {
                var webResult = await SendRequestAsync<NotifyInput, AjaxResponse<int>>(HttpMethod.Post, absolutePath, input);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendAwardNotify:" + ex.InnerException);
            }
            return -1;
        }

        public async Task<SnsUserAwardOuput> DoLotteryAwardByAction(ActionDataInput input)
        {
            input.SecurityKey = _deviceActivityGameSecurityKey;
            var absolutePath = $"{ServerBase}{ActivityDataPath}{DoLotteryAwardByActionQuery}";
            //var absolutePath = "http://localhost:13654/api/services/app/SensingDeviceActivity/DoLotteryAwardByAction";

            var webResult = await SendRequestAsync<ActionDataInput, AjaxResponse<SnsUserAwardOuput>>(HttpMethod.Post, absolutePath, input);
            return webResult.Result;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("DoLotteryByAwardId:" + ex.InnerException);
            //}
            //return null;
        }

        public async Task<PagedResultDto<AwardOutput>> GetAwardsAsync(AwardInput input)
        {
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetAwardsByActivityQuery}?{GetBasicNameValuesQueryString()}&Sorting={"awardSeq ASC"}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<AwardOutput>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAwardsAsync:" + ex.InnerException);
            }
            return null;
        }

        //public async Task<PagedResultDto<AwardOutput>> GetAwardsBySecurityKeyAsync(AwardInput input)
        //{
        //    var absolutePath = $"{ServerBase}{ActivityDataPath}{GetAwardsByActivityQuery}?{GetBasicNameValuesQueryString()}&Sorting={"awardSeq ASC"}";
        //    try
        //    {
        //        var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<AwardOutput>>>(HttpMethod.Get, absolutePath, null);
        //        return webResult.Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("GetRecommendsByFaces:" + ex.InnerException);
        //    }
        //    return null;
        //}

        public async Task<UserActionInfoOutput> GetUserActionByIdAsync(ActionDataInput input)
        {
            input.SecurityKey = _deviceActivityGameSecurityKey;
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetUserActionByIdQuery}?{GetBasicNameValuesQueryString()}&actionId={input.ActionId}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<UserActionInfoOutput>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetUserActionById:" + ex.InnerException);
            }
            return null;
        }

        public async Task<PagedResultDto<UserActionInfoOutput>> GetUserActionsByActivityGameAsync(ActivityGamePageInput input)
        {
            input.SecurityKey = _deviceActivityGameSecurityKey;
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetUserActionsByActivityGameQuery}?{GetBasicNameValuesQueryString()}&filter={input.Filter}&sorting={input.Sorting}&maxResultCount={input.MaxResultCount}&skipCount={input.SkipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<UserActionInfoOutput>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetUserActionsByActivityGameAsync:" + ex.InnerException);
            }
            return null;
        }

        /// <summary>
        /// 发送聊天信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> SendTextMessageByUserAsync(SnsUserTextDataInput input)
        {
            var absolutePath = $"{ServerBase}{ActivityDataPath}{SendTextMessageByUserQuery}";
            input.SecurityKey = _deviceActivityGameSecurityKey;
            try
            {
                var webResult = await SendRequestAsync<SnsUserTextDataInput, AjaxResponse<int>>(HttpMethod.Post, absolutePath, input);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendTextMessageByUserAsync:" + ex.InnerException);
            }
            return -1;
        }

        /// <summary>
        /// 获得活动的聊天信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<PagedResultDto<ChatMessage>> GetActivityChattingRecordsAsync(SensingDeviceGamePagedSortedFilteredInputBase input)
        {
            input.SecurityKey = _deviceActivityGameSecurityKey;
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetActivityChattingRecordsQuery}?{GetBasicNameValuesQueryString()}&filter={input.Filter}&sorting={input.Sorting}&maxResultCount={input.MaxResultCount}&skipCount={input.SkipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<ChatMessage>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetActivityChattingRecords:" + ex.InnerException);
            }
            return null;
        }


        /// <summary>
        /// 获得游戏的聊天信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ChatMessage>> GetDeviceActivityGameChattingRecordAsync(SensingDeviceGamePagedSortedFilteredInputBase input)
        {
            input.SecurityKey = _deviceActivityGameSecurityKey;            
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetDeviceActivityGameChattingRecordsQuery}?{GetBasicNameValuesQueryString()}&filter={input.Filter}&sorting={input.Sorting}&maxResultCount={input.MaxResultCount}&skipCount={input.SkipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<ChatMessage>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetActivityChattingRecords:" + ex.InnerException);
            }
            return null;
        }

        public async Task<PagedResultDto<PaperDto>> GetPaperAsync(PaperInput input)
        {
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetPaperQuery}?{GetBasicNameValuesQueryString()}&softwareId={input.SoftwareId}&filter={input.Filter}&sorting={input.Sorting}&maxResultCount={input.MaxResultCount}&skipCount={input.SkipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<PaperDto>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetActivityChattingRecords:" + ex.InnerException);
            }
            return null;
        }

        public async Task<PagedResultDto<QuestionDto>> GetQuestionsByPaperIdAsync(QuestionInput input)
        {
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetQuestionsByPaperIdQuery}?{GetBasicNameValuesQueryString()}&paperId={input.PaperId}&sorting={input.Sorting}&maxResultCount={input.MaxResultCount}&skipCount={input.SkipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<QuestionDto>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetActivityChattingRecords:" + ex.InnerException);
            }
            return null;
        }

        public async Task<PagedResultDto<PaperDto>> GetPapersByTagAsync(string tag)
        {
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetPapersByTagsQuery}?{GetBasicNameValuesQueryString()}&Tags={tag}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<PaperDto>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetActivityChattingRecords:" + ex.InnerException);
            }
            return null;
        }

        public async Task<string> AddUserPaperAsync(AddUserPaperInput input)
        {
            var absolutePath = $"{ServerBase}{ActivityDataPath}{AddUserPaperQuery}?{GetBasicNameValuesQueryString()}";
            input.subkey = _subKey;
            try
            {
                var webResult = await SendRequestAsync<AddUserPaperInput, AjaxResponse<string>>(HttpMethod.Post, absolutePath, input);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddUserPaperAsync:" + ex.InnerException);
            }
            return null;
        }

        public async Task<PagedResultDto<PaperAnswerReportDto>> GetPaperAnswerReportAsync(int paperId)
        {
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetPaperAnswerReportQuery}?{GetBasicNameValuesQueryString()}&paperId={paperId}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<PaperAnswerReportDto>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetPaperAnswerReportAsync:" + ex.InnerException);
            }
            return null;
        }

        public async Task<PagedResultDto<GameUserActionOutput>> GetDeviceActivityGameUserActions(ActivityGameActionInput input)
        {
            input.SecurityKey = _deviceActivityGameSecurityKey;
            var absolutePath = $"{ServerBase}{ActivityDataPath}{GetDeviceActivityGameUserActionsQuery}?{GetBasicNameValuesQueryString()}&filter={input.Filter}&sorting={input.Sorting}&maxResultCount={input.MaxResultCount}&skipCount={input.SkipCount}&startTime={input.StartTime}&endTime={input.EndTime}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<GameUserActionOutput>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetUserActionsByActivityGameAsync:" + ex.InnerException);
            }
            return null;
        }

    }
}
