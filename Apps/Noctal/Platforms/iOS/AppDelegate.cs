using Foundation;
using Microsoft.Maui.Hosting;
using UIKit;

namespace Noctal;

//[Register(nameof(AppDelegate))]
//public class AppDelegate : UIApplicationDelegate, IUIApplicationDelegate
//{
//    public override UIWindow? Window { get; set; }

//    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
//    {
//        Window = new UIWindow(UIScreen.MainScreen.Bounds);


//        var vc = new UIViewController();
//        vc.View!.BackgroundColor = UIColor.Red;
//        var navController = new UINavigationController(vc);
//        var tabController = new UITabBarController();
//        tabController.SetViewControllers(new[] { navController }, false);
//        Window.RootViewController = tabController;

//        Window.MakeKeyAndVisible();


//        return true;
//    }


//}

[Register(nameof(AppDelegate))]
public class AppDelegate : UIApplicationDelegate
{
    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        return true;
    }

    public override UISceneConfiguration GetConfiguration(UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
    {
        return new UISceneConfiguration("Default Configuration", connectingSceneSession.Role);
    }
}