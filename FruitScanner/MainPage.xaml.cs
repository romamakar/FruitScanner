using MLLibrary;
using System.Drawing;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace FruitScanner;

public partial class MainPage : ContentPage
{

    public MainPage()
    {
        InitializeComponent();
    }



    private void camera_CamerasLoaded(object sender, EventArgs e)
    {
        camera.Camera = camera.Cameras[0];

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(3000);
            await camera.StopCameraAsync();
            await camera.StartCameraAsync();

        });

    }

    private async void scanBtn_Clicked(object sender, EventArgs e)
    {
        fileBtn.IsVisible = false;
        camera.IsVisible = false;
        scanBtn.IsVisible = false;
        backBtn.IsVisible = true;
        myImage.IsVisible = false;
        listLabel.IsVisible = false;
        listBtn.IsVisible = true;
        loadingLabel.IsVisible = true;
        loadingLabel.IsVisible = true;
        scanBtn.IsEnabled = false;
        var filename = Guid.NewGuid().ToString() + ".jpg";
        var result = await camera.SaveSnapShot(Camera.MAUI.ImageFormat.JPEG, FileSystem.Current.CacheDirectory + "//" + filename);
        if (result)
        {
            var ml = MLAnalyzer.Analyze(FileSystem.Current.CacheDirectory + "//" + filename);
            loadingLabel.IsVisible = false;
            myImage.Source = ImageSource.FromFile(ml.Item1);
            myImage.IsVisible = true;
            listLabel.Text = ml.Item2;
        }
    }

    private void backBtn_Clicked(object sender, EventArgs e)
    {
        loadingLabel.IsVisible = false;
        camera.IsVisible = true;
        scanBtn.IsVisible = true;
        backBtn.IsVisible = false;
        myImage.IsVisible = false;
        listBtn.IsVisible = false;
        scanBtn.IsEnabled = true;
        listLabel.IsVisible = false;
        fileBtn.IsVisible = true;
    }

    private void listBtn_Clicked(object sender, EventArgs e)
    {
        listLabel.IsVisible = true;
    }

    private async void fileBtn_Clicked(object sender, EventArgs e)
    {
        fileBtn.IsVisible = false;
        camera.IsVisible = false;
        scanBtn.IsVisible = false;
        backBtn.IsVisible = true;
        myImage.IsVisible = false;
        listLabel.IsVisible = false;
        listBtn.IsVisible = true;
        loadingLabel.IsVisible = true;
        loadingLabel.IsVisible = true;
        scanBtn.IsEnabled = false;
        try
        {
            var result = await FilePicker.Default.PickAsync();
            if (result != null)
            {
                var ml = MLAnalyzer.Analyze(result.FullPath);
                myImage.Source = ImageSource.FromFile(ml.Item1);
                loadingLabel.IsVisible = false;
                myImage.IsVisible = true;
                listLabel.Text = ml.Item2;
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", ex.Message, "OK");
        }

    }
}

