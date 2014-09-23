using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.Storage;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using System.Linq;

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

        public static async void DeleteSheet(Sheet s)
        {
            StorageFile file = await SheetsFolder.GetFileAsync(s.ID);
            await file.DeleteAsync();
        }

        public static bool firstRun()
        {
            object k;
            if (!ApplicationData.Current.RoamingSettings.Values.TryGetValue("firstRun2", out k))
            {
                ApplicationData.Current.RoamingSettings.Values["firstRun2"] = true;
                return true;
            }
            return false;
        }

        public async static void DownloadCurrencies()
        {
            string url = "http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            string xml = await response.Content.ReadAsStringAsync();
            StorageFile xmlFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("currency.xml" , CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(xmlFile, xml);
        }

        // TODO : Finish this damn table
        public static Dictionary<string, List<string>> Aliases = new Dictionary<string, List<string>>()
        {
            {"USD" , new List<string>{"dollar" , "dollars" , "bucks"}},
            {"JPY" , new List<string>{"Yen"}},
            {"BGN" , new List<string>{"lev"}},
            {"CZK" , new List<string>{"koruna"}},
            {"DKK" , new List<string>{"krone"}},
            {"GBP" , new List<string>{"pounds"}},
            {"HUF" , new List<string>{"forint"}},
            {"LTL" , new List<string>{"litas"}},
            {"PLN" , new List<string>{"złoty"}},
            {"RON" , new List<string>{"leu"}},
            {"SEK" , new List<string>{"krona"}},
            {"CHF" , new List<string>{"franc"}},
            {"NOK" , new List<string>{"norwegian krone"}},
            {"HRK" , new List<string>{"kruna"}},
            {"RUB" , new List<string>{"ruble"}},
            {"TRY" , new List<string>{"lira"}},
            {"AUD" , new List<string>{"australian dollars"}},
            {"BRL" , new List<string>{"real"}},
            {"CAD" , new List<string>{"canadian dollars"}},
            {"CNY" , new List<string>{"yuan"}},
            {"HKD" , new List<string>{"hong kong dollars"}},
            {"IDR" , new List<string>{"rupiah"}},
            {"ILS" , new List<string>{"new sheqel"}},
            {"INR" , new List<string>{"pounds"}},
            {"KRW" , new List<string>{"pounds"}},
            {"MXN" , new List<string>{"pounds"}},
            {"MYR" , new List<string>{"forint"}},
            {"NZD" , new List<string>{"pounds"}},
            {"PHP" , new List<string>{"pounds"}},
            {"SGD" , new List<string>{"pounds"}},
            {"THB" , new List<string>{"pounds"}},
            {"ZAR" , new List<string>{"pounds"}},
        };

        public static Dictionary<string, double> data = new Dictionary<string, double>();
        public async static void GetCurrencies()
        {
            data.Clear();
            try
            {
                StorageFile xmlFile = await ApplicationData.Current.LocalFolder.GetFileAsync("currency.xml");
                string xml = await FileIO.ReadTextAsync(xmlFile);
                XmlDocument document = await XmlDocument.LoadFromFileAsync(xmlFile);
                var itemss = document.GetElementsByTagName("Cube");
                var items = from currency in document.GetElementsByTagName("Cube") where currency.Attributes.GetNamedItem("currency") != null select currency;
                foreach (var item in items)
                {
                    data.Add(item.Attributes.GetNamedItem("currency").NodeValue as string, double.Parse(item.Attributes.GetNamedItem("rate").NodeValue as string));
                }
            }
            catch
            {
                DownloadCurrencies();
            }
        }
    }

}
