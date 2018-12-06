/// <reference path="MetaThrift.d.ts" />
//require(['thrift.min', 'MetaThrift_types.min','MetaService.min','MetaBroker.min'],
//    function (thrift, types, metaService, metaBroker) { });
var MetaThrift;
(function (MetaThrift) {
    function unwrap(operation) {
        var unwrapped = new MetaOperation(operation);
        var value = operation.name;
        unwrapped.name = value.substring(value.indexOf("/") + 1);
        return unwrapped;
    }
    MetaThrift.unwrap = unwrap;
    function prettyPrint(operation) {
        return (!isNullOrEmpty(operation.outputTypeName) ? operation.outputTypeName : "void")
            + " " + operation.name + "("
            + (!isNullOrEmpty(operation.inputTypeName) ? operation.inputTypeName + " value" : "")
            + ");"
            + (!isNullOrEmpty(operation.description) ? " // " + operation.description : "");
    }
    MetaThrift.prettyPrint = prettyPrint;
    function isNullOrEmpty(value) {
        return !(value && value !== "");
    }
    MetaThrift.isNullOrEmpty = isNullOrEmpty;
    function createProtocol(url) {
        var transport = new Thrift.Transport(url);
        //if (!transport.isOpen())
        //    transport.open();
        return new Thrift.Protocol(transport);
    }
    MetaThrift.createProtocol = createProtocol;
})(MetaThrift || (MetaThrift = {}));
//# sourceMappingURL=MetaThrift.js.map