using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InstantPhotos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TakePhotoPage : ContentPage
    {
        public TakePhotoPage()
        {
            InitializeComponent();
            this.BindingContext = Viewmodel.GetInstance();
        }
    }
}