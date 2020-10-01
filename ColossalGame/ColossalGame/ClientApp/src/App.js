"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var react_router_1 = require("react-router");
var GlobalChat_1 = require("./components/GlobalChat");
var Login_1 = require("./components/Login");
var Signup_1 = require("./components/Signup");
require("./custom.css");
var GameMainMenuToggler_1 = require("./components/GameMainMenuToggler");
exports.default = (function () { return (React.createElement("body", null,
    React.createElement(react_router_1.Route, { exact: true, path: '/', component: GameMainMenuToggler_1.default }),
    React.createElement(react_router_1.Route, { exact: true, path: '/globalchat', component: GlobalChat_1.default }),
    React.createElement(react_router_1.Route, { path: '/login', component: Login_1.default }),
    React.createElement(react_router_1.Route, { path: '/signup', component: Signup_1.default }))); });
//# sourceMappingURL=App.js.map