using Newtonsoft.Json;
using Sensing.SDK;
using Sensing.SDK.Contract;
using Sensing.SDK.Contract.Faces;
using SensingSite.ClientSDK.Common;
using SensingStoreCloud.Activity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZXing;
using static Sensing.SDK.SensingWebClient;
using Brushes = System.Windows.Media.Brushes;
using Path = System.IO.Path;

namespace Sensing.SDK.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SensingWebClient _sensingWebClient;
        private Dictionary<string, EnumQRStatus> qrcodeTypes = new Dictionary<string, EnumQRStatus>();
        public MainWindow()
        {
            InitializeComponent();
            this.ClientNoTB.Text = MacIPHelper.GetClientMac();

            qrcodeTypes.Add("游戏之前", EnumQRStatus.BeforeGame);
            qrcodeTypes.Add("游戏之后", EnumQRStatus.AfterGame);
            qrcodeTypes.Add("活动介绍", EnumQRStatus.ActivityIntroduction);
            qrcodeTypes.Add("活动注册", EnumQRStatus.ActivityRegister);
            qrcodeTypes.Add("在线游戏", EnumQRStatus.OnlineGame);
            qrcodeTypes.Add("游戏结果", EnumQRStatus.GameResult);
            qrcodeTypes.Add("中奖信息", EnumQRStatus.Award);

            InitializeComponent();
            platformCBox.Items.Add("WeChat");
            platformCBox.Items.Add("Taobao");

            foreach (var pair in qrcodeTypes)
            {
                qrCodeCBox.Items.Add(pair.Key);
            }
            qrCodeCBox.SelectedIndex = 1;

            orderByCBox.Items.Add("score");
            orderByCBox.Items.Add("likecount");
            orderByCBox.Items.Add("viewcount");

            orderByCBox.SelectedIndex = 1;
        }

        #region private 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var subKey = SubKeyTB.Text;
            var mac = ClientNoTB.Text;
            var dAG = SecurityKey.Text;
            //subKey = "hahaha";
            //mac = "aa:bb:cc:dd:ee:f0";
            _sensingWebClient = new SensingWebClient(subKey, "123456", mac,dAG);
            CreateBtn.Background = Brushes.Green;
            tabControl.IsEnabled = true;
        }

        public static BitmapSource WebImageToImage(string imageUrl)
        {
            try
            {
                var imageBytes = new WebClient().DownloadData(imageUrl);
                MemoryStream ms = new MemoryStream(imageBytes);
                BitmapImage bmImage = new BitmapImage();
                bmImage.BeginInit();
                bmImage.StreamSource = ms;
                bmImage.EndInit();
                return bmImage;
            }
            catch
            {
                //var imageBytes = new WebClient().DownloadFile(imageUrl,"avator.png");
                new WebClient().DownloadFile(imageUrl, "avator.png");
                var path = Path.Combine(Environment.CurrentDirectory, "avator.png");
                OpenCvSharp.Mat src = new OpenCvSharp.Mat(path, OpenCvSharp.ImreadModes.Color);
                // Mat src = Cv2.ImRead("lenna.png", ImreadModes.GrayScale);
                BitmapImage bmImage = new BitmapImage();
                bmImage.BeginInit();
                bmImage.StreamSource = src.ToMemoryStream();
                bmImage.EndInit();
                return bmImage;
            }
        }

        public static BitmapSource ValueToImage(string qrcode)
        {

            IBarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.QR_CODE };
            var bitmap = writer.Write(qrcode);

            var hbmp = bitmap.GetHbitmap();
            BitmapSource source;
            try
            {
                source = Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            }
            finally
            {
                //DeleteObject(hbmp);
            }
            return source;
        }

        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
        public static BitmapSource UriToImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return null;
            if (imageUrl.Contains("mp.weixin.qq.com/cgi-bin/showqrcode") || imageUrl.Contains("wx.qlogo.cn"))
            {
                return WebImageToImage(imageUrl);
            }
            else
            {
                return ValueToImage(imageUrl);
            }
        }
        #endregion

        private async void UploadBehaviorDataBtn_Click(object sender, RoutedEventArgs e)
        {
            var action = ActionComboBox.SelectedValue.ToString();
            var category = ThingType.SelectedValue.ToString();
            var sku = ThingNoTB.Text;
            var name = ThingNameTB.Text;
            var increment = int.Parse(IncrementTB.Text);
            var records = new List<BehaviorRecord>();
            records.Add(new BehaviorRecord
            {
                Action = action,
                ThingId = sku,
                CollectionTime = DateTime.Now,
                CollectEndTime = DateTime.Now,
                Increment = increment,
                Category = category,
                Name = name,
                SoftwareName = SoftwareNameTB.Text,
                PageName = PageNameTB.Text
            });
            var result = await _sensingWebClient.PostBehaviorRecordsAsync(records);
            BMessage.Text += result + Environment.NewLine;
        }

        private async void GetThings_Click(object sender, RoutedEventArgs e)
        {
            ThingMsg.Text = "loading..." + Environment.NewLine;
            var data = await _sensingWebClient.GetProducts();
            var brand = await _sensingWebClient.GetBrands();
            if(data != null && data.Items != null)
            {
                ThingMsg.Text = "Thing Successfully" + Environment.NewLine;
                ThingMsg.Text += $"Total Count = {data.TotalCount}, Current Count {data.Items.Count()}" + Environment.NewLine;
            }
            else
            {
                ThingMsg.Text = "failed" + Environment.NewLine;
            }
        }

        private async void GetCategoriesBtn_Click(object sender, RoutedEventArgs e)
        {
            ThingMsg.Text = "loading..." + Environment.NewLine;
            var data = await _sensingWebClient.GetProductCategories();
            if (data != null)
            {
                ThingMsg.Text = "Categories Successfully" + Environment.NewLine;
                ThingMsg.Text += $"Total Count = {data.TotalCount}, Current Count {data.Items.Count()}" + Environment.NewLine;
            }
            else
            {
                ThingMsg.Text = "failed" + Environment.NewLine;
            }
        }

        private async void CouponBtn_Click(object sender, RoutedEventArgs e)
        {
            //todo:Zaric
            CouponMsg.Text = "loading..." + Environment.NewLine;
            var data = await _sensingWebClient.GetCoupons();
            if (data != null)
            {
                CouponMsg.Text = "Coupon Successfully" + Environment.NewLine;
                CouponMsg.Text += $"Total Count = {data.TotalCount}, Current Count {data.Items.Count()}" + Environment.NewLine;
            }
            else
            {
                CouponMsg.Text = "failed" + Environment.NewLine;
            }
        }

        private async void GetAds_Click(object sender, RoutedEventArgs e)
        {
            var data = await _sensingWebClient.GetAds();
            if (data != null)
            {
                AdsMessage.Text = "Ads Successfully" + Environment.NewLine;
                AdsMessage.Text += $"Total Count = {data.TotalCount}, Current Count {data.Items.Count()}" + Environment.NewLine;
            }
            else
            {
                AdsMessage.Text = "failed" + Environment.NewLine;
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void LikesBtn_Click(object sender, RoutedEventArgs e)
        {
            var data = await _sensingWebClient.GetLikeInfos();
            if (data != null)
            {
                LikeMsg.Text = "Likes Successfully" + Environment.NewLine;
                LikeMsg.Text += $"Total Count = {data.TotalCount}, Current Count {data.Items.Count()}" + Environment.NewLine;
            }
            else
            {
                LikeMsg.Text = "failed" + Environment.NewLine;
            }
        }

        private async void MatchBtn_Click(object sender, RoutedEventArgs e)
        {
            var data = await _sensingWebClient.GetMatchInfos();
            if (data != null)
            {
                MatchMsg.Text = "Match Successfully" + Environment.NewLine;
                MatchMsg.Text += $"Total Count = {data.TotalCount}, Current Count {data.Items.Count()}" + Environment.NewLine;
            }
            else
            {
                MatchMsg.Text = "failed" + Environment.NewLine;
            }
        }

        public string ImageToBase64(string imagePath)
        {
            byte[] bytesImage = File.ReadAllBytes(imagePath);
            return Convert.ToBase64String(bytesImage);
        }

        private async void GoodsByFacesBtn_Click(object sender, RoutedEventArgs e)
        {
            var faces = new FacesRecommendsInput();
            var face = new FaceImage();
            var fileName = genderCBox.SelectedIndex == 0 ? "face-m.jpg" : "face-f.jpg";
            var faceImagePath = System.IO.Path.Combine(Environment.CurrentDirectory, fileName);

            var bytes = File.ReadAllBytes(faceImagePath);
            face.Image = ImageToBase64(faceImagePath);
            face.Type = "head";
            faces.Faces = new FaceImage[] { face };
            var data = await _sensingWebClient.GetRecommendsByFaces(faces);
            if (data != null)
            {
                MatchMsg.Text = "GoodsByFaces Successfully" + Environment.NewLine;
                MatchMsg.Text += data;
            }
            else
            {
                MatchMsg.Text = "failed" + Environment.NewLine;
            }
        }

        private async void DeviceStatus_Click(object sender, RoutedEventArgs e)
        {
            var records = new List<DeviceStatusInput>();
            records.Add(new DeviceStatusInput
            {
                StartTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(5)),
                EndTime = DateTime.Now,
                Memory = 0.5,
                Cpu = 0.5
            });
            var result = await _sensingWebClient.PostDeviceStatusRecordAsync(records);
            BMessage.Text += result + Environment.NewLine;
        }

        private async void DeviceInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            var data = await _sensingWebClient.GetDeviceInfo();
            if (data != null)
            {
                BMessage.Text = "Match Successfully" + Environment.NewLine;
                BMessage.Text += JsonConvert.SerializeObject(data);
            }
            else
            {
                BMessage.Text = "failed" + Environment.NewLine;
            }
        }

        private async void MetaphysicsList_Click(object sender, RoutedEventArgs e)
        {
            var data = await _sensingWebClient.GetMetaphysicsList();
            if (data != null)
            {
                MetaphysicsStatusMessage.Text = $"Metaphysics Successfully with count = {data.Items.Count}" + Environment.NewLine;
            }
            else
            {
                MetaphysicsStatusMessage.Text = "failed" + Environment.NewLine;
            }
        }

        private async void DateMetaphysicsList_Click(object sender, RoutedEventArgs e)
        {
            var data = await _sensingWebClient.GetDateMetaPhysics(DateTime.Now, DateTime.Now.AddDays(30));
            if (data != null)
            {
                MetaphysicsStatusMessage.Text = $"GetDateMetaPhysics Successfully with count = {data.Items.Count}" + Environment.NewLine;
            }
            else
            {
                MetaphysicsStatusMessage.Text = "failed" + Environment.NewLine;
            }
        }

        private async void MemberFaceBtn_Click(object sender, RoutedEventArgs e)
        {
            //Go(null,null);
            var faces = new FaceInput();
            var fileName = genderCBox.SelectedIndex == 0 ? "face-m.jpg" : "face-f.jpg";
            var faceImagePath = System.IO.Path.Combine(Environment.CurrentDirectory, fileName);

            var bytes = File.ReadAllBytes(faceImagePath);
            faces.Face = bytes;

            File.WriteAllBytes("wu.txt", bytes);

            var data = await _sensingWebClient.IsFaceMember(faces);
            if (data != null)
            {
                MatchMsg.Text = "IsFaceMember Successfully" + Environment.NewLine;
                MatchMsg.Text += data;
            }
            else
            {
                MatchMsg.Text = "failed" + Environment.NewLine;
            }
        }




        #region Activity

        private void ScannedAvator_Click(object sender, RoutedEventArgs e)
        {
            Timer_ScanUsers(null,null);
        }

        UserActionInfoOutput firstUserAction;
        private async void Timer_ScanUsers(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(firstQrCode))
            {
                var first = new Qrcode4UsersInput { QrcodeId = long.Parse(firstQrCode), Sorting = "CreateTime" };
                var users = await _sensingWebClient.GetScanQrCodeUserActions(first);
                if (users != null && users.Items.Count > 0)
                {
                    scanCountBefore.Content = $"目前有{users.Items.Count} 人扫码";
                    firstUserAction = users.Items[0];
                    //avatorImg.Source = new BitmapImage(new Uri(user.Data.Headimgurl));
                    avatorImg.Source = UriToImage(firstUserAction.SnsUserInfo.Headimgurl);
                    //await gameSvc.PostDataByUserAsync(firstUser.ActionId.ToString(), null, null, 80);
                }
            }
        }


        private string firstQrCode;
        private string afterQrCode;
        private async void CreateQrcode_Click(object sender, RoutedEventArgs e)
        {
            var games = await _sensingWebClient.GetActivityGames();
            var snsType = platformCBox.SelectedValue.ToString() == "Taobao" ? EnumSnsType.Taobao : EnumSnsType.WeChat;
            var qrType = qrcodeTypes[qrCodeCBox.SelectedValue.ToString()];
            if (qrType == EnumQRStatus.AfterGame)
            {
                var playingData = new PlayerDataInput()
                {
                    IsSendWeChatMsg = true,
                    PlayerImage = System.IO.Path.Combine(Environment.CurrentDirectory, "player.png"),
                    PlayingImage = System.IO.Path.Combine(Environment.CurrentDirectory, "player.png"),
                    Score = double.Parse(scoreafter.Text),
                    QrType = qrType,
                    SnsType = snsType,
                     TargetUrl = YourTargetUrl.Text
                };
                playingData.Type = "x";
                var json = File.ReadAllText("result1.json");
                var extensionData = JsonConvert.SerializeObject(json);
                playingData.ExtensionData = extensionData;
                var actionQrcode = await _sensingWebClient.PostPlayerData4ActionQrcodeAsync(playingData);
                if (actionQrcode != null)
                {
                    firstQrCode = actionQrcode.QrCodeId;
                    qrCodeImg.Source = UriToImage(actionQrcode.QrCodeUrl);
                }
            }
            else
            {
                //var loginData = new CreatQrcodeInput() { IsSendWeChatMsg = false, SnsType = snsType, QrType = qrType, TargetUrl = YourTargetUrl.Text };
                //var data = await _sensingWebClient.CreateQrCode4LoginAsync(loginData);
                //if (data != null)
                //{
                //    //var qrcode = data.Data;
                //    firstQrCode = data.QrCodeId;
                //    qrCodeImg.Source = UriToImage(data.QrCodeUrl);
                //}
            }
        }

        /// <summary>
        /// 特定用户随机抽奖.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ActivityWinner_Click(object sender, RoutedEventArgs e)
        {
            if (_awards != null && _awards.Count > 0)
            {
                var actionData = new ActionDataInput { ActionId = firstUserAction.Id };
                try
                {
                    var winner = await _sensingWebClient.DoLotteryAwardByAction(actionData);
                    if (winner != null)
                    {
                        if (!winner.Award.AwardImagePath.ToLower().StartsWith("http"));
                        {
                            winner.Award.AwardImagePath = Path.Combine("https://all.api.troncell.com/s", winner.Award.AwardImagePath);
                        }
                        avatorWinnerImg.Source = WebImageToImage(winner.Award.AwardImagePath);
                        AwardProductLable.Content = $"Award Id:{winner.Award.Id},Award Name:{winner.Award.AwardProduct}";
                    }
                }
                catch(Exception ex)
                {
                    AwardProductLable.Content = ex.Message;
                }
            }
        }

        private async void GetRankUsers_Click(object sender, RoutedEventArgs e)
        {
            var rankMax = int.Parse(rankPos.Text);
            var orderby = orderByCBox.SelectedValue as string;
            var first = new Qrcode4UsersInput { QrcodeId = long.Parse(firstQrCode), Sorting = orderby, SkipCount = rankMax-1 };
            var rankUserActions = await _sensingWebClient.GetScanQrCodeUserActions(first);
            if (rankUserActions != null && rankUserActions.Items.Count > 0)
            {
                var topUser = rankUserActions.Items[0];
                avatorRank.Source = UriToImage(topUser.SnsUserInfo.Headimgurl);
                rankMsg.Content = $"Id:{topUser.Id}--OpenId:{topUser.SnsUserInfo.Openid}--Nickname:{topUser.SnsUserInfo.Nickname}";
            }
            else
            {
                rankMsg.Content = "没有这么多人";
            }
        }

        private async void CreateWinUser_Click(object sender, RoutedEventArgs e)
        {
            var awardId = awardIDBox.Text;
            var awardData = new AwardDataInput { AwardId = long.Parse(awardId) };
            var awardUser = await _sensingWebClient.DoLotteryUserByAwardId(awardData);
            if (awardUser != null)
            {
                awardImgy.Source = WebImageToImage(awardUser.Award.AwardImagePath);
                if (!string.IsNullOrEmpty(awardUser.SnsUserInfo.Headimgurl))
                {
                    awardUserImg.Source = WebImageToImage(awardUser.SnsUserInfo.Headimgurl);
                }
                else
                {
                    awardUserImg.Source = new BitmapImage(new Uri(Path.Combine(Environment.CurrentDirectory, "no.jpg")));
                }
                awardTextBlock.Text = $"Award User Id ={awardUser.SnsUserInfo.Openid}, SnsType={awardUser.SnsUserInfo.SnsType.ToString()}" + Environment.NewLine;
                awardTextBlock.Text += $"Award  Id ={awardUser.AwardId}, Product Name={awardUser.Award.AwardProduct}" + Environment.NewLine;
            }
        }

        private void StartActivity_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StopAcitivy_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void PostDataByUserClick(object sender, RoutedEventArgs e)
        {
            var playData = new PlayerActionDataInput
            {
                ActionId = firstUserAction.Id,
                PlayerImage = System.IO.Path.Combine(Environment.CurrentDirectory, "player.jpg"),
                PlayingImage = System.IO.Path.Combine(Environment.CurrentDirectory, "playing.jpg"),
                Score = Convert.ToInt16(score.Text),
                SnsType = EnumSnsType.WeChat,
                QrType = EnumQRStatus.AfterGame,
                IsSendWeChatMsg = true,
                Params = ParamsTbx.Text
            };
            var data = await _sensingWebClient.PostPlayerDataByAction(playData);
            if (data != null)
            {
                var userActionData = new ActionDataInput { ActionId = firstUserAction.Id };
                var userAction = await _sensingWebClient.GetUserActionByIdAsync(userActionData);
                if(userAction != null)
                {
                    PlayingImg.Source = WebImageToImage(userAction.GameImage);
                }
            }
        }

        #endregion

        List<AwardOutput> _awards = null;
        private async void GetDeviceActivityGameInfos_Click(object sender, RoutedEventArgs e)
        {
            var deviceActivityGame = await _sensingWebClient.GetDeviceActivityGameInfoAsync();
            if (deviceActivityGame != null)
            {
                if (deviceActivityGame.Activity != null)
                {
                    activityDetails.Text = $"Activity Name:{deviceActivityGame.Activity.Name}" + Environment.NewLine;
                    activityDetails.Text += $"Wechat Public Id:{deviceActivityGame.Activity.WeChatAppID}" + Environment.NewLine;
                    activityDetails.Text += $"Taobao Seller Id:{deviceActivityGame.Activity.TaobaoSellerID}" + Environment.NewLine;
                    activityDetails.Text += $"Enable Special User:{deviceActivityGame.Activity.IsEnableWhiteUser}" + Environment.NewLine;
                    activityDetails.Text += $"Enable Repeart Award:{deviceActivityGame.Activity.IsAllowedRepeatAward}" + Environment.NewLine;
                }

                //game details
                if (deviceActivityGame.Software != null)
                {
                    gameInfoDetails.Text = $"Game Name:{deviceActivityGame.Software.Name}" + Environment.NewLine;
                    gameInfoDetails.Text += $"Game Language:{deviceActivityGame.Software.Language}" + Environment.NewLine;
                    gameInfoDetails.Text += $"Game EnvType:{deviceActivityGame.Software.EnvType}" + Environment.NewLine;
                    gameInfoDetails.Text += $"Game Version:{deviceActivityGame.Software.VersionNumber}" + Environment.NewLine;
                }

                //game details
                //gameInfoDetails.Text = $"ActivityGame Name:{deviceActivityGame.Name}" + Environment.NewLine;
                activityGameDetails.Text += $"WeChatAuthorizationType:{deviceActivityGame.WeChatAuthorizationType.ToString()}" + Environment.NewLine;
                activityGameDetails.Text += $"TaobaoAuthorizationType:{deviceActivityGame.TaobaoAuthorizationType}" + Environment.NewLine;
                activityGameDetails.Text += $"DeviceId:{deviceActivityGame.DeviceId}" + Environment.NewLine;
                activityGameDetails.Text += $"DeviceName:{deviceActivityGame.DeviceId}" + Environment.NewLine;
            }

            //var awards = await _sensingWebClient.GetAwardsAsync();
            //if (awards != null)
            //{
            //    _awards = awards;
            //    awardDetails.Text += $"Awards Count :{awards.Count}" + Environment.NewLine;
            //    foreach (var award in awards)
            //    {
            //        awardDetails.Text += $"Award:{award.Name}--AwardId:{award.Id}----{award.AwardProduct}" + Environment.NewLine;
            //    }
            //}
        }

        private async void TagRecommendBtn_Click(object sender, RoutedEventArgs e)
        {
            var tagInput = new GetTagAndRecommendsBySubKeyInput();
            tagInput.Age = Convert.ToInt32(AgeTBox.Text);
            tagInput.Happiness = Convert.ToInt32(HappinessTBox.Text);
            tagInput.BeautyScore = Convert.ToInt32(HappinessTBox.Text);
            tagInput.Gender =  GenderTBox.Text;

            var tagRecommend = await _sensingWebClient.GetTagRecommends(tagInput);
            if(tagRecommend != null)
            {
                GoodsByFaceTxt.Text = $"Tag's Count = {tagRecommend.Tags.Count}" + Environment.NewLine;
                GoodsByFaceTxt.Text += $"Recommend's Count = {tagRecommend.Recommends.Count}" + Environment.NewLine;
            }
        }

        private async void RegisterMemberFace_Click(object sender, RoutedEventArgs e)
        {
            var myFace = new UserFaceDataInput();
            var fileName = "wu.jpg";
            var faceImagePath = System.IO.Path.Combine(Environment.CurrentDirectory, fileName);

            var bytes = File.ReadAllBytes(faceImagePath);
            myFace.FaceBytes = bytes;
            myFace.SecurityKey = "da4e335581084497a9faae269727fa45";
            myFace.OpenId = OpenIdTBox.Text.Trim();
            myFace.SnsType = EnumSnsType.WeChat;

            var data = await _sensingWebClient.RegisterFaceMember(myFace);
            if (data != null)
            {
                MatchMsg.Text = "RegisterFaceMember Successfully" + Environment.NewLine;
                MatchMsg.Text += data;
            }
            else
            {
                MatchMsg.Text = "failed" + Environment.NewLine;
            }
        }

        private async void UserByFaceBtn_Click(object sender, RoutedEventArgs e)
        {
            var faces = new FaceInput();
            var fileName = "wu.jpg";
            var faceImagePath = System.IO.Path.Combine(Environment.CurrentDirectory, fileName);

            var bytes = File.ReadAllBytes(faceImagePath);
            faces.Face = bytes;

            //File.WriteAllBytes("wu.txt", bytes);

            var myFace = new FaceDataInput();
            myFace.FaceBytes = bytes;
            var data = await _sensingWebClient.QueryUserByFace(myFace);
            if (data != null)
            {
                MatchMsg.Text = "QueryUserByFace Successfully" + Environment.NewLine;
                MatchMsg.Text += data;
            }
            else
            {
                MatchMsg.Text = "failed" + Environment.NewLine;
            }
        }

        private async void GetTimelines_Click(object sender, RoutedEventArgs e)
        {
            var timelines = await _sensingWebClient.GetAdAndAppTimelinesInAWeek(DateTime.Now);
            if (timelines != null)
            {
                AdsMessage.Text = "Query GetAdAndAppTimelinesInAWeek Successfully" + Environment.NewLine;
                AdsMessage.Text += "Timeline count is " + timelines.Count;
            }
            else
            {
                AdsMessage.Text = "failed" + Environment.NewLine;
            }
        }
    }
}
