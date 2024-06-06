using UIKit;

namespace ttki.iOS
{
	public class LogView : UIView
	{
		UITextView textView;

		public LogView()
		{
			textView = new UITextView();
			textView.Editable = false;

			AddSubview(textView);
		}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

			textView.Frame = Bounds;
        }

		public void Update(string text)
		{
			textView.Text = text;
		}
    }
}
