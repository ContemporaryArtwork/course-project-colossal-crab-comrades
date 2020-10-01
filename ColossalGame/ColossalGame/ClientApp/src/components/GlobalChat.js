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
var GlobalChatStore = require("../store/GlobalChat");
var GlobalChat = /** @class */ (function (_super) {
    __extends(GlobalChat, _super);
    function GlobalChat() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    GlobalChat.prototype.componentDidMount = function () {
        console.log("About to initialize");
        this.initialize();
    };
    GlobalChat.prototype.initialize = function () {
        this.props.initialize();
    };
    GlobalChat.prototype.render = function () {
        var _this = this;
        return (React.createElement(React.Fragment, null,
            React.createElement("h1", null, "Global Chat"),
            React.createElement("p", null, "This fragment can be easily moved elsewhere since it's an independent component."),
            React.createElement("input", { value: this.props.username, onChange: function (ev) { _this.props.setUsername(ev.target.value); } }),
            React.createElement("input", { value: this.props.inputMessage, onChange: function (ev) { _this.props.setInputMessage(ev.target.value); } }),
            React.createElement("button", { type: "button", className: "btn btn-primary btn-lg", onClick: function () { _this.props.frontendSentMessage(_this.props.inputMessage); } }, "Send Message"),
            this.props.messages && this.props.messages.map(function (m) { return (React.createElement("p", null,
                m.timestamp,
                " <",
                m.user,
                ">: ",
                m.message)); })));
    };
    return GlobalChat;
}(React.PureComponent));
;
exports.default = react_redux_1.connect(function (state) { return state.globalchat; }, GlobalChatStore.actionCreators)(GlobalChat);
//# sourceMappingURL=GlobalChat.js.map