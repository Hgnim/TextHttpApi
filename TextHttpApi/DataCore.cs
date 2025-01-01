namespace TextHttpApi {
	internal struct PInfo {
		internal const string version = "1.0.0.20250101";
		internal const string version_addV = $"V{version}";
		internal const string copyright = "Copyright (C) 2025 Hgnim, All rights reserved.";
		internal const string githubUrl = "https://github.com/Hgnim/TextHttpApi";
		internal const string githubUrl_addHead = $"Github: {githubUrl}";
	}
	internal struct FilePath 
		{
		internal const string dataDir = "tha_data/";
		internal const string textFile = dataDir+ "text.txt";
	}
	internal struct DataCore {
		internal static List<string> allText=[];
	}
}
