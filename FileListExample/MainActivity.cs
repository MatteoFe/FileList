using Android.App;
using Android.Widget;
using Android.OS;
using FileList;

namespace ProvaFileList
{
    [Activity(Label = "FileList Example", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        CheckBox CB;
        FileListView fileListView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            // using in runtime style
            // fileListView = new FileListView(Application.Context);
            // FindViewById<LinearLayout>(Resource.Id.MainLayout).AddView(fileListView);

            fileListView = FindViewById<FileListView>(Resource.Id.fileListView);
            fileListView.ItemClick += FileListView_ItemClick;
            fileListView.DirChange += FileListView_DirChange;

            CB = FindViewById<CheckBox>(Resource.Id.checkBoxShowDirectory);
            CB.CheckedChange += CB_CheckedChange;
        }

        private void FileListView_DirChange(object sender, string NewDir)
        {
            Toast.MakeText(this, $"Changing to:{NewDir}", ToastLength.Short).Show();
        }

        private void FileListView_ItemClick(object sender, string SelectedFile)
        {
            Toast.MakeText(this, $"You selected:{SelectedFile}", ToastLength.Short).Show();
        }

        private void CB_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            fileListView.ShowCurrentDir = CB.Checked;
        }

    }
}

