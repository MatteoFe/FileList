using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using FileList.Model;

namespace FileList.Adapter
{
    public class FileListAdapter : ArrayAdapter<FileSystemInfo>
    {
        private readonly Context _context;
        public FileListAdapter(Context context, IList<FileSystemInfo> fsi) : base(context, Resource.Layout.file_picker_list_item, Android.Resource.Id.Text1, fsi) => _context = context;

        public void AddDirectoryContents(IEnumerable<FileSystemInfo> directoryContents)
        {
            Clear();
            // Notify the _adapter that things have changed or that there is nothing to display.
            if (directoryContents.Any())
            {
                AddAll(directoryContents.ToArray());
                NotifyDataSetChanged();
            }
            else
            {
                NotifyDataSetInvalidated();
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var fileSystemEntry = GetItem(position);

            FileListRowViewHolder viewHolder;
            View row;
            if (convertView == null)
            {
                row = LayoutInflater.From(_context).Inflate(Resource.Layout.file_picker_list_item, parent, false);
                viewHolder = new FileListRowViewHolder(
                    imageView: row.FindViewById<ImageView>(Resource.Id.file_picker_image),
                    textView: row.FindViewById<TextView>(Resource.Id.file_picker_text),
                    textInfo_Size: row.FindViewById<TextView>(Resource.Id.file_picker_info_size),
                    textInfo_Attribute: row.FindViewById<TextView>(Resource.Id.file_picker_info_attribute),
                    textInfo_DateTime: row.FindViewById<TextView>(Resource.Id.file_picker_info_datetime));
                row.Tag = viewHolder;
            }
            else
            {
                row = convertView;
                viewHolder = (FileListRowViewHolder)row.Tag;
            }

            var TestoNomeFile = fileSystemEntry.Name;
            var TestoInfoSize = FileSizeString(fileSystemEntry);
            var TestoInfoAttributi = FileAttributeString(fileSystemEntry);
            var TestoInfoDateTime = fileSystemEntry.CreationTime.ToString("G");
            if (fileSystemEntry.Name == "..")
            {
                TestoInfoSize = "";
                TestoInfoAttributi = "";
            }

            viewHolder.Update(fileImageResourceId: fileSystemEntry.Attributes.HasFlag(FileAttributes.Directory) ? Resource.Drawable.folder : Resource.Drawable.file,
                fileName: TestoNomeFile,
                Size: TestoInfoSize,
                Attribute: TestoInfoAttributi,
                DateTime: TestoInfoDateTime);

            return row;
        }

        private string DirContentString(string directory)
        {
            // Show directory content preview
            var dir = new DirectoryInfo(directory);
            try
            {
                return $"{dir.GetFileSystemInfos().Length} {_context.Resources.GetString(Resource.String.Elements)}";

            }
            catch (Exception)
            {
                return _context.Resources.GetString(Resource.String.Folder);
            }
        }
        private string FileAttributeString(FileSystemInfo fileSystemEntry)
        {
            var res = "";
            res += fileSystemEntry.Attributes.HasFlag(FileAttributes.Directory) ? "d" : "-";
            res += fileSystemEntry.Attributes.HasFlag(FileAttributes.ReadOnly) ? "r" : "-";
            res += fileSystemEntry.Attributes.HasFlag(FileAttributes.Hidden) ? "h" : "-";
            return res;
        }
        private string FileSizeString(FileSystemInfo fileSystemEntry)
        {
            long TempFileSize = 0;
            var result = "";
            if (fileSystemEntry != null && !fileSystemEntry.Attributes.HasFlag(FileAttributes.Directory))
            {
                string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
                TempFileSize = Math.Max(1, new FileInfo(fileSystemEntry.FullName).Length);
                var ind = Math.Min(sizes.Length, (int)Math.Truncate(Math.Log(TempFileSize, 1024)));
                result = $"{(TempFileSize / Math.Pow(1024, ind)).ToString("0.##")} {sizes[ind]}";
            }
            else
            {
                result = DirContentString(fileSystemEntry?.FullName);
            }

            return result;
        }
    }
}