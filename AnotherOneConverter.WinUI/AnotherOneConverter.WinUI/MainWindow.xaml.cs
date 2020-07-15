using System;
using System.Runtime.InteropServices;
using AnotherOneConverter.Core;
using AnotherOneConverter.Core.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT;

namespace AnotherOneConverter.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Locator = new ViewModelLocator(services =>
            {
                services.AddSingleton<IFilePickerHelpers, FilePickerHelpers>();
            });

            LayoutRoot.Loaded -= OnLoaded;
            LayoutRoot.Loaded += OnLoaded;

            Tabs.TabCloseRequested -= OnTabCloseRequested;
            Tabs.TabCloseRequested += OnTabCloseRequested;
        }

        private void OnTabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (ViewModel.CloseProjectCommand.CanExecute(args.Item))
            {
                ViewModel.CloseProjectCommand.Execute(args.Item);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Loaded.CanExecute(null))
            {
                ViewModel.Loaded.Execute(null);
            }
        }

        public ViewModelLocator Locator { get; }

        public MainViewModel ViewModel => Locator.MainViewModel;

        #region IInitializeWithWindow

        [ComImport]
        [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize(IntPtr hwnd);
        }

        // This is the actual wrapper for CSWinRT  
        [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
        internal class InitializeWithWindowWrapper : IInitializeWithWindow
        {
            [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
            public struct Vftbl
            {
                public delegate int _Initialize_0(IntPtr thisPtr, IntPtr hwnd);

                internal global::WinRT.Interop.IUnknownVftbl IUnknownVftbl;
                public _Initialize_0 Initialize_0;

                public static readonly Vftbl AbiToProjectionVftable;
                public static readonly IntPtr AbiToProjectionVftablePtr;

                static Vftbl()
                {
                    AbiToProjectionVftable = new Vftbl
                    {
                        IUnknownVftbl = global::WinRT.Interop.IUnknownVftbl.AbiToProjectionVftbl,
                        Initialize_0 = Do_Abi_Initialize_0
                    };
                    AbiToProjectionVftablePtr = Marshal.AllocHGlobal(Marshal.SizeOf<Vftbl>());
                    Marshal.StructureToPtr(AbiToProjectionVftable, AbiToProjectionVftablePtr, false);
                }

                private static int Do_Abi_Initialize_0(IntPtr thisPtr, IntPtr windowHandle)
                {
                    try
                    {
                        ComWrappersSupport.FindObject<IInitializeWithWindow>(thisPtr).Initialize(windowHandle);
                    }
                    catch (Exception ex)
                    {
                        return Marshal.GetHRForException(ex);
                    }
                    return 0;
                }
            }
            internal static ObjectReference<Vftbl> FromAbi(IntPtr thisPtr) => ObjectReference<Vftbl>.FromAbi(thisPtr);

            public static implicit operator InitializeWithWindowWrapper(IObjectReference obj) => (obj != null) ? new InitializeWithWindowWrapper(obj) : null;
            protected readonly ObjectReference<Vftbl> _obj;
            public IObjectReference ObjRef { get => _obj; }
            public IntPtr ThisPtr => _obj.ThisPtr;
            public ObjectReference<I> AsInterface<I>() => _obj.As<I>();
            public A As<A>() => _obj.AsType<A>();
            public InitializeWithWindowWrapper(IObjectReference obj) : this(obj.As<Vftbl>()) { }
            internal InitializeWithWindowWrapper(ObjectReference<Vftbl> obj)
            {
                _obj = obj;
            }

            public void Initialize(IntPtr windowHandle)
            {
                Marshal.ThrowExceptionForHR(_obj.Vftbl.Initialize_0(ThisPtr, windowHandle));
            }
        }

        #endregion
    }
}
