using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App7
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            //imgChoosed.Source = ImageSource.FromResource("App7.thumbnail.jpg");
        }
        private async void BtnPick_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });
                if (file == null) return;
                imgChoosed.Source = ImageSource.FromStream(() => {
                    var stream = file.GetStream();
                    return stream;
                });
                var result = await GetImageDescription(file.GetStream());
                lblResult.Text = null;
                file.Dispose();
                foreach (string tag in result.Description.Tags)
                {
                    lblResult.Text = lblResult.Text + "\n" + tag;
                }
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }
        }

        public async Task<AnalysisResult> GetImageDescription(Stream imageStream)
        {
            VisionServiceClient visionClient = new VisionServiceClient("afb2168d56a147d7b3b7728aa0a9558a", "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0");
            VisualFeature[] features = {
                VisualFeature.Tags,
                VisualFeature.Categories,
                VisualFeature.Description
            };
            return await visionClient.AnalyzeImageAsync(imageStream, features.ToList(), null);
        }

        public async void BtnTake_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "xamarin.jpg"
                });
                if (file == null)
                    return;
                imgChoosed.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
                var result = await GetImageDescription(file.GetStream());
                file.Dispose();
                lblResult.Text = null;
                foreach (string tag in result.Description.Tags)
                {
                    lblResult.Text = lblResult.Text + "\n" + tag;
                }
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }
        }
    }
}
