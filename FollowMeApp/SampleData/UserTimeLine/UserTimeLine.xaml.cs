//      *********    このファイルを編集しないでください     *********
//      このファイルはデザイン ツールにより作成されました。
//      このファイルに変更を加えるとエラーが発生する場合があります。
namespace Expression.Blend.SampleData.UserTimeLine
{
	using System; 

// To significantly reduce the sample data footprint in your production application, you can set
// the DISABLE_SAMPLE_DATA conditional compilation constant and disable sample data at runtime.
#if DISABLE_SAMPLE_DATA
	internal class UserTimeLine { }
#else

	public class UserTimeLine : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		public UserTimeLine()
		{
			try
			{
				System.Uri resourceUri = new System.Uri("/FollowMeApp;component/SampleData/UserTimeLine/UserTimeLine.xaml", System.UriKind.Relative);
				if (System.Windows.Application.GetResourceStream(resourceUri) != null)
				{
					System.Windows.Application.LoadComponent(this, resourceUri);
				}
			}
			catch (System.Exception)
			{
			}
		}

		private ItemCollection _Collection = new ItemCollection();

		public ItemCollection Collection
		{
			get
			{
				return this._Collection;
			}
		}
	}

	public class Item : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private System.Windows.Media.ImageSource _ProfileImage = null;

		public System.Windows.Media.ImageSource ProfileImage
		{
			get
			{
				return this._ProfileImage;
			}

			set
			{
				if (this._ProfileImage != value)
				{
					this._ProfileImage = value;
					this.OnPropertyChanged("ProfileImage");
				}
			}
		}

		private string _Text = string.Empty;

		public string Text
		{
			get
			{
				return this._Text;
			}

			set
			{
				if (this._Text != value)
				{
					this._Text = value;
					this.OnPropertyChanged("Text");
				}
			}
		}
	}

	public class ItemCollection : System.Collections.ObjectModel.ObservableCollection<Item>
	{ 
	}
#endif
}
