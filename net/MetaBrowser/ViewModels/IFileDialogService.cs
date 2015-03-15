namespace MetaBrowser.ViewModels
{
    public interface IFileDialogService
    {
        string Title { get; set; }
        string FileName { get; }
        string Filter { get; set; }
        bool ShowDialog();
    }
}