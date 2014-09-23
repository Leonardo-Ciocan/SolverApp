using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace CalculatorApp
{
    public class TileManager
    {
        public async static void SaveAndPin(FrameworkElement tile , UIElement smallTile, String ID)
        {
            SecondaryTile pinTile = null;
            try
            {

                if (smallTile != null)
                {
                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
                    await renderTargetBitmap.RenderAsync(smallTile);
                    //var xaml = await FileIO.ReadTextAsync(mediumTileFile);
                    //Border mediumTile = (Border)XamlReader.Load(xaml);
                    //mediumTile.DataContext = tile.DataContext;
                    //await renderTargetBitmap.RenderAsync(mediumTile);

                    var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

                    StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                    var tileFile = await storageFolder.CreateFileAsync(ID + "_medium.png", CreationCollisionOption.OpenIfExists);

                    // Encode the image to the selected file on disk
                    using (var fileStream = await tileFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                        encoder.SetPixelData(
                            BitmapPixelFormat.Bgra8,
                            BitmapAlphaMode.Straight,
                            (uint)renderTargetBitmap.PixelWidth,
                            (uint)renderTargetBitmap.PixelHeight,
                            DisplayInformation.GetForCurrentView().LogicalDpi,
                            DisplayInformation.GetForCurrentView().LogicalDpi,
                            pixelBuffer.ToArray());

                        await encoder.FlushAsync();
                    }

                    //if (pinTile == null)
                    //{
                    //    var tile2 = new SecondaryTile(ID, "a", "b", new Uri("ms-appdata:///local/" + ID + "_medium.png"), TileSize.Default);
                    //    tile2.VisualElements.Square150x150Logo = new Uri("ms-appdata:///local/" + ID + "_medium.png");
                    //    pinTile = tile2;
                    //}
                }

                
                if (tile != null)
                {
                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
                    await renderTargetBitmap.RenderAsync(tile);
                    var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

                    StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                    var tileFile = await storageFolder.CreateFileAsync(ID + "_wide.png", CreationCollisionOption.ReplaceExisting);

                    // Encode the image to the selected file on disk
                    using (var fileStream = await tileFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                        encoder.SetPixelData(
                            BitmapPixelFormat.Bgra8,
                            BitmapAlphaMode.Straight,
                            (uint)renderTargetBitmap.PixelWidth,
                            (uint)renderTargetBitmap.PixelHeight,
                            DisplayInformation.GetForCurrentView().LogicalDpi,
                            DisplayInformation.GetForCurrentView().LogicalDpi,
                            pixelBuffer.ToArray());

                        await encoder.FlushAsync();
                    }
                    //var tile2 = new SecondaryTile(ID, "a", "b", new Uri("ms-appdata:///local/" + ID + "_wide.png"), TileSize.Wide310x150);
                    //pinTile.VisualElements.Wide310x150Logo = new Uri("ms-appdata:///local/" + ID + "_wide.png");

                }

                //if (await pinTile.UpdateAsync())
                //{
                // await pinTile.RequestCreateAsync();
                // }


                /*XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Image);


                XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
                ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appdata:///local/" + ID + "_wide.png");
                ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "tile");



                TileNotification tileNotification = new TileNotification(tileXml);

                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);



                XmlDocument tileXml2 = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Image);


                XmlNodeList tileImageAttributes2 = tileXml2.GetElementsByTagName("image");
                ((XmlElement)tileImageAttributes2[0]).SetAttribute("src", "ms-appdata:///local/" + ID + "_medium.png");
                ((XmlElement)tileImageAttributes2[0]).SetAttribute("alt", "tile");



                TileNotification tileNotification2 = new TileNotification(tileXml2);*/

                //TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification2);

               
                Uri wideLogo = new Uri("ms-appdata:///local/" + ID + "_medium.png");
                SecondaryTile secondaryTile = new SecondaryTile(ID.Substring(0, 10),
                                                "a",
                                                ID.Substring(0,10),
                                                wideLogo,
                                                TileSize.Default);
                secondaryTile.VisualElements.Wide310x150Logo = new Uri("ms-appdata:///local/" + ID + "_wide.png");
                await secondaryTile.RequestCreateAsync();
                



            }
            catch(Exception ex)
            {
                int y = 0;
            }


            
        }
    }
}
