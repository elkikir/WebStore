using System.Text;
using WebStore.WebMVC.Models;

namespace WebStore.WebMVC
{
    public static class SessionExtentions
    {
        private const string key = "Cart";

        public static void RemoveCart(this ISession session)
        {
            session.Remove(key);
        }
        public static void Set(this ISession session, Cart value)
        {
            if (value == null) return;

            using(var stream = new MemoryStream())
            using(var writer = new BinaryWriter(stream, Encoding.UTF8, true)) 
            {
                writer.Write(value.OrderId);
                writer.Write(value.TotalCount);
                writer.Write(value.TotalPrice);

                session.Set(key, stream.ToArray());
            }
        }

        public static bool TryGetCart(this ISession session, out Cart value)
        {
            if(session.TryGetValue(key, out byte[] buffer))
            {
                using (var stream = new MemoryStream(buffer))
                using(var reader =  new BinaryReader(stream, Encoding.UTF8, true)) 
                {
                    var orderId = reader.ReadUInt32();
                    var totalCount = reader.ReadUInt32();
                    var totalPrice = reader.ReadDecimal();

                    value = new Cart((int)orderId)
                    {
                        TotalCount = (int)totalCount,
                        TotalPrice = (decimal)totalPrice
                    };

                    return true;
                }
            }
            value = null;
            return false;
        }
    }
}
