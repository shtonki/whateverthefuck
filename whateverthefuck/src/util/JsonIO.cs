using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using whateverthefuck.src.model;
using whateverthefuck.src.model.entities;

namespace whateverthefuck.src.util
{
    public class JsonIO
    {
        private static JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
        };

        public static List<GameEntity> LoadEntitiesFromFile(string filename)
        {
            if (!File.Exists(filename + ".json"))
            {
                return new List<GameEntity>();
            }

            List<GameEntity> vars = new List<GameEntity>();
            JObject entitiesList = ReadFromJsonFile<JObject>(filename + ".json");
            JArray entities = entitiesList["$values"] as JArray;
            foreach (var entity in entities)
            {
                try
                {
                    string sType = entity["$type"].ToString();
                    Type entityType = Type.GetType(sType);
                    dynamic xd = JsonConvert.DeserializeObject(entity.ToString(), entityType);
                    vars.Add(xd);
                }
                catch (Exception e)
                {
                    Logging.Log("fucked up deserializing " + entity.GetType() + e);
                }
            }

            return vars;
        }

        public static T ConvertToString<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        /// <summary>
        /// Writes the given object instance to a Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite, typeof(T), Settings);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        public static string ConvertToJson<T>(T objectToWrite) where T : new()
        {
            try
            {
                return JsonConvert.SerializeObject(objectToWrite, typeof(T), Settings);
            }
            catch(Exception e)
            {
                Logging.Log("Could not serialize " + objectToWrite + ": " + e);
                return "";
            }
        }

        /// <summary>
        /// Reads an object instance from an Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the Json file.</returns>
        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                var v = JsonConvert.DeserializeObject<T>(fileContents, Settings);
                return v;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
    }
}