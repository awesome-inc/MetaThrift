/// <reference path="MetaThrift.d.ts" />

//require(['thrift.min', 'MetaThrift_types.min','MetaService.min','MetaBroker.min'],
//    function (thrift, types, metaService, metaBroker) { });

module MetaThrift {

	export function unwrap(operation: MetaOperation): MetaOperation {
		const unwrapped = new MetaOperation(operation);
		const value = operation.name;
		unwrapped.name = value.substring(value.indexOf("/") + 1);
		return unwrapped;
	}

	export function prettyPrint(operation: MetaOperation) : string {
		return (!isNullOrEmpty(operation.outputTypeName) ? operation.outputTypeName : "void")
			+ " " + operation.name + "("
			+ (!isNullOrEmpty(operation.inputTypeName) ? operation.inputTypeName + " value" : "")
			+ ");"
			+ (!isNullOrEmpty(operation.description) ? ` // ${operation.description}` : "")
			;
	}

	export function isNullOrEmpty(value: string): Boolean {
		return !(value && value !== "");
	}

	export function createProtocol(url: string): Thrift.Protocol {
	    const transport = new Thrift.Transport(url);
	    //if (!transport.isOpen())
		 //    transport.open();
		 return new Thrift.Protocol(transport);
    }
}
