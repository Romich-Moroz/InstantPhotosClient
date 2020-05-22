using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InstantPhotos.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoundButton : ContentView
    {

       // public static readonly BindableProperty ButtonRadius = BindableProperty.Create("Radius", typeof(int), typeof(RoundButton), 0.0, BindingMode.TwoWay, propertyChanged:);
        public RoundButton()
        {
            InitializeComponent();
        }

        private static void ButtonRadiusChanged(BindableObject bindable,object oldValue, object newValue)
        {

        }
    }
}