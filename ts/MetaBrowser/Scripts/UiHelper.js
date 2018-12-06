var UiHelper = /** @class */ (function () {
    function UiHelper() {
    }
    UiHelper.handledCall = function (action, couldNot) {
        try {
            action();
        }
        catch (ex) {
            alert(couldNot + ": " + ex.name + " " + ex.message);
        }
    };
    return UiHelper;
}());
//# sourceMappingURL=UiHelper.js.map