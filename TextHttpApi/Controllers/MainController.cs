using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using static TextHttpApi.DataCore;

namespace TextHttpApi.Controllers {
	public class MainController : Controller {
		private readonly IHttpClientFactory _httpClientFactory;
		public MainController(IHttpClientFactory httpClientFactory) {
			//依赖注入
			_httpClientFactory = httpClientFactory;
		}

		public IActionResult Index() =>
			//return View();
			Redirect("/api");
		[Route("api")]
		public async Task<string> Api() {
			Random ran = new ((int)DateTime.Now.Ticks);
			DataFile.ApiConfig ac=DataCore.DataFiles.apiConfig;//引入本地
			int ranItemWei=ran.Next(ac.AllWeight);//使用所有项目的权重和进行随机
			int range = 0;
			foreach(DataFile.ApiConfig.ItemModel item in ac.Items) {
				range += item.Weight;
				if (ranItemWei < range) {
					int ranCon=ran.Next(item.Content.Length);
					string outputValue;
					switch (item.Type) {
						case DataFile.ApiConfig.ItemModel.ItemType.Text:
							outputValue = item.Content[ranCon];
							break;
						case DataFile.ApiConfig.ItemModel.ItemType.ApiUrl: {
								async Task<string> GetData(string url) {
									try {
										var response = await _httpClientFactory.CreateClient().GetAsync(url);
										return response.IsSuccessStatusCode
											? await response.Content.ReadAsStringAsync()
											: $"请求失败，错误码：{response.StatusCode}";
									} 
									catch {
										return $"请求失败，错误码：500";
									}
								}
								outputValue =await GetData(item.Content[ranCon]);
								break;
							}
						case DataFile.ApiConfig.ItemModel.ItemType.LocalFile: {
								int theLeng = item.TextList[ranCon].Length;
								if (theLeng != 0) {
									int ranTextList = ran.Next(theLeng);
									outputValue = item.TextList[ranCon][ranTextList];
								}
								else 
									outputValue="";
								break; 
							}
						default:
							outputValue = "未知错误";break;
					}
					return string.Format(item.Format,outputValue);
				}
			}
			return null!;
		}
	}
}
