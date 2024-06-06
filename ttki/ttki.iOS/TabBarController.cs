using UIKit;

namespace ttki.iOS
{
	public class TabBarController : UITabBarController
	{
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UITabBar.Appearance.SelectedImageTintColor = UIColor.FromRGB(67, 160, 71);

            UINavigationController homeNavController = new UINavigationController(new HomeController());
            homeNavController.TabBarItem = new UITabBarItem("Kodu", null, 0);
            homeNavController.TabBarItem.Image = UIImage.FromFile("home.png");

            UINavigationController locationsNavController = new UINavigationController(new AreasController(false));
            locationsNavController.TabBarItem = new UITabBarItem("Tööalad", null, 1);
            locationsNavController.TabBarItem.Image = UIImage.FromFile("area.png");

            UINavigationController worksNavController = new UINavigationController(new WorksController());
            worksNavController.TabBarItem = new UITabBarItem("Tööd", null, 2);
            worksNavController.TabBarItem.Image = UIImage.FromFile("work.png");

            ViewControllers = new UIViewController[] { homeNavController, locationsNavController, worksNavController };
        }
	}
}
