namespace Sensing.SDK.Contract
{
    public class ShoppingCartItem
    {
        public string ItemId  { get; set; }
        public string SkuId { get; set; }
        public int ItemCount { get; set; }
    }

    public class ShoppingCartInput
    {
        public string Item_ids { get; set; }
        public bool Longterm { get; set; }
    }

    public class CartQrcodeOutput
    {
        public string ShortUrl { get; set; }
        public string ShortImgUrl { get; set; }
    }
}
