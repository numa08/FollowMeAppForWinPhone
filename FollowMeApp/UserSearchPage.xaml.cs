using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;
using System.IO.IsolatedStorage;

namespace FollowMeApp
{
    public partial class UserSearchPage : PhoneApplicationPage
    {
        public UserSearchPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 検索ボタンがクリックされた
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void searchButtonClocked(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: ここにイベント ハンドラーのコードを追加します。
            //TODO: TextBoxの値を取得する
            string userid = UserIdBox.Text;
            if (!userid.Equals(""))
            {
                //TODO: 正しく入っていたら、分離ストレージに保存
                IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
                setting[MainPage.User_ID_Key] = userid;
                setting.Save();
                //TODO: 前の画面に戻る
                NavigationService.GoBack();
            }
            else
            {
                //TODO: 正しく入っていない時は
                //TODO: メッセージBOXを表示
                string message = "ユーザーIDを正しく入力してください.";
                MessageBox.Show(message, "", MessageBoxButton.OK);

            }
        }
    }
}