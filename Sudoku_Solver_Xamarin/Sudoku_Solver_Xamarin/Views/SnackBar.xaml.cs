using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sudoku_Solver_Xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SnackBar : TemplatedView
    {
        public SnackBar()
        {
            InitializeComponent();
            Close();
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(SnackBar), Color.White);
        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public static readonly BindableProperty MessageProperty = BindableProperty.Create("Message", typeof(string), typeof(SnackBar), default(string), BindingMode.TwoWay);
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create("FontSize", typeof(float), typeof(SnackBar), default(float));
        public float FontSize
        {
            get { return (float)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly BindableProperty IsOpenProperty = BindableProperty.Create("IsOpen", typeof(bool), typeof(SnackBar), false, propertyChanged: IsOpenChanged);
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        private static void IsOpenChanged(BindableObject bindable, object oldValue, object newValue)
        {

            if (bindable != null && newValue != null)
            {
                var control = (SnackBar)bindable;
                control.IsOpen = (bool)newValue;

                if (control.IsOpen == false)
                {
                    control.Close();
                }
                else
                {
                    control.Open(control.Message);
                }
            }
        }

        public async void Close()
        {
            await this.TranslateTo(0, 50, 100);
            Message = string.Empty;
            IsOpen = IsVisible = false;
        }

        public async void Open(string message)
        {
            IsVisible = true;
            Message = message;
            await this.TranslateTo(0, 0, 100);
        }
    }
}