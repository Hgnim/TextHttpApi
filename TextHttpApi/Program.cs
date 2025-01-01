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
@$"��ӭʹ���ı�����HttpApi��
�汾: {version}
{copyright}
{githubUrl_addHead}
��ֹ���÷��������κ�Υ����;�����κ�ԭ���µ��κκ���������û��е��������߶Դ˲��е��κ����Ρ�"
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
				Console.WriteLine("���ڵ�ǰĿ¼���������ļ���������ú��ٴ���������ˡ�");
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
