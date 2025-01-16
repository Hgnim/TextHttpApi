using System.Diagnostics.CodeAnalysis;
using System.Xml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static TextHttpApi.DataCore;

namespace TextHttpApi {
	internal struct PInfo {
		internal const string version = "1.1.0.20250116";
		internal const string version_addV = $"V{version}";
		internal const string copyright = "Copyright (C) 2025 Hgnim, All rights reserved.";
		internal const string githubUrl = "https://github.com/Hgnim/TextHttpApi";
		internal const string githubUrl_addHead = $"Github: {githubUrl}";
	}
	internal struct FilePath {
		internal const string dataDir = "tha_data/";
		internal const string configFile = $"{dataDir}config.yml";
		internal const string apiConfigFile = $"{dataDir}api_config.yml";
	}
	internal struct DataCore {
		internal struct DataFiles {
			static internal DataFile.ConfigFile config=new();
			static internal DataFile.ApiConfig apiConfig = new() {
				Items = [
					new(){
						Type=DataFile.ApiConfig.ItemModel.ItemType.Text,
						Weight=15,
						Format="[example] {0}",
						Content=["text-1","text-2","text-3","text-4"]
					},
					new(){
						Type=DataFile.ApiConfig.ItemModel.ItemType.ApiUrl,
						Weight=3,
						Content=["https://example.com/", "https://example.com"]
					},
					new(){
						Type=DataFile.ApiConfig.ItemModel.ItemType.ApiUrl,
						Weight=5,
						Format="output: {0}",
						Content=["https://example.org/"]
					},
					new(){
						Type=DataFile.ApiConfig.ItemModel.ItemType.LocalFile,
						Weight=10,
						Format="[File] {0}",
						Content=["/text.txt"]
					}
					],
			};
		}
	}
	public struct DataFile {
		/// <summary>
		/// 将配置数据保存至配置文件中
		/// </summary>
		internal static void SaveData() {
			ISerializer yamlS = new SerializerBuilder()
				.WithNamingConvention(UnderscoredNamingConvention.Instance)
				.Build();

			File.WriteAllText(FilePath.configFile, yamlS.Serialize(DataFiles.config));
			File.WriteAllText(FilePath.apiConfigFile, yamlS.Serialize(DataFiles.apiConfig));
		}
		/// <summary>
		/// 读取数据文件并将数据写入实例中
		/// </summary>
		internal static void ReadData() {
			IDeserializer yamlD = new DeserializerBuilder()
				.WithNamingConvention(UnderscoredNamingConvention.Instance)
					.Build();
			if(File.Exists(FilePath.configFile))
			DataFiles.config = yamlD.Deserialize<ConfigFile>(File.ReadAllText(FilePath.configFile));
			if(File.Exists(FilePath.apiConfigFile))
			DataFiles.apiConfig = yamlD.Deserialize<ApiConfig>(File.ReadAllText(FilePath.apiConfigFile));
		}
		public struct ConfigFile {
			public struct WebsiteModel {
				public required string Addr { get; set; }
				private string urlRoot;
				public required string UrlRoot {
					readonly get => urlRoot;
					//对UrlRoot的值进行格式化
					set {
						string urlRoot_;
						if (value == "/") urlRoot_ = "";//urlRoot不能只包含单独的斜杠
						else if (value == "") { urlRoot_ = value; }//如果为空则直接输出
						else {
							if (value[..1] == "/")
								urlRoot_ = value;
							else//如果开头没有斜杠则加上斜杠
								urlRoot_ = "/" + value;
							if (urlRoot_.Substring(urlRoot_.Length - 1, 1) == "/")//如果末尾包含斜杠则去掉
								urlRoot_ = urlRoot_[..(urlRoot_.Length - 1)];
						}
						urlRoot = urlRoot_;
					}
				}

				public required int Port { get; set; }
				public required bool UseHttps { get; set; }
				public required bool UseXFFRequestHeader { get; set; }
				/* UseXFFRequestHeader
				 * 后续用于获取客户端IP时可能会用到的选项
				 string GetClientIP() {
					if (Config.UseXFFRequestHeader)
						return Request.Headers["X-Forwarded-For"].ToString();
					else
						return HttpContext.Connection.RemoteIpAddress!.ToString();
				 }
				 */
			}
			public required WebsiteModel Website { get; set; }
			//public required int DebugOutput { get; set; }
			public required bool UpdateConfig { get; set; }

			[SetsRequiredMembers]
			public ConfigFile() {
				Website = new() {
					Addr = "*",
					UrlRoot = "/",
					Port = 80,
					UseHttps = false,
					UseXFFRequestHeader = false
				};
				//DebugOutput = 0;
				UpdateConfig = false;
			}
		}
		public class ApiConfig {
			public class ItemModel {
				public enum ItemType {
					Text, ApiUrl,LocalFile
				}
				public required ItemType Type { get; set; }
				/// <summary>
				/// 权重，用于计算随机几率
				/// </summary>
				public required int Weight { get; set; }
				[YamlMember(ApplyNamingConventions = false)]
				public string Format { get; set; } = "{0}";
				/// <summary>
				/// 内容
				/// </summary>
				public required string[] Content { get; set; }

				private string[][]? textList = null;
				[YamlIgnore]
				internal string[][] TextList {
					get {
						if (textList == null) {
							List<string[]> strsL = [];
							foreach (var con in Content) {
								List<string> strL = [];
								try {
									using StreamReader sr = new(Path.Combine( FilePath.dataDir+con));
									string? rStr;
									do {
										rStr = sr.ReadLine();
										if (rStr != null)
											strL.Add(rStr);
									} while (rStr != null);
								} catch { }
								strsL.Add([.. strL]); 
							}
							textList= [.. strsL];
						}
						return textList;
					}
				}
			}
			public required ItemModel[] Items { get; set; }
			private int allWeight = -1;
			[YamlIgnore]
			internal int AllWeight { get {
					if(allWeight==-1) {
						allWeight = 0;
						foreach(ItemModel item in Items) {
							allWeight += item.Weight;
						}
					}
						return allWeight;
				} }
		}
	}
}
