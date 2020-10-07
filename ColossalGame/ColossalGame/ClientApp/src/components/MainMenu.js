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
require("./MainMenu.css");
//Redux Imports
var react_redux_1 = require("react-redux");
var GameMainMenuTogglerStore = require("../store/GameMainMenuToggler");
var BGVideo_mp4_1 = require("../assets/mainMenu/BGVideo.mp4");
var Settings_Button_png_1 = require("../assets/mainMenu/Settings Button.png");
var SettingsPanel_png_1 = require("../assets/mainMenu/SettingsPanel.png");
var MainMenu = /** @class */ (function (_super) {
    __extends(MainMenu, _super);
    function MainMenu() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    MainMenu.prototype.ToggleSettings = function () {
        var settingsPanel = document.getElementsByClassName("settings");
        if (settingsPanel[0].style.display === "none") {
            settingsPanel[0].style.display = "block";
        }
        else {
            settingsPanel[0].style.display = "none";
        }
    };
    MainMenu.prototype.SignIn = function () {
        var log = document.getElementsByClassName("loginPanel");
        if (log[0].style.display === "none") {
            log[0].style.display = "block";
        }
        else {
            log[0].style.display = "none";
        }
    };
    MainMenu.prototype.SignUp = function () {
        var signUp = document.getElementsByClassName("registerPanel");
        var log = document.getElementsByClassName("loginPanel");
        if (signUp[0].style.display === "none") {
            signUp[0].style.display = "block";
            log[0].style.display = "none";
        }
        else {
            signUp[0].style.display = "none";
        }
    };
    MainMenu.prototype.SendData = function () {
        var signUp = document.getElementsByClassName("registerPanel");
        var log = document.getElementsByClassName("loginPanel");
        if (signUp[0].style.display === "none") {
            signUp[0].style.display = "none";
            log[0].style.display = "none";
        }
        else {
            signUp[0].style.display = "none";
        }
    };
    MainMenu.prototype.OpenLoadout = function () {
        var load = document.getElementsByClassName("loadout");
        if (load[0].style.display === "none") {
            load[0].style.display = "block";
        }
        else {
            load[0].style.display = "none";
        }
    };
    MainMenu.prototype.ChooseHeavy = function () {
        var heav = document.getElementsByClassName("classColor");
        var load = document.getElementsByClassName("loadout");
        heav[0].style.backgroundColor = "purple";
        load[0].style.display = "none";
    };
    MainMenu.prototype.ChooseBrawler = function () {
        var braw = document.getElementsByClassName("classColor");
        var load = document.getElementsByClassName("loadout");
        braw[0].style.backgroundColor = "red";
        load[0].style.display = "none";
    };
    MainMenu.prototype.ChooseBuilder = function () {
        var bui = document.getElementsByClassName("classColor");
        var load = document.getElementsByClassName("loadout");
        bui[0].style.backgroundColor = "yellowgreen";
        load[0].style.display = "none";
    };
    MainMenu.prototype.render = function () {
        var _this = this;
        return (React.createElement("body", null,
            React.createElement("section", { className: "enclosing" },
                React.createElement("button", { className: "settingsButton", onClick: this.ToggleSettings },
                    React.createElement("img", { src: Settings_Button_png_1.default })),
                React.createElement("div", { className: "settings" },
                    " ",
                    React.createElement("img", { src: SettingsPanel_png_1.default }),
                    "  "),
                React.createElement("button", { className: "loginOpen", onClick: this.SignIn }, " Login "),
                React.createElement("div", { className: "loginPanel" },
                    React.createElement("div", { className: "signContainer" },
                        React.createElement("label", null, "Email"),
                        React.createElement("br", null),
                        React.createElement("input", { type: "text", placeholder: "Email" }),
                        React.createElement("br", null),
                        React.createElement("label", null, "Password"),
                        React.createElement("br", null),
                        React.createElement("input", { type: "text", placeholder: "Password" }),
                        React.createElement("br", null),
                        React.createElement("button", { className: "loginButton", onClick: this.SendData }, " LOGIN "),
                        React.createElement("button", { className: "registerButton", onClick: this.SignUp }, "Don't have an account? Click here to register"))),
                React.createElement("div", { className: "registerPanel" },
                    React.createElement("div", { className: "signContainer" },
                        React.createElement("label", null, "Email"),
                        React.createElement("br", null),
                        React.createElement("input", { type: "text", placeholder: "Email" }),
                        React.createElement("br", null),
                        React.createElement("label", null, "Password"),
                        React.createElement("br", null),
                        React.createElement("input", { type: "text", placeholder: "Password" }),
                        React.createElement("br", null),
                        React.createElement("label", null, "Confirm Password"),
                        React.createElement("br", null),
                        React.createElement("input", { type: "text", placeholder: "Repeat Password" }),
                        React.createElement("br", null),
                        React.createElement("button", { className: "loginButton", onClick: this.SendData }, " REGISTER "))),
                React.createElement("div", { className: "video-container" },
                    React.createElement("video", { src: BGVideo_mp4_1.default, loop: true, autoPlay: true, muted: true })),
                React.createElement("div", { className: "content" },
                    React.createElement("h1", null, "Pre Alpha CSE 442 Project"),
                    React.createElement("h3", null, "Founders (A-Z): Eoghan Mccarroll, Jacob Santoni, Joshua Lacey, Kyle Pellechia, Zachary Wagner "),
                    React.createElement("button", { className: "classSelectButton", onClick: this.OpenLoadout }, " Select Class "),
                    React.createElement("div", { className: "classColor" }),
                    React.createElement("button", { className: "classSelectButton", onClick: function () { _this.props.toggleGame(); } }, " Start Game ")),
                React.createElement("div", { className: "loadout" },
                    React.createElement("div", { className: "signContainer" },
                        React.createElement("div", { className: "classContainer" },
                            React.createElement("div", { className: "Heavy" }),
                            React.createElement("button", { className: "loadButton", onClick: this.ChooseHeavy }, "HEAVY")),
                        React.createElement("div", { className: "classContainer" },
                            React.createElement("div", { className: "Brawler" }),
                            React.createElement("button", { className: "loadButton", onClick: this.ChooseBrawler }, "BRAWLER")),
                        React.createElement("div", { className: "classContainer" },
                            React.createElement("div", { className: "Builder" }),
                            React.createElement("button", { className: "loadButton", onClick: this.ChooseBuilder }, "BUILDER")))))));
    };
    return MainMenu;
}(React.PureComponent));
exports.default = react_redux_1.connect(function (state) { return state.gameMainMenuToggler; }, GameMainMenuTogglerStore.actionCreators)(MainMenu);
//# sourceMappingURL=MainMenu.js.map