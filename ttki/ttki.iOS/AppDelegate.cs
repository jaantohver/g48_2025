using UIKit;
using Foundation;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;

namespace ttki.iOS
{
    public partial class AppDelegate : UIApplicationDelegate
    {
        UIWindow window;
        override public UIWindow Window
        {
            get => window;
            set => window = value;
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            AppCenter.Start("49a89ce5-bdee-422e-8dff-46d5cb5b9eb8", typeof(Analytics), typeof(Crashes));
            Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);

            TabBarController controller = new TabBarController();

            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.RootViewController = controller;
            Window.MakeKeyAndVisible();

            LogController.Log("App started successfully");

            return true;
        }
    }
}
