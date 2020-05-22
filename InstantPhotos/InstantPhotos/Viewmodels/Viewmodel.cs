using Java.Lang.Reflect;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InstantPhotos
{
    /// <summary>
    /// Viewmodel for this app
    /// </summary>
    class Viewmodel : INotifyPropertyChanged
    {
        /// <summary>
        /// Notifies view when properties are changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// Instance of this viewmodel
        /// </summary>
        static private Viewmodel instance = null;

        /// <summary>
        /// Command responsible for handling take photo action
        /// </summary>
        public ICommand TakePhotoCommand { get; set; }

        /// <summary>
        /// Command responsible for handling connect action
        /// </summary>
        public ICommand ConnectToggleCommand { get; set; }


        /// <summary>
        /// lock object for multitasked TransferQueue operations
        /// </summary>
        public readonly object syncObject = new object();


        private TcpClient dataClient;
        /// <summary>
        /// Represents clients to server connection
        /// </summary>
        private TcpClient DataClient
        {
            get
            {
                return this.dataClient;
            }
            set
            {
                if (value == dataClient)
                {
                    return;
                }
                this.dataClient = value;

            }
        }

        /// <summary>
        /// Current photos queue pending to send on server
        /// </summary>
        public ObservableCollection<TransferQueueItem> TransferQueue { get; set; } = new ObservableCollection<TransferQueueItem>();


        /// <summary>
        /// Part of server address (another part is set by magic number)
        /// </summary>
        private string serverAddress;

        /// <summary>
        /// Default server port
        /// </summary>
        private const int dataPort = 1337;

        /// <summary>
        /// Represents current server connection status
        /// </summary>
        public bool ConnectionStatus { get; set; }

        /// <summary>
        /// default constructor for this viewmodel
        /// </summary>
        public Viewmodel()
        {
            this.TakePhotoCommand = new RelayCommand<Page>(GetPhoto, null);
            this.ConnectToggleCommand = new RelayCommand<Page>(ConnectToggleAction, null);
            string tmp = GetLocalIpAddress();
            this.serverAddress = tmp.Remove(tmp.LastIndexOf('.') + 1, tmp.Length - tmp.LastIndexOf('.') - 1);
        }

        /// <summary>
        /// Action to do on connect button press
        /// </summary>
        /// <param name="sender"></param>
        private async void ConnectToggleAction(Page sender)
        {         
            if (!ConnectionStatus)
            {
                try
                {
                    string answer = await sender.DisplayPromptAsync("Message", "Please, enter server's magic number");
                    if (answer != null)
                    {
                        this.DataClient = new TcpClient();
                        if (!this.DataClient.ConnectAsync(this.serverAddress + answer, dataPort).Wait(1000))
                        {
                            await sender.DisplayAlert("Error", "There is no such server in this network", "OK");
                        }
                        else
                        {
                            ConnectionStatus = true;
                            Task.Run(() => UploadTask(sender));                            
                        }                       
                    }           

                }
                catch
                {
                    await sender.DisplayAlert("Error", "This server is offline", "OK");
                }
            }
            else
            {
                this.dataClient.Close();
                this.dataClient = null;
                this.ConnectionStatus = false;
            }
        }

        /// <summary>
        /// Gets local network address
        /// </summary>
        /// <returns></returns>
        private string GetLocalIpAddress()
        {
            return Dns.GetHostAddresses(Dns.GetHostName()).LastOrDefault().ToString();
        }

        /// <summary>
        /// Action to do on take photo button press
        /// </summary>
        /// <param name="sender"></param>
        private async void GetPhoto(Page sender)
        {
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = false
                });

                if (file == null)
                    return;

                lock (syncObject)
                {
                    FileStream s = (FileStream)file.GetStream();
                    string name = s.Name.Substring(s.Name.LastIndexOf('/')+1, (int)s.Name.Length - s.Name.LastIndexOf('/')-1);
                    TransferQueue.Add(new TransferQueueItem(s, ImageSource.FromFile(file.Path), name));
                }
            }
            else
            {
                await sender.DisplayAlert("Error", "Grant permissions to camera to make photos", "OK");
            }
        }

        /// <summary>
        /// Task that uploads queue to server
        /// </summary>
        private async void UploadTask(Page sender)
        {
            try
            {
                while (ConnectionStatus)
                {
                    if (TransferQueue.Count != 0)
                    {
                        TransferQueue[0].Stream.Position = 0;
                        BinaryReader r = new BinaryReader(TransferQueue[0].Stream);
                        //Message format (datasize FILENAME/DATASIZE DATA) without spaces and  brackets

                        //Byte representation of filename
                        byte[] name = Encoding.ASCII.GetBytes(TransferQueue[0].Name + '/');

                        //Byte respresentation of image data
                        byte[] imageData;
                        using (var memoryStream = new MemoryStream())
                        {
                            TransferQueue[0].Stream.CopyTo(memoryStream);
                            imageData = memoryStream.ToArray();
                        }
                        //byte representation of package size
                        byte[] totalLen = BitConverter.GetBytes(name.Length + imageData.Length + 4);

                        byte[] package = new byte[name.Length + imageData.Length + 4];
                        System.Array.Copy(totalLen, 0, package, 0, totalLen.Length);
                        System.Array.Copy(name, 0, package, totalLen.Length, name.Length);
                        System.Array.Copy(imageData, 0, package, totalLen.Length + name.Length, imageData.Length);
                        NetworkStream s = DataClient.GetStream();
                        int totalSentBytes = 0;
                        int packetSize = 4096;
                        while (totalSentBytes != package.Length)
                        {
                            int leftToSend = package.Length - totalSentBytes;
                            if (leftToSend > packetSize)
                            {
                                s.Write(package, totalSentBytes, packetSize);
                                totalSentBytes += packetSize;
                            }
                            else
                            {
                                s.Write(package, totalSentBytes, leftToSend);
                                totalSentBytes += leftToSend;
                            }                           
                        }
                        string n = TransferQueue[0].Stream.Name;                                      
                        TransferQueue.RemoveAt(0);
                        File.Delete(n);
                    }
                    await Task.Delay(20);
                }
            }
            catch
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await sender.DisplayAlert("Error", "Server is not responding", "OK");
                    ConnectionStatus = false;
                });
                
            }
            
        }


        /// <summary>
        /// Get instance of this viewmodel
        /// </summary>
        /// <returns></returns>
        static public Viewmodel GetInstance()
        {
            if (instance == null)
            {
                instance = new Viewmodel();
            }
            return instance;
        }
    }
}
