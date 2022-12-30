using JsonSubTypes;
using Newtonsoft.Json;
using SensingAds.CustomAds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SensingAds.ViewBanner.AdsItem
{
    public class CustomAd : BaseAd
    {
        public BasicData BasicData { get; set; }
        public List<ViewBuilder> ItemList { get; set; }

        public override Banner CreateBanner()
        {
            Canvas rootPanel = new Canvas();
            if (ItemList != null)
            {
                foreach(var viewBuilder in ItemList)
                {
                    FrameworkElement childView = viewBuilder.Build();
                    if(childView is Canvas)
                    {
                        rootPanel = (Canvas)childView;
                    }
                    else
                    {
                        rootPanel.Children.Add(childView);
                    }
                }
            }
            CustomViewBanner banner = new CustomViewBanner(rootPanel,ItemList,TimeSpan);
            banner.Title = this.Name;
            banner.Id = Id;
            banner.Transition = Transition;
            return banner;
        }

        public static CustomAd  Parse(string json, int id, string name, int timeSpan)
        {
            try
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(JsonSubtypesConverterBuilder
                                .Of(typeof(ViewBuilder), "type")
                                .RegisterSubtype(typeof(CavnasBuilder), "canvas")
                                .RegisterSubtype(typeof(ImageBuilder), "image")
                                .RegisterSubtype(typeof(TextBuilder), "text")
                                .RegisterSubtype(typeof(MediaBuilder), "media")
                                .RegisterSubtype(typeof(QrcodeBuilder), "qrcode")
                                .RegisterSubtype(typeof(SwiperBuilder), "swiper")
                                .RegisterSubtype(typeof(H5Builder), "html")
                                .RegisterSubtype(typeof(ProductBuilder),"product")
                                .SetFallbackSubtype(typeof(UnknownBuilder))
                                .SerializeDiscriminatorProperty()
                                .Build());
                var ad = JsonConvert.DeserializeObject<CustomAd>(json, settings);
                ad.Id = id;
                ad.Name = name;
                ad.TimeSpan = timeSpan;
                return ad;
            }
            catch (Exception ex)
            {
                return new CustomAd
                {
                    Id = id,
                    Name = name,
                    TimeSpan = timeSpan,
                    ItemList = new List<ViewBuilder>
                    {
                        new TextBuilder()
                        {
                            textContent = ex.Message
                        }
                    } 
                };
            }
        }


        public override IEnumerable<string> ExtractLinks()
        {
            foreach (var childLink in ItemList.OfType<ILink>())
            {
                foreach (var link in childLink.ExtractLinks())
                {
                    yield return link;
                }
            }
        }

    }

    public class BasicData
    {
        public string Name { get; set; }
    }
}

//foreach (var item in ItemList)
//{
//    var type = item.Value<string>("type");
//    if (type == "canvas")
//    {
//        Items.Add(item.ToObject<CanvasItem>());
//    }
//    else if (type == "button")
//    {
//        Items.Add(item.ToObject<ButtonItem>());
//    }
//    else if (type == "image")
//    {
//        Items.Add(item.ToObject<ImageItem>());
//    }
//    else if (type == "swiper")
//    {
//        Items.Add(item.ToObject<SwiperItem>());
//    }
//    else if (type == "html")
//    {
//        Items.Add(item.ToObject<H5Item>());
//    }
//    else if (type == "media")
//    {
//        Items.Add(item.ToObject<VideoItem>());
//    }
//    else if (type == "text")
//    {
//        Items.Add(item.ToObject<LabelItem>());
//    }
//    else if (type == "qrcode")
//    {
//        var qrcodeItem = item.ToObject<QrcodeItem>();
//Items.Add(qrcodeItem);
//    }
//}

