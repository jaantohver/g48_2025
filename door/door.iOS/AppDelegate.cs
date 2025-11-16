using UIKit;
using Foundation;

namespace door.iOS
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
            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.RootViewController = new MainController();
            Window.MakeKeyAndVisible();

            return true;
        }
    }
}
