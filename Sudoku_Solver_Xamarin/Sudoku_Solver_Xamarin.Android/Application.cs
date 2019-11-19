using System;
using System.Collections.Generic;
using System.Reflection;
using Android.App;
using Android.Runtime;
using Caliburn.Micro;
using Sudoku_Solver_Xamarin;
using Sudoku_Solver_Xamarin.ViewModels;

namespace Setup.Forms.Droid
{
    [Application]
    public class Application : CaliburnApplication
    {
        private SimpleContainer container;

        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {

        }

        public override void OnCreate()
        {
            base.OnCreate();

            Initialize();
        }

        protected override void Configure()
        {
            container = new SimpleContainer();
            container.Instance(container);
            container.Singleton<FormsApp>();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[]
            {
                GetType().Assembly,
                typeof (HomeViewModel).Assembly
            };
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }
    }
}