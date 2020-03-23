using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using MonoGame.Framework;

namespace MGNamespace
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.FullUser,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public class GameFrameActivity : AndroidGameActivity
    {
        private GameFrame _game;
        private View _view;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _game = new Game1();
            _view = _game.Services.GetService<View>();

            SetContentView(_view);
            _game.Run();
        }
    }
}
