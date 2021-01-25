using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace NoticeBoard.Helpers
{
    public class CustomSerializer<ObType> where ObType : class
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
        };
        public async Task Serialize(ObType ob, string fileName)
        {
            
            using (FileStream fs = File.Create(fileName))
            {
                await JsonSerializer.SerializeAsync(fs, ob, options: options);
            }
        }
        public async Task<ObType> deSerialize(string fileName)
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                return await JsonSerializer.DeserializeAsync<ObType>(fs,options:options);
            }
        }
    }
}
