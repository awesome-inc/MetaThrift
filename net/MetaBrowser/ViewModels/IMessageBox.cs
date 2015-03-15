using System.ComponentModel.Composition;

namespace MetaBrowser.ViewModels 
{
    public interface IMessageBox
    {
        void Show(string message, string caption);
    }
}