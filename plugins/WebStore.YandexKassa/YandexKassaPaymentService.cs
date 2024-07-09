using WebStore.Contractors;
using WebStore.Web.Contractors;

namespace WebStore.YandexKassa
{
    public class YandexKassaPaymentService : IPaymentService, IWebContractorService
    {
        public string UniqueCode => "YandexKassa";

        public string Title => "Оплата банковской картой";

        public string GetUri => "/YandexKassa/";

        public Form CreateForm(Order order)
        {
            return new Form(UniqueCode, order.Id, 1, true, new Field[0]);
        }

        public OrderPayment GetPayment(Form form)
        {
            return new OrderPayment(UniqueCode, "Опалта картой", new Dictionary<string, string> { });
        }

        public Form MoveNext(int orderId, int step, IReadOnlyDictionary<string, string> values)
        {
            return new Form(UniqueCode, orderId, 2, true, new Field[0]);
        }
    }
}
