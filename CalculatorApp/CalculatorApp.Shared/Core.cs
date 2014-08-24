using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.Storage;
using Newtonsoft.Json;
using System.Threading;

namespace CalculatorApp
{
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
                var json = JsonConvert.SerializeObject(s);
                await FileIO.WriteTextAsync(file, json);
            }
        }

        static SemaphoreSlim _mutex = new SemaphoreSlim(1);

        public static async void Save(Sheet s)
        {
            if (s == null) return;

            if (SheetsFolder == null)
                SheetsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Sheets", CreationCollisionOption.OpenIfExists);

            var file = await SheetsFolder.CreateFileAsync(s.ID, CreationCollisionOption.OpenIfExists);
            
            var json = JsonConvert.SerializeObject(s);
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
                if(json != "" )Sheets.Add(JsonConvert.DeserializeObject<Sheet>(json));
            }
        }
    }
}
