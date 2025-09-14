using Newtonsoft.Json;
using System.IO;

namespace CharacterCreation
{
    public static class CharacterStorage
    {
        static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto // важно для полиморфных списков (наследников)
        };

        public static string ToJson(Character c)
        {
            var dto = CharacterMapper.ToDto(c);
            return JsonConvert.SerializeObject(dto, Settings);
        }

        public static void SaveToFile(Character c, string path)
        {
            File.WriteAllText(path, ToJson(c));
        }

        public static CharacterDto LoadDtoFromJson(string json)
        {
            return JsonConvert.DeserializeObject<CharacterDto>(json, Settings);
        }

        public static CharacterDto LoadDtoFromFile(string path)
        {
            var json = File.ReadAllText(path);
            return LoadDtoFromJson(json);
        }

        public static void LoadInto(Character c, string json)
        {
            var dto = LoadDtoFromJson(json);
            CharacterMapper.ApplyDto(c, dto);
        }
    }
}

