declare class MetaObject {
    typeName: string;
    data: string;
    constructor(args?: MetaObject);
}

declare class MetaOperation {
    name: string;
    inputTypeName: string;
    outputTypeName: string;
    displayName: string;
    description: string;
    constructor(args?: MetaOperation);
}

declare class MetaServiceClient {
    // thrift default implementation for any client
    input: Thrift.Protocol;
    output: Thrift.Protocol;
    constructor(input: Thrift.Protocol, output?:Thrift.Protocol);
    // specific for MetaThrift
    getName(): string;
    getDisplayName(): string;
    getDescription(): string;
    ping();
    getOperations() : MetaOperation[];
    call(operation: MetaOperation, input: MetaObject) : MetaObject;
}

declare class MetaServiceInfo {
    name: string;
    url: string;
    displayName: string;
    description: string;
    constructor(args?: MetaServiceInfo);
}

declare class MetaBrokerClient extends MetaServiceClient {
    bind(serviceInfo: MetaServiceInfo);
    unbind(serviceName: string);
    getInfo(serviceName: string): MetaServiceInfo;
    getInfos() : MetaServiceInfo[];
}

