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
using System.Windows.Media.Imaging;
using ImageTools;
using ImageTools.IO.Gif;

namespace FollowMeApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static string User_ID_Key = "user_id";
        private static string TWITTER_URL = "http://twitter.com/";
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
                userTimeLineList.Visibility = Visibility.Collapsed;
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
                
                if (store.Contains(User_ID_Key))
                {
                    //ちゃんとユーザーIDを入力した
                    requestTwitter((string)store[MainPage.User_ID_Key]);
                }
                else
                {
                    //してない
                    MessageBox.Show("ユーザーIDを入力してください");
                }
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
            //TODO: プログレスバー起動
            progressBar.Visibility = Visibility.Visible;
            //TODO: Twitterにアクセスして、データを得る
            var twitterUrl = buildTwitterUri(userId);
            Debug.WriteLine(twitterUrl);
            WebRequest.Create(twitterUrl)
                .DownloadStringAsync()
                .ObserveOnDispatcher()
                .Subscribe(
                    str => onLoadTwitter(str),
                    e => MessageBox.Show("エラ-.ユーザー名,通信状況を確認してください."));
        }

        /// <summary>
        /// Twitterのデータ読み込み後の処理
        /// </summary>
        /// <param name="json">The json.</param>
        private void onLoadTwitter(string json)
        {
            var twitterDate = parserJson(json);
            //データチェック後、表示処理
            if (isOk(twitterDate))
            {
                showResult(twitterDate);
                //TODO プログレスバーを終わる
                progressBar.Visibility = Visibility.Collapsed;
            }
            else
            {
            //TODO プログレスバーを終わる
            progressBar.Visibility = Visibility.Collapsed;
            MessageBox.Show("エラー.データの取得ができません.");
            }
        }

        /// <summary>
        /// JSONを解析する
        /// </summary>
        /// <param name="json">The json.</param>
        private TwitterRoot[] parserJson(string json)
        {
            //TODO: データを解析するU
            var serializer = new DataContractJsonSerializer(typeof(TwitterRoot[]));
            return (TwitterRoot[])serializer.ReadObject(new MemoryStream(Encoding.Unicode.GetBytes(json)));
        }

        /// <summary>
        ///取得したデータが正常かどうか判断する
        /// </summary>
        /// <param name="dataArray">取得したデータ</param>
        /// <returns>
        ///   <c>true</c> データの配列が存在する <c>false</c>.
        /// </returns>
        private bool isOk(TwitterRoot[] dataArray)
        {
            return dataArray.Length > 0;
        }

        /// <summary>
        /// 結果を表示する
        /// </summary>
        /// <param name="twitterDataArray">The twitter.</param>
        private void showResult(TwitterRoot[] twitterDataArray)
        {

            var twitter = twitterDataArray[0];
            //TODO: 解析結果を表示する
            //TODO: １個目のパノラマのタイトルをユーザーIDに変える
            profileItem.Header = twitter.user.screen_name;

            //背景画像を表示する
            //BacgroundImage.ImageSource = getBitmap(new Uri(twitter.user.profile_background_image_url));

            //自己紹介文の表示
            profileText.Text = twitterDataArray[0].user.description;

            //アイコンの表示
            profileImage.Source = getBitmap(new Uri(twitter.user.profile_image_url));

            //QRコードの表示
            var qrCodeUri = buildQrCodeUri(TWITTER_URL + twitter.user.screen_name);
            Debug.WriteLine(qrCodeUri.Uri);
            qrImage.Source = new BitmapImage(qrCodeUri.Uri);


            List<UserTimeLine> timeLine = new List<UserTimeLine>();
            foreach (TwitterRoot tweet in twitterDataArray)
            {
                timeLine.Add(new UserTimeLine() { Text = tweet.text, ProfileImage = tweet.user.profile_image_url });
            }
            userTimeLineList.ItemsSource = timeLine;
            userTimeLineList.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///URIからBitmapを得る.GIFかどうかをファイルから判断して、それなりに処理する 予定
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        private BitmapImage getBitmap(Uri uri)
        {
            return new BitmapImage(uri);
            //BitmapImage bitmap;
            //if (uri.ToString().EndsWith(".gif"))
            //{
            //    using (var strm = Application.GetResourceStream(uri).Stream)
            //    {
            //        bitmap = new BitmapImage();
            //        ExtendedImage image = new ExtendedImage();
            //        var decoder = new GifDecoder();
            //        decoder.Decode(image, strm);

            //        bitmap.SetSource(image.ToStream());
            //    }
            //}
            //else
            //{
            //    bitmap = new BitmapImage(uri);                
            //}
            //return bitmap;
        }
        /// <summary>
        /// QRコードのURIを作る
        /// </summary>
        /// <param name="twitter">The twitter.</param>
        /// <returns></returns>
        private static UriBuilder buildQrCodeUri(string twitterUri)
        {
            var qrCodeUri = new UriBuilder("http", "www.it-top.biz");
            qrCodeUri.Path = "/qr/qrapi/";
            qrCodeUri.Query = "d=" + twitterUri + "&s=3&q=3";
            return qrCodeUri;
        }

        /// <summary>
        /// Builds the twitter URI.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        private Uri buildTwitterUri(string userId)
        {
            var twitterUrl = new UriBuilder("https", "api.twitter.com");
            twitterUrl.Path = "/1/statuses/user_timeline.json";
            var screeNameQuery = "screen_name=" + userId;
            var countQuery = "count=20";
            twitterUrl.Query = screeNameQuery + "&" + countQuery;
            return twitterUrl.Uri;
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