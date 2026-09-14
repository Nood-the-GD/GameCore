using Core.FileUtil;
using Newtonsoft.Json;

namespace Core.Json
{
    public static class JsonSaveLoad
    {
        public static void SaveToJson(object obj, string savePath)
        {
            FileUtility.WriteToPath(savePath, JsonConvert.SerializeObject(obj));
        }

        public static T LoadFromSavePath<T>(string savePath)
        {
            return JsonConvert.DeserializeObject<T>(FileUtility.ReadTextFromPath(savePath));
        }

        public static void QuickSaveToJson(object obj, string fileName)
        {
            FileUtility.WriteToPath(fileName, JsonConvert.SerializeObject(obj));
        }

        public static T QuickLoadFromJson<T>(string fileName) where T : new()
        {
            var json = FileUtility.ReadTextFromPath(fileName);
            if (json != null)
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                return new T();
            }
        }

        public static T QuickLoadFromJson<T>(string fileName, T @default) where T : new()
        {
            var json = FileUtility.ReadTextFromPath(fileName);
            if (json != null)
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                return @default;
            }
        }
    }
}
