using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Sudoku_Solver_Xamarin.Controls;
using Sudoku_Solver_Xamarin.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SudokuSquareEntry), typeof(SudokuSquareEntryRenderer))]
namespace Sudoku_Solver_Xamarin.Droid.Renderers
{
    public class SudokuSquareEntryRenderer : EntryRenderer
    {
        public SudokuSquareEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(Android.Graphics.Color.Transparent);
                Control.Background = gd;
                Control.SetRawInputType(InputTypes.ClassNumber);
                Control.SetHintTextColor(ColorStateList.ValueOf(Android.Graphics.Color.White));
                Control.Gravity = GravityFlags.Center;
            }
        }
    }
}