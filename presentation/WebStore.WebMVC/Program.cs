using WebStore.Contractors;
using WebStore.Memory;
using WebStore.Web.Contractors;
using WebStore.YandexKassa;

namespace WebStore.WebMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true; //������ � ���� ������ �� �������
                options.Cookie.IsEssential = true; //�������������� ����, �� ������� �������� ������������
            });

            builder.Services.AddSingleton<IBookRepository, BookRepository>();
            builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
            builder.Services.AddSingleton<IDeliveryService, PostamateDeliveryService>();
            builder.Services.AddSingleton<IPaymentService, CashPaymentService>();
            builder.Services.AddSingleton<IPaymentService, YandexKassaPaymentService>();
            builder.Services.AddSingleton<IWebContractorService, YandexKassaPaymentService>();
            builder.Services.AddSingleton<BookService>();
            builder.Services.AddSingleton<NotificationService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapAreaControllerRoute(
                name: "yandex.kassa",
                areaName: "YandexKassa",
                pattern: "YandexKassa/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}