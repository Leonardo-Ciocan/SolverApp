using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.Storage;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace CalculatorApp
{
    public class ContractResolver : DefaultContractResolver
    {
        public override JsonContract ResolveContract(Type type)
        {
            //check if type is ObservableCollection
            if (type.GetTypeInfo().IsGenericType
                     && type.GetTypeInfo().GetGenericTypeDefinition()
                              == typeof(ObservableCollection<>))
            {
                //use list as default contract
                var c = (JsonArrayContract)
                          base.ResolveContract(typeof(List<>)
                              .MakeGenericType(type.GenericTypeArguments[0]));
                //use Activator to create instance
                c.DefaultCreator = () => Activator.CreateInstance(type);
                return c;
            }
            else return base.ResolveContract(type);
        }
    }

    public class Core
    {

        public static ObservableCollection<Sheet> Sheets = new ObservableCollection<Sheet>();

        public static StorageFolder SheetsFolder;



        public static async void SaveAll()
        {
            if(SheetsFolder == null) 
                SheetsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Sheets", CreationCollisionOption.OpenIfExists);


            foreach (Sheet s in Sheets)
            {
                var file = await SheetsFolder.CreateFileAsync(s.ID, CreationCollisionOption.OpenIfExists);
                JsonSerializerSettings settings = new JsonSerializerSettings { ContractResolver = new ContractResolver() };
                var json = JsonConvert.SerializeObject(s , Formatting.Indented , settings);
                await FileIO.WriteTextAsync(file, json);
            }
        }

        static SemaphoreSlim _mutex = new SemaphoreSlim(1);

        static JsonSerializerSettings settings = new JsonSerializerSettings { ContractResolver = new ContractResolver() };
        public static async void Save(Sheet s)
        {
            if (s == null) return;

            if (SheetsFolder == null)
                SheetsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Sheets", CreationCollisionOption.OpenIfExists);

            var file = await SheetsFolder.CreateFileAsync(s.ID, CreationCollisionOption.OpenIfExists);
            var json = JsonConvert.SerializeObject(s, Formatting.Indented, settings);
            await _mutex.WaitAsync();
              try
              {
                  await FileIO.WriteTextAsync(file, json);
              }
              finally
              {
                _mutex.Release();
              }

        }

        public static async void LoadAll()
        {
            if (SheetsFolder == null)
                SheetsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Sheets", CreationCollisionOption.OpenIfExists);


            var files = await SheetsFolder.GetFilesAsync();
            foreach (var file in files)
            {
                var json = await FileIO.ReadTextAsync(file);
                if(json != "" )Sheets.Add(JsonConvert.DeserializeObject<Sheet>(json , settings));
            }
        }
    }

}
