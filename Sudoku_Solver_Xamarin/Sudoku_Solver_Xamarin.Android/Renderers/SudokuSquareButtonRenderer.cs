using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Views;
using Sudoku_Solver_Xamarin.Controls;
using Sudoku_Solver_Xamarin.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SudokuSquareButton), typeof(SudokuSquareButtonRenderer))]
namespace Sudoku_Solver_Xamarin.Droid.Renderers
{
    public class SudokuSquareButtonRenderer : EntryRenderer
    {
        public SudokuSquareButtonRenderer(Context context) : base(context)
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