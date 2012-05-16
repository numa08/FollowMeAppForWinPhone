using System;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using Microsoft.Phone.Reactive;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Collections.Generic;

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
                    requestTwitter((string)store[MainPage.User_ID_Key]);
                }
                else
                {
                    //してない
                    NavigationService.Navigate(new Uri("/UserSearchPage.xaml", UriKind.Relative));
                }
            }
            else if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
                requestTwitter((string)store[MainPage.User_ID_Key]);
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
        /// Twitterにリクエストを送る
        /// </summary>
        /// <param name="userId">The user id.</param>
        private void requestTwitter(string userId)
        {
            //TODO: １個目のパノラマのタイトルをユーザーIDに変える
            profileItem.Header = userId;
            //TODO: Twitterにアクセスして、データを得る
            var twitterUrl = buildTwitterUri(userId);
            Debug.WriteLine(twitterUrl.Uri);
            WebRequest.Create(twitterUrl.Uri)
                .DownloadStringAsync()
                .ObserveOnDispatcher()
                .Subscribe(
                    str => parserJson(str),
                    e => MessageBox.Show("エラ-.ユーザー名,通信状況を確認してください."));
        }

        /// <summary>
        /// JSONを解析する
        /// </summary>
        /// <param name="json">The json.</param>
        private void parserJson(string json)
        {
            //TODO: データを解析するU
            var serializer = new DataContractJsonSerializer(typeof(TwitterRoot[]));
            var twitter = (TwitterRoot[])serializer.ReadObject(new MemoryStream(Encoding.Unicode.GetBytes(json)));
            Debug.WriteLine(twitter[0].text);
            //TODO: 解析結果を表示する
            List<UserTimeLine> timeLine = new List<UserTimeLine>();
            foreach (TwitterRoot tweet in twitter)
            {
                timeLine.Add(new UserTimeLine() { Text = tweet.text, ProfileImage = tweet.user.profile_image_url });
            }
            userTimeLineList.ItemsSource = timeLine;
        }

        /// <summary>
        /// Builds the twitter URI.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        private UriBuilder buildTwitterUri(string userId)
        {
            var twitterUrl = new UriBuilder("https", "api.twitter.com");
            twitterUrl.Path = "/1/statuses/user_timeline.json";
            var screeNameQuery = "screen_name=" + userId;
            var countQuery = "count=20";
            twitterUrl.Query = screeNameQuery + "&" + countQuery;
            return twitterUrl;
        }
    }

    public static class WebRequestExtensions
    {
        public static IObservable<string> DownloadStringAsync(this WebRequest request)
        {
            return Observable.FromAsyncPattern<WebResponse>(request.BeginGetResponse, request.EndGetResponse)()
                .Select(res =>
                    {
                        using (var stream = res.GetResponseStream())
                        using (var sr = new StreamReader(stream))
                        {
                            return sr.ReadToEnd();
                        }
                    });
        }
    }

    public class UserTimeLine
    {
        public string ProfileImage { get; set; }
        public string Text { get; set; }
    }
}