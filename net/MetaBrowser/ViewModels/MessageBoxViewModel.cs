using System.ComponentModel.Composition;

namespace MetaBrowser.ViewModels 
{
    [Export(typeof(IMessageBox)), PartCreationPolicy(CreationPolicy.NonShared)]
    class DefaultMessageBox : IMessageBox
    {
        public void Show(string message, string caption)
        {
            System.Windows.MessageBox.Show(message, caption);
        }
    }
}