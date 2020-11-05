"use strict";
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.reducer = exports.actionCreators = void 0;
var SingalR_1 = require("../SignalR/SingalR");
var SignalRActionsList = ["SEND_MESSAGE", "RECEIVED_MESSAGE"];
// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).
exports.actionCreators = {
    setUsername: function (name) { return ({ type: 'SET_USERNAME', username: name }); },
    setInputMessage: function (inputMessage) { return ({ type: 'SET_INPUT_MESSAGE', inputmessage: inputMessage }); },
    initialize: function () { return function (dispatch, getState) {
        // Only load data if it's something we don't already have (and are not already loading)
        var appState = getState();
        console.log(appState);
        if (appState && appState.globalchat) {
            var setupEventsHub = SingalR_1.setupSignalRConnection('/hubs/globalChat', SignalRActionsList)(dispatch);
            dispatch({ type: 'INITIALIZED', connection: setupEventsHub });
        }
    }; },
    frontendSentMessage: function (inputMessage) { return function (dispatch, getState) {
        var appState = getState();
        if (appState && appState.globalchat && appState.globalchat.connection) {
            var messageDTO = {
                user: appState.globalchat.username,
                message: inputMessage,
                timestamp: new Date().toLocaleString()
            };
            appState.globalchat.connection && appState.globalchat.connection.invoke("SendMessage", messageDTO);
            dispatch({ type: 'SEND_MESSAGE', messageText: inputMessage });
        }
    }; }
};
// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
var unloadedState = { messages: [], inputMessage: "", isLoading: false, username: "" };
exports.reducer = function (state, incomingAction) {
    if (state === undefined) {
        return unloadedState;
    }
    var action = incomingAction;
    switch (action.type) {
        case 'INITIALIZED':
            return { messages: [], inputMessage: "", isLoading: false, username: "", connection: action.connection };
        case 'SEND_MESSAGE':
            return __assign(__assign({}, state), { inputMessage: "" });
        case 'RECEIVED_MESSAGE':
            return __assign(__assign({}, state), { messages: __spreadArrays(state.messages, [action.message]) });
        case 'SET_USERNAME':
            return __assign(__assign({}, state), { username: action.username });
        case 'SET_INPUT_MESSAGE':
            return __assign(__assign({}, state), { inputMessage: action.inputmessage });
        default:
            return state;
    }
};
//# sourceMappingURL=GlobalChat.js.map