using UIKit;
using CoreGraphics;

namespace door.iOS
{
	public class HomeView : UIView
	{
		readonly UILabel workerIdLabel, timerLabel;
		public readonly UIButton Button;

		public HomeView()
		{
			BackgroundColor = UIColor.SystemBackground;

			workerIdLabel = new UILabel();
			workerIdLabel.TextAlignment = UITextAlignment.Center;
			workerIdLabel.Font = UIFont.SystemFontOfSize(15);
			workerIdLabel.TextColor = UIColor.LightGray;
			workerIdLabel.Hidden = true;

			timerLabel = new UILabel();
			timerLabel.Text = "00:00:00";
			timerLabel.TextAlignment = UITextAlignment.Center;
			timerLabel.Font = UIFont.MonospacedDigitSystemFontOfSize(20, 20);
			timerLabel.Hidden = true;

			Button = new UIButton();

			AddSubviews(workerIdLabel, timerLabel, Button);
		}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            workerIdLabel.Frame = new CGRect(0, 0, Frame.Width, 200);

            timerLabel.Frame = new CGRect(0, 0, Frame.Width, Frame.Height / 2);

			Button.Frame = new CGRect(
				(Frame.Width - 100) / 2,
				(Frame.Height - 100) / 2,
				100,
				100
			);
			Button.Layer.CornerRadius = Button.Frame.Width / 2;
        }

		public void SetIdentify()
		{
			Button.BackgroundColor = UIColor.FromRGB(181, 226, 253);
			Button.SetTitle("Isikusta", UIControlState.Normal);

			workerIdLabel.Hidden = true;
            timerLabel.Hidden = true;
        }

		public void SetStart()
		{
            Button.BackgroundColor = UIColor.FromRGB(67, 160, 71);
			Button.SetTitle("Alusta", UIControlState.Normal);

			workerIdLabel.Hidden = false;
            timerLabel.Hidden = true;
        }

		public void SetStop()
		{
            Button.BackgroundColor = UIColor.FromRGB(160, 71, 70);
            Button.SetTitle("Lõpeta", UIControlState.Normal);

            workerIdLabel.Hidden = false;
            timerLabel.Hidden = false;
        }

		public void SetTimerString(string text)
		{
			InvokeOnMainThread(delegate
			{
				timerLabel.Text = text;
			});
		}

		public void SetWorkerId(string id)
		{
			workerIdLabel.Text = "Töötaja id: " + id;
		}
    }
}
