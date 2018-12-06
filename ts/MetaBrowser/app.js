/// <reference path="Scripts/typings/knockout/knockout.d.ts" />
/// <reference path="Scripts/ServicesViewModel.ts" />
//declare var require;
//require(['knockout-3.3.0', 'Scripts/ViewModels.min'], function(ko, appViewModel) {
//    ko.applyBindings(appViewModel);
//});
var appViewModel = new ServicesViewModel();
ko.applyBindings(appViewModel);
//# sourceMappingURL=app.js.map