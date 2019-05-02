﻿using System;
using Autofac;
using FinalApp.Network;
using FinalApp.Services;
using FinalApp.ViewModels;
using FinalApp.Views.Pages.MainMasterDetail;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace FinalApp
{
    public partial class App : Application {

        public static IContainer Container { get; private set; }

        public App() {
            InitializeComponent();
            RegisterDependencies();
            MainPage = new MainMasterDetailPage();
        }

        protected override void OnStart() {
            // Handle when your app starts
        }

        protected override void OnSleep() {
            // Handle when your app sleeps
        }

        protected override void OnResume() {
            // Handle when your app resumes
        }

        private void RegisterDependencies() {
            var builder = new ContainerBuilder();
            builder.RegisterType<LoginViewModel>();
            builder.RegisterType<MainPageViewModel>();
            builder.RegisterType<WestEuropeOCRServices>().As<INetworkOCRServices>().SingleInstance();
            builder.RegisterType<UserDataRepository>().As<IUserDataRepository>().SingleInstance();
            builder.RegisterType<OCRDataExtractor>().As<IOCRDataExtractor>().SingleInstance();
            builder.RegisterType<AnalyzePicturePageViewModel>();
            builder.RegisterType<ExpensesPageViewModel>();
            builder.RegisterType<IncomesPageViewModel>();
            builder.RegisterType<TagsPageViewModel>();
            builder.RegisterType<TagDetailPageViewModel>().InstancePerDependency();
            builder.RegisterType<IncomeDetailPageViewModel>();
            builder.RegisterType<ExpenseDetailPageViewModel>();
            builder.RegisterType<ExpensesListPageViewModel>();

            Container = builder.Build();
        }
    }
}
