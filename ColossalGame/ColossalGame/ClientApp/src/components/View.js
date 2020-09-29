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
var MainMenu_1 = require("./MainMenu");
var View = /** @class */ (function (_super) {
    __extends(View, _super);
    function View() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.state = {
            inGame: true
        };
        return _this;
    }
    View.prototype.render = function () {
        if (!this.state.inGame) {
            return (React.createElement("div", null, "yeeheeyee"));
        }
        else {
            return (React.createElement(MainMenu_1.default, null));
        }
    };
    return View;
}(React.Component));
exports.default = View;
//# sourceMappingURL=View.js.map