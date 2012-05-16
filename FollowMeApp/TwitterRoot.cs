using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace FollowMeApp
{
    public class TwitterRoot
    {
        public string text { get; set; }
        public User user { get; set; }

    }

    public class User
    {
        public string name { get; set; }
        public string screen_name { get; set; }
        public string url { get; set; }
        public bool _protected { get; set; }
        public string profile_image_url { get; set; }
    }
}
