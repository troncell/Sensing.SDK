

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Sensing.SDK.AdsItems
{
    public class CustomAd 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public BasicData BasicData { get; set; }
        public JObject[] ItemList { get; set; }
        public List<BaseItem> Items { get; set; }

        public void Initialize()
        {
            Items = new List<BaseItem>();
            foreach (var item in ItemList)
            {
                var type = item.Value<string>("type");
                if(type == "canvas")
                {
                    Items.Add(item.ToObject<CanvasItem>());
                }
                else if(type == "button")
                {
                    Items.Add(item.ToObject<ButtonItem>());
                }
                else if(type == "image")
                {
                    Items.Add(item.ToObject<ImageItem>());
                }
                else if(type == "swiper")
                {
                    Items.Add(item.ToObject<SwiperItem>());
                }
                else if (type == "html")
                {
                    Items.Add(item.ToObject<H5Item>());
                }
                else if (type == "media")
                {
                    Items.Add(item.ToObject<VideoItem>());
                }
                else if (type == "text")
                {
                    Items.Add(item.ToObject<LabelItem>());
                }
                else if (type == "product")
                {
                    Items.Add(item.ToObject<ProductItem>());
                }
            }
        }
            

        public  IEnumerable<string> ExtractLinks()
        {
            foreach (var childLink in Items.OfType<ILink>())
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
