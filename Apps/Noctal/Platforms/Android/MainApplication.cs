﻿using Android.App;
using Android.Runtime;
//using Microsoft.Maui.Hosting;

namespace Noctal;

[Application]
public class MainApplication : Application
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	//protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
