using System;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;

namespace FollowMeApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static string User_ID_Key = "user_id";
        IsolatedStorageSettings store;
        // コンストラクター
        public MainPage()
        {
            InitializeComponent();

            // ListBox コントロールのデータ コンテキストをサンプル データに設定します
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            store = IsolatedStorageSettings.ApplicationSettings;
#if DEBUG
            store.Clear();
            store.Save();
#endif
        }
        // ViewModel Items のデータを読み込みます
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //検索ページから戻ってきた
            Debug.WriteLine("navigate from " + e.NavigationMode);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Debug.WriteLine("navigate to " + e.NavigationMode);
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                //初回起動時
                if (store.Contains(User_ID_Key))
                {
                    //前回、ユーザーIDをを入力した
                    setPageContext((string)store[MainPage.User_ID_Key]);
                }
                else
                {
                    //してない
                    NavigationService.Navigate(new Uri("/UserSearchPage.xaml", UriKind.Relative));
                }
            }
            else if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
                setPageContext((string)store[MainPage.User_ID_Key]);
            }
        }

        private void search_clicked(object sender, System.EventArgs e)
        {
            // TODO: ここにイベント ハンドラーのコードを追加します。
            NavigationService.Navigate(new Uri("/UserSearchPage.xaml", UriKind.Relative));
        }

        private void page_loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: ここにイベント ハンドラーのコードを追加します。
            Debug.WriteLine("on loaded");
        }

        /// <summary>
        /// ページの表示内容を切り替える
        /// </summary>
        /// <param name="userId">The user id.</param>
        private void setPageContext(string userId)
        {
            //TODO: １個目のパノラマのタイトルをユーザーIDに変える
            profileItem.Header = userId;
            //TODO: Twitterにアクセスして、データを得る
            //TODO: データを解析する
            //TODO: 解析結果を表示する
        }
    }
}