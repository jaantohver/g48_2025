using System;

using UIKit;
using Foundation;

namespace ttki.iOS
{
	public class LogController : UIViewController
	{
        static string text;

        LogView contentView;

        static LogController()
        {
            text = "Application version 1.0.5 (8)\n";
            text += NSUserDefaults.StandardUserDefaults.StringForKey("log");
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View = contentView = new LogView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            contentView.Update(text);
        }

        public static void Log(string s)
        {
            Console.WriteLine(s);
            text += "[" + DateTime.Now.ToString() + "] " + s + "\n";
            NSUserDefaults.StandardUserDefaults.SetString(text, "log");
        }

        public static void Log(object o)
        {
            Console.WriteLine(o);
            text += "[" + DateTime.Now.ToString() + "] " + o.ToString() + "\n";
            NSUserDefaults.StandardUserDefaults.SetString(text, "log");
        }
    }
}
