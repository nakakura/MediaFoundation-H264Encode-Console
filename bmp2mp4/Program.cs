using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using System;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Linq;

namespace ConsoleApplication4
{
    class Program
    {
        CancellationTokenSource _cts;
        string _OutputFileName = "TranscodeSampleOutput.mp4";
        Windows.Media.MediaProperties.MediaEncodingProfile _Profile;
        Windows.Storage.StorageFile _OutputFile = null;
        Windows.Media.Transcoding.MediaTranscoder _Transcoder = new Windows.Media.Transcoding.MediaTranscoder();
        string _OutputType = "MP4";

        static void Main(string[] args)
        {
            new Program().init();
            Console.ReadLine();
        }

        private void init()
        {


            Run();
        }

        private async void Run()
        {
            try
            {

    var files = await Windows.Storage.KnownFolders.VideosLibrary.GetFilesAsync(
        Windows.Storage.Search.CommonFileQuery.OrderBySearchRank, 0, 10);
 
    var pics = files.Where(f =>{
        return f.Name.EndsWith("wmv");
    });
 
    foreach (var pic in pics)
    {
        _OutputFile = await KnownFolders.VideosLibrary.CreateFileAsync("out.mp4", CreationCollisionOption.GenerateUniqueName);

        _Profile = MediaEncodingProfile.CreateAvi(VideoEncodingQuality.Vga);
        var preparedTranscodeResult = await _Transcoder.PrepareFileTranscodeAsync(pic, _OutputFile, _Profile);

            _Transcoder.VideoProcessingAlgorithm = MediaVideoProcessingAlgorithm.Default;

        if (preparedTranscodeResult.CanTranscode)
        {
            var progress = new Progress<double>(TranscodeProgress);
            _cts = new CancellationTokenSource();
            await preparedTranscodeResult.TranscodeAsync().AsTask(_cts.Token, progress);
            TranscodeComplete();
        }
        else
        {
            TranscodeFailure(preparedTranscodeResult.FailureReason);
        }
    }
            }
            catch (TaskCanceledException)
            {
                TranscodeError("Transcode Canceled");
            }
            catch (Exception exception)
            {
                Console.WriteLine("error");
                Console.WriteLine(exception);
                TranscodeError(exception.Message);
            }

        }

        void TranscodeProgress(double percent)
        {
            Console.WriteLine("Progress:  " + percent.ToString().Split('.')[0] + "%");
        }

        async void TranscodeComplete()
        {
            Console.WriteLine("Transcode completed.");
        }

        async void TranscodeFailure(TranscodeFailureReason reason)
        {
            try
            {
                if (_OutputFile != null)
                {
                    await _OutputFile.DeleteAsync();
                }
            }
            catch (Exception exception)
            {
            }
        }

        async void TranscodeError(string error)
        {
        }
    }
}
