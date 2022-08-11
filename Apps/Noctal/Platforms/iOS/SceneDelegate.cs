using Foundation;
using UIKit;

namespace Noctal;

[Register(nameof(SceneDelegate))]
public class SceneDelegate : UIWindowSceneDelegate
{
    public override UIWindow? Window { get; set; }

    public override void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
    {
        if (scene is UIWindowScene windowScene)
        {
            Window = new UIWindow(windowScene);

            var vc = new UIViewController()
            {
                Title = "Hello",
            };
            var navController = new UINavigationController(vc)
            {
                TabBarItem = new UITabBarItem("World", null, 0),
            };
            var tabController = new UITabBarController();
            tabController.SetViewControllers(new[] { navController }, false);
            Window.RootViewController = tabController;

            Window.MakeKeyAndVisible();
        }
    }
}
