using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenGLTests.src.Util
{
    internal class EquipmentModel
    {
        public string Slot { get; set; }
        public GLCoordinate Location { get; set; }
    }

    static class JsonCoordinateReader
    {
        public static Dictionary<string, GLCoordinate> GetEquipmentLocations()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader("EquipmentLocations.json");
                var fileContents = reader.ReadToEnd();
                JObject res = JsonConvert.DeserializeObject<JObject>(fileContents, new JsonSerializerSettings());
                JArray models = (JArray) res["Equipment Locations"];
                List<EquipmentModel> em = models.ToObject<List<EquipmentModel>>();
                Dictionary<string, GLCoordinate> slotLocMap = new Dictionary<string, GLCoordinate>();
                foreach (var e in em)
                {
                    slotLocMap.Add(e.Slot, e.Location);
                }

                return slotLocMap;

            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return null;
        }
    }
}
