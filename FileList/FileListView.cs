using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using FileList.Adapter;
using System.IO;
using Android.OS;

namespace FileList
{
    public class FileListView : LinearLayout
    {
        private static readonly string RootDir = "/";
        private readonly string KEY_CURRENTDIR = "Current_Dir";
        private readonly string KEY_SHOWHIDDENFILES = "Show_Hidden_Files";
        private readonly string KEY_SHOWCURRENTDIR = "Show_Current_Dir";
        private readonly string KEY_PARENT_STATE = "Linear_Layout_Parent";

        private bool _showHiddenFiles = true;
        public bool ShowHiddenFiles
        {
            get { return _showHiddenFiles; }
            set
            {
                _showHiddenFiles = value;
                RefreshFilesList();
            }
        }
        private bool _showCurrentDir;
        public bool ShowCurrentDir
        {
            get { return _showCurrentDir; }
            set
            {
                _showCurrentDir = value;
                if (_currentDir_TextView != null)
                {
                    _currentDir_TextView.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
                    _currentDir_TextView.Invalidate();
                    RequestLayout();
                }

                RefreshFilesList();
            }
        }
        private string _currentDir = RootDir;
        public string CurrentDir
        {
            get { return _currentDir; }
            set
            {
                var OldDir = _currentDir;
                _currentDir = value;
                if (RefreshFilesList())
                {
                    DirChange?.Invoke(this, _currentDir);
                }
                else { _currentDir = OldDir; } //If cannot change to new dir then undo
                if (ShowCurrentDir)
                {
                    _currentDir_TextView.Text = _currentDir;
                }
            }
        }

        private TextView _currentDir_TextView;
        private ListView _fileList_ListView;
        private FileListAdapter _fileListAdapter;

        public FileListView(Context context) : base(context) => Initialize(context, null, 0);
        public FileListView(Context context, IAttributeSet attrs) : base(context, attrs) => Initialize(context, attrs, 0);
        public FileListView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) => Initialize(context, attrs, defStyle);

        private void Initialize(Context context, IAttributeSet attrs, int defStyle)
        {
            InitializeStyleAttributeProperties(context, attrs);
            InflateLayout(context);
            RefreshFilesList();
        }

        private void InflateLayout(Context context)
        {
            var inflater = LayoutInflater.FromContext(context);
            inflater.Inflate(Resource.Layout.FileListViewWithDirName, this);

            _currentDir_TextView = FindViewById<TextView>(Resource.Id.FileList_CurrentDir);
            _fileList_ListView = FindViewById<ListView>(Resource.Id.FileList_ListView);
            _fileList_ListView.Adapter = _fileListAdapter = new FileListAdapter(context, new FileSystemInfo[0]);
            _fileList_ListView.ItemClick += _fileList_ListView_ItemClick;
        }

        protected override IParcelable OnSaveInstanceState()
        {
            var bundle = new Bundle();
            bundle.PutParcelable(KEY_PARENT_STATE, base.OnSaveInstanceState());
            bundle.PutString(KEY_CURRENTDIR, _currentDir);
            bundle.PutBoolean(KEY_SHOWCURRENTDIR, _showCurrentDir);
            bundle.PutBoolean(KEY_SHOWHIDDENFILES, _showHiddenFiles);
            return bundle;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            var bundle = state as Bundle;
            _showCurrentDir = bundle.GetBoolean(KEY_SHOWCURRENTDIR);
            _showHiddenFiles = bundle.GetBoolean(KEY_SHOWHIDDENFILES);
            CurrentDir = bundle.GetString(KEY_CURRENTDIR, "");
            state = (IParcelable)bundle.GetParcelable(KEY_PARENT_STATE);
            base.OnRestoreInstanceState(state);

            RefreshFilesList();
        }
        private void _fileList_ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var fileSystemInfo = _fileListAdapter.GetItem(e.Position);
            if (fileSystemInfo.Name == "..")
            {
                fileSystemInfo = Directory.GetParent(_currentDir);
            }

            if (fileSystemInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                // Dig into this directory, and display it's contents
                CurrentDir = fileSystemInfo.FullName;
                DirChange?.Invoke(this, fileSystemInfo.FullName);
            }
            else
            {
                // Do something with the file.
                ItemClick?.Invoke(this, fileSystemInfo.FullName);
            }
            // throw new NotImplementedException();
        }

        public delegate void ItemClickHandler(object sender, string SelectedFile);
        public event ItemClickHandler ItemClick;
        public delegate void DirChangeHandler(object sender, string NewDir);
        public event DirChangeHandler DirChange;

        public bool RefreshFilesList()
        {
            if (_currentDir != null && _fileListAdapter != null)
            {
                IList<FileSystemInfo> visibleThings = new List<FileSystemInfo>();
                var dir = new DirectoryInfo(_currentDir);
                try
                {
                    var itemList = dir.GetFileSystemInfos().Where(x => true);
                    if (!_showHiddenFiles) itemList = itemList.Where(item => !item.Attributes.HasFlag(FileAttributes.Hidden));
                    if (!ShowCurrentDir)
                    {
                        itemList = itemList.Where(item => !item.Attributes.HasFlag(FileAttributes.Directory)); // Se necessario filtra le directory
                    }
                    else
                    {
                        if (dir.FullName != RootDir)
                        {
                            visibleThings.Add(new FileInfo(".."));
                        }
                    }
                    foreach (var item in itemList)
                    {
                        visibleThings.Add(item);
                    }
                }
                catch (Exception)
                {
                    Toast.MakeText(Context, $"{Resources.GetString(Resource.String.ToastDirectoryContentsError)} {_currentDir}", ToastLength.Long).Show();
                    return false;
                }

                _fileListAdapter?.AddDirectoryContents(visibleThings);

                // If we don't do this, then the ListView will not update itself when then data set 
                // in the adapter changes. It will appear to the user that nothing has happened.
                _fileList_ListView.RefreshDrawableState();
                //if (_container != null)
                //{
                //}
                return true;
            }
            return false;
        }
        private void InitializeStyleAttributeProperties(Context context, IAttributeSet attrs)
        {
            if (context == null)
            {
                return;
            }

            var typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.FileListView, 0, 0);
            InitializeDateFromCustomViewAttributes(typedArray);
            typedArray.Recycle();
        }
        private void InitializeDateFromCustomViewAttributes(Android.Content.Res.TypedArray typedArray)
        {
            ShowCurrentDir = typedArray.GetBoolean(Resource.Styleable.FileListView_showHiddenFiles, true);
            ShowHiddenFiles = typedArray.GetBoolean(Resource.Styleable.FileListView_showHiddenFiles, true);
        }
    }
}