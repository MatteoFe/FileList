
namespace FileList.Model
{
    using Android.Widget;
    using Java.Lang;
    public class FileListRowViewHolder : Object
    {
        public FileListRowViewHolder(ImageView imageView, TextView textView, TextView textInfo_Size, TextView textInfo_Attribute, TextView textInfo_DateTime )
        {
            TextViewFileName = textView;
            TextViewInfo_Size = textInfo_Size;
            TextViewInfo_Attribute = textInfo_Attribute;
            TextViewInfo_DateTime = textInfo_DateTime;
            ImageView = imageView;
        }

        public ImageView ImageView { get; private set; }
        public TextView TextViewFileName { get; private set; }
        public TextView TextViewInfo_Size { get; private set; }
        public TextView TextViewInfo_Attribute { get; private set; }
        public TextView TextViewInfo_DateTime { get; private set; }

        /// <summary>
        ///   This method will update the TextView and the ImageView that are
        ///   are
        /// </summary>
        /// <param name="fileName"> </param>
        /// <param name="fileImageResourceId"> </param>
        public void Update(int fileImageResourceId, string fileName, string Size, string Attribute, string DateTime)
        {
            TextViewFileName.Text = fileName;
            TextViewInfo_Size.Text = Size;
            TextViewInfo_Attribute.Text = Attribute;
            TextViewInfo_DateTime.Text = DateTime;
            ImageView.SetImageResource(fileImageResourceId);
        }
    }
}