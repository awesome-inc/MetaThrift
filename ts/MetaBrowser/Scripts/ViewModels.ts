/// <reference path="typings/knockout/knockout.d.ts" />
/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/bootstrap/bootstrap.d.ts" />
/// <reference path="MetaThrift.d.ts" />
/// <reference path="MetaThrift.ts" />

//declare var require;
//require(['MetaThrift.min'], function () { });

//declare var define;
//define(function () { return new ServicesViewModel(); });

class UiHelper {
	public static handledCall(action: () => void, couldNot: string) {
		try  {
			action(); 
		}
		catch(ex)  {
			alert(couldNot + ": " + ex.name + " " + ex.message);
		}
	}
}

class OperationViewModel {
	public name: string;
	public prettyName: string;
	constructor(public operation: MetaOperation) {
		var op = MetaThrift.unwrap(operation);
		this.name = op.name;
		this.prettyName = MetaThrift.prettyPrint(op);
	}
}
	
class ServiceViewModel {
	public name: string;
	public operations: OperationViewModel[];
	constructor(public serviceInfo: MetaServiceInfo, operations: MetaOperation[]) {
		this.name = (this.serviceInfo ? this.serviceInfo.name : "");
		this.operations = operations.map(o => new OperationViewModel(o));
	}
	static empty = new ServiceViewModel(null, []);
}

class ServicesViewModel {
	private broker: MetaBrokerClient;
	
	public brokerUri: KnockoutObservable<string>;
	public validUri: KnockoutComputed<boolean>;

	public services: KnockoutObservableArray<ServiceViewModel>;
	public selectedService: KnockoutObservable<ServiceViewModel>;
	
	public selectedOperation: KnockoutObservable<OperationViewModel>;
	public operationSelected: KnockoutComputed<boolean>;

	public inputData: KnockoutObservable<string>;
	public outputData: KnockoutObservable<string>;

	constructor() {
		this.brokerUri = ko.observable("http://localhost:9091/services/metabroker/");
		this.validUri = ko.computed(() => {
			return !MetaThrift.isNullOrEmpty(this.brokerUri());
		});
		this.broker = new MetaBrokerClient(MetaThrift.createProtocol(this.brokerUri()));

		this.services = ko.observableArray<ServiceViewModel>([]);
		this.selectedService = ko.observable(ServiceViewModel.empty);

		this.selectedOperation = ko.observable<OperationViewModel>();
		this.operationSelected = ko.computed(() => {
			return (this.selectedOperation() != null);
		});

		this.inputData = ko.observable("");
		this.outputData = ko.observable("");
	}

	refreshServices() {
		this.services([]);
		this.selectedService(ServiceViewModel.empty);
		this.selectedOperation(null);

		UiHelper.handledCall(() => {
			this.broker.input.getTransport().url = this.brokerUri();
			this.broker.output.getTransport().url = this.brokerUri();

			var operations = this.broker.getOperations();
			if (operations.length == 0) {
				alert("No MetaServices registered.");
				return;
			}
			var infos = this.broker.getInfos();
			var services = ServicesViewModel.filterServices(infos, operations);

			this.services(services);
			//this.selectedService(this.services[0]);
			//this.selectedOperation(services[0].operations[0]);

		}, "Could not refresh services");
	}

	showExecuteModal() {
		this.inputData("");
		this.outputData("");
		$("#executeModal").modal("show");
	}

	executeOperation() {
		var operation : MetaOperation = this.selectedOperation().operation;
		UiHelper.handledCall(() => {
			var input = new MetaObject();
			input.typeName = operation.inputTypeName;
			input.data = this.inputData();
			var output = this.broker.call(operation, input);
			this.outputData(output.data);
		}, "Could not execute operation");
	}

	public static filterServices(infos: MetaServiceInfo[], operations: MetaOperation[]): ServiceViewModel[] {
		var services = infos.map(info => {
			var ops = operations.
				filter(o => o.name.indexOf(info.name + "/") == 0);
			//.map(MetaThrift.unwrap);
			return new ServiceViewModel(info, ops);
		});
		return services;
	}
}
