"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var react_redux_1 = require("react-redux");
var PlayerOutputProcessorStore = require("../store/PlayerOutputProcessor");
var PlayerOutputProcessor = /** @class */ (function (_super) {
    __extends(PlayerOutputProcessor, _super);
    function PlayerOutputProcessor() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    PlayerOutputProcessor.prototype.render = function () {
        return (React.createElement("div", null, "hi"));
    };
    return PlayerOutputProcessor;
}(React.PureComponent));
;
exports.default = react_redux_1.connect(function (state) { return state.playerOutputProcessor; }, PlayerOutputProcessorStore.actionCreators)(PlayerOutputProcessor);
//# sourceMappingURL=PlayerOutputProcessor.js.map