using Microsoft.Phone.Controls;
using System.IO;
using System.IO.IsolatedStorage;

namespace FollowMeApp
{
    public partial class SearchPage : PhoneApplicationPage
    {
        IsolatedStorageSettings store;
        public SearchPage()
        {
            InitializeComponent();
            store = IsolatedStorageSettings.ApplicationSettings;
        }

        /// <summary>
        /// Handles the clicked event of the search control.
        ///検索ボタンがクリックされた
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void search_clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            //TODO: TextBoxの値を取得する
            string userId = userIdBox.Text;
            if (userId != "")
            {
                //TODO: 分割ストレージに格納する
                store["user_id"] = userId;
                store.Save();
                //TODO: 前の画面に戻る
                NavigationService.GoBack();
            }
            else
            {
                //TODO: 正しく入力してください
            }
        }
    }
}