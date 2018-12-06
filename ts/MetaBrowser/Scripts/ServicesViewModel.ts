/// <reference path="typings/knockout/knockout.d.ts" />
/// <reference path="MetaThrift.ts" />

//declare var require;
//require(['MetaThrift.min'], function () { });

//declare var define;
//define(function () { return new ServicesViewModel(); });

class ServicesViewModel {
	private readonly broker: MetaBrokerClient;
	
	brokerUri: KnockoutObservable<string>;
	validUri: KnockoutComputed<boolean>;

	services: KnockoutObservableArray<ServiceViewModel>;
	selectedService: KnockoutObservable<ServiceViewModel>;
	
	selectedOperation: KnockoutObservable<OperationViewModel>;
	operationSelected: KnockoutComputed<boolean>;

	inputData: KnockoutObservable<string>;
	outputData: KnockoutObservable<string>;

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
			if (operations.length === 0) {
				alert("No MetaServices registered.");
				return;
			}
			var infos = this.broker.getInfos();
			var services = ServicesViewModel.filterServices(infos, operations);

			this.services(services);

		}, "Could not refresh services");
	}

	showExecuteModal() {
		this.inputData("");
		this.outputData("");
	}

	executeOperation() {
		var operation = this.selectedOperation().operation;
		UiHelper.handledCall(() => {
			var input = new MetaObject();
			input.typeName = operation.inputTypeName;
			input.data = this.inputData();
			var output = this.broker.call(operation, input);
			this.outputData(output.data);
		}, "Could not execute operation");
	}

	static filterServices(infos: MetaServiceInfo[], operations: MetaOperation[]): ServiceViewModel[] {
	    const services = infos.map(info => {
	        var ops = operations.
	            filter(o => o.name.indexOf(info.name + "/") === 0);
	        return new ServiceViewModel(info, ops);
	    });
	    return services;
    }
}
