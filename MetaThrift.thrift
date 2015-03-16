namespace csharp MetaThrift
namespace java inc.awesome.metathrift

exception ArgumentException { 1: string reason }
exception ServiceException { 1: string reason }
exception CanceledException { 1: string reason }

struct MetaObject { 1: string typeName, 2: string data }

struct MetaOperation
{
  1: string name,
  2: optional string inputTypeName, // null = void
  3: optional string outputTypeName, // null = void
  4: optional string displayName,
  5: optional string description
}

service MetaService
{
  string getName() throws (1:ServiceException sEx),
  string getDisplayName() throws (1:ServiceException sEx),
  string getDescription() throws (1:ServiceException sEx),

  void ping() throws (1:ServiceException sEx),
  list<MetaOperation> getOperations() throws (1:ServiceException sEx),
  MetaObject call(1:MetaOperation operation, 2:MetaObject input) throws (1:ArgumentException argEx, 2:ServiceException sEx, 3:CanceledException cEx)
}

struct MetaServiceInfo
{
  1: string name,
  2: string url, // e.g. tcp://<host>:<port>, http://<host>[:port]/directory/, file://<path>
  3: optional string displayName,
  4: optional string description
}

service MetaBroker extends MetaService
{
	void bind(1: MetaServiceInfo serviceInfo) throws (1:ArgumentException argEx, 2:ServiceException sEx),
	void unbind(1: string serviceName) throws (1:ArgumentException argEx, 2:ServiceException sEx),
	MetaServiceInfo getInfo(1:string serviceName) throws (1:ArgumentException argEx, 2:ServiceException sEx),
	list<MetaServiceInfo> getInfos() throws (1:ServiceException sEx)
}
