using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Prerelease.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
		readonly Game1 _game;

		public GamePage()
        {
            this.InitializeComponent();

			// Create the game.
			var launchArguments = string.Empty;
            _game = MonoGame.Framework.XamlGame<Game1>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);

            App.Current.Suspending += AppOnSuspending;

            SetupInkCanvas();
        }

        private async void AppOnSuspending(object sender, SuspendingEventArgs suspendingEventArgs)
        {
            //await SaveInkStrokes();
        }

        private async Task SaveInkStrokes()
        {
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync("horse.data", CreationCollisionOption.ReplaceExisting);

            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                /*using (var writer = new StreamWriter(stream.AsStreamForWrite()))
                {
                    writer.WriteLine("horse");
                }*/

                await Canvas.InkPresenter.StrokeContainer.SaveAsync(stream);
            }
        }

        private async Task LoadInkStrokes()
        {
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync("horse.data", CreationCollisionOption.OpenIfExists);

            if (file != null)
            {
                using (var stream = await file.OpenSequentialReadAsync())
                {
                    try
                    {
                        await Canvas.InkPresenter.StrokeContainer.LoadAsync(stream);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        private void SetupInkCanvas()
        {
            const int minPenSize = 2;
            const int penSizeIncrement = 2;
            int penSize;

            penSize = minPenSize + penSizeIncrement * 1;

            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            drawingAttributes.Color = Windows.UI.Colors.Red;
            drawingAttributes.Size = new Size(penSize, penSize);
            drawingAttributes.IgnorePressure = false;
            drawingAttributes.FitToCurve = true;

            Canvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            Canvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen;

            Canvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;
            Canvas.InkPresenter.StrokesErased += InkPresenter_StrokesErased;

            //LoadInkStrokes().Wait();
        }

        private void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
        {
        }

        private void InkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            //SaveInkStrokes().Wait();

            foreach (var segment in args.Strokes[0].GetRenderingSegments())
            {

            }
        }
    }
}
