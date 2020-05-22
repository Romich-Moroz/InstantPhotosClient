using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InstantPhotos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectPage : ContentPage
    {
        public ConnectPage()
        {
            InitializeComponent();
            this.BindingContext = Viewmodel.GetInstance();
        }

    }
}