﻿namespace WebStore.Contractors
{
    public class PostamateDeliveryService : IDeliveryService
    {
        private static IReadOnlyDictionary<string, string> cities = new Dictionary<string, string>
        {
            {"1", "Москва"},
            {"2", "Санкт-Петербург"}
        };

        private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> postamates = new Dictionary<string, IReadOnlyDictionary<string, string>>
        {
            {
                "1", 
                new Dictionary<string, string> 
                {
                    {"1","Казанский вокзал"},
                    {"2","Охотный ряд"},
                    {"3","Савёловский рынок"}
                }
            },
            {
                "2",
                new Dictionary<string, string>
                {
                    {"4","Московский вокзал"},
                    {"5","Гостиный двор"},
                    {"6","Петропавловкая крепость"}
                }
            },
        };

        public string UniqueCode => "Postamate";

        public string Title => "Доставка через постаматы";


        public OrderDelivery CreateDelivery(Form form)
        {
            if (form.UniqueCode != UniqueCode || !form.IsFinal)
                throw new InvalidOperationException("Invalide form");

            var cityId = form.Fields.Single(field => field.Name == "city").Value;
            var cityName = cities[cityId];
            var postamateId = form.Fields.Single(field => field.Name == "postamate").Value;
            var postamateName = postamates[cityId][postamateId];

            var fields = new Dictionary<string, string>
            {
                {nameof(cityId), cityId},
                {nameof(cityName), cityName},
                {nameof(postamateId), postamateId},
                {nameof(postamateName), postamateName}
            };

            var description = $"Город: {cityName}\nПостамат: {postamateName}";

            return new OrderDelivery(UniqueCode, description, 150m, fields);
        }

        public Form CreateForm(Order order)
        {
            if(order == null) 
                throw new ArgumentNullException(nameof(order));

            return new Form(UniqueCode, order.Id, 1, false, new[]
            {
                new SelectedField("Город", "city", "1", cities),
            });
        }

        public Form MoveNext(int orderId, int step, IReadOnlyDictionary<string, string> values)
        {
            if(step == 1)
            {
                if (values["city"] == "1")
                {
                    return new Form(UniqueCode, orderId, 2, false, new Field[]
                    {
                        new HiddenField("Город", "city", "1"),
                        new SelectedField("Постамат", "postamate", "1", postamates["1"])
                    });
                }
                else if (values["city"] == "2")
                {
                    return new Form(UniqueCode, orderId, 2, false, new Field[]
                    {
                        new HiddenField("Город", "city", "2"),
                        new SelectedField("Постамат", "postamate", "4", postamates["2"])
                    });
                }
                else
                {
                    throw new InvalidOperationException("Invalide postamate");
                }
            }
            else if(step == 2)
            {
                return new Form(UniqueCode, orderId, 3, true, new Field[]
                {
                    new HiddenField("Город", "city", values["city"]),
                    new HiddenField("Постамат", "postamate", values["postamate"]),
                });
            }
            else 
            {
                throw new InvalidOperationException("Invalid postamate");
            }
        }
    }
}
