namespace MetaBrowser.Models.Entities
{
    public interface IMetaService
    {
        string Name { get; }
        string DisplayName { get; }
        string Description { get; }
        MetaOperation[] Operations { get; }
        MetaObject Call(MetaOperation operation, MetaObject input);
    }
}