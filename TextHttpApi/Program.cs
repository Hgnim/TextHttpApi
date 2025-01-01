using static TextHttpApi.PInfo;
using static TextHttpApi.FilePath;
using static TextHttpApi.DataCore;

namespace TextHttpApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
			Console.WriteLine(
@$"欢迎使用文本调用HttpApi。
版本: {version}
{copyright}
{githubUrl_addHead}
禁止将该服务用于任何违法用途。因任何原因导致的任何后果都将由用户承担，开发者对此不承担任何责任。"
);

            if(!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
			if (File.Exists(textFile)) {
				using StreamReader sr = new(textFile);
				{
					string? rStr;
					do {
						rStr = sr.ReadLine();
						if (rStr != null)
							allText.Add(rStr);
					} while (rStr != null);
				}
			}
			else {
				File.Create(textFile).Close();
				Console.WriteLine("已在当前目录生成配置文件，完成配置后再次启动服务端。");
				return;
			}

			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();
			builder.WebHost.UseUrls("http://127.0.0.1:18008");
			var app = builder.Build();

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Main}/{action=Index}"); // /{id?}");

			app.Run();
		}
    }
}
