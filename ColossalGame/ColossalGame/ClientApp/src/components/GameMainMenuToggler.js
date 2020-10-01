"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
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
var GameMainMenuTogglerStore = require("../store/GameMainMenuToggler");
var MainMenu_1 = require("./MainMenu");
var GameMainMenuToggler = /** @class */ (function (_super) {
    __extends(GameMainMenuToggler, _super);
    function GameMainMenuToggler() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    GameMainMenuToggler.prototype.render = function () {
        return (this.props.inGame ? React.createElement("div", null, "[GAME PLACEHOLDER TEXT]") : React.createElement(MainMenu_1.default, null));
    };
    return GameMainMenuToggler;
}(React.PureComponent));
;
exports.default = react_redux_1.connect(function (state) { return state.gameMainMenuToggler; }, GameMainMenuTogglerStore.actionCreators)(GameMainMenuToggler);
//# sourceMappingURL=GameMainMenuToggler.js.map