using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using System.IO;
using Microsoft.Xna.Framework;
using Android.Runtime;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace mobiletests
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Landscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public class Activity1 : AndroidGameActivity
    {
        private Game1 _game;
        private View _view;

        protected override void OnCreate(Bundle bundle)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
            }
            base.OnCreate(bundle);
            ToggleFullscreen();

            var files = new MobileScan { gameActivity = this }.GetFiles("Content");

            _game = new Game1 {  };
            _view = _game.Services.GetService(typeof(View)) as View;
            _view.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.LayoutStable | SystemUiFlags.LayoutHideNavigation | SystemUiFlags.LayoutFullscreen | SystemUiFlags.HideNavigation | SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky);

            SetContentView(_view);
            _game.Run();
        }
        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            _view.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.LayoutStable | SystemUiFlags.LayoutHideNavigation | SystemUiFlags.LayoutFullscreen | SystemUiFlags.HideNavigation | SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky);

        }
        void ToggleFullscreen()
        {

            int uiOptions = (int)Window.DecorView.SystemUiVisibility;

            uiOptions |= (int)SystemUiFlags.LowProfile;
            uiOptions |= (int)SystemUiFlags.Fullscreen;
            uiOptions |= (int)SystemUiFlags.HideNavigation;
            uiOptions |= (int)SystemUiFlags.ImmersiveSticky;

            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
        }

        class MobileScan
        {
            public AndroidGameActivity gameActivity;

            bool IsFolder(string path)
            {
                //return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
                return !Path.HasExtension(path);
            }

            public string[] GetFiles(string path)
            {
                var pack = gameActivity.Assets.List(path);
                var b = new List<string>();

                foreach (var n in pack)
                {
                    if (IsFolder(n))
                        b.InsertRange(b.Count, GetFiles($"{path}/{n}"));
                    else
                        b.Add($"{path}/{n}");
                }
                //List<string> pack = new List<string>();
                return b.ToArray();
            }
        }
    }
}
