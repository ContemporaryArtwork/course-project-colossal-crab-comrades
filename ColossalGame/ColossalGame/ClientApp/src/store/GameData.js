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
Object.defineProperty(exports, "__esModule", { value: true });
exports.reducer = exports.actionCreators = exports.Direction = void 0;
var SingalR_1 = require("../SignalR/SingalR");
var Direction;
(function (Direction) {
    Direction[Direction["Down"] = 0] = "Down";
    Direction[Direction["Up"] = 1] = "Up";
    Direction[Direction["Left"] = 2] = "Left";
    Direction[Direction["Right"] = 3] = "Right";
})(Direction = exports.Direction || (exports.Direction = {}));
var SignalRActionsList = ["RECEIVED_STRING", "RECEIVE_TOKEN", "RECEIVE_POSITIONS_UPDATE"];
// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).
exports.actionCreators = {
    setUsername: function (name) { return ({ type: 'SET_USERNAME', username: name }); },
    initialize: function () { return function (dispatch, getState) {
        // Only load data if it's something we don't already have (and are not already loading)
        var appState = getState();
        console.log(appState);
        if (appState && appState.gameData) { // might be caps
            var setupEventsHub = SingalR_1.setupSignalRConnection('/hubs/gamedata', SignalRActionsList)(dispatch);
            dispatch({ type: 'INITIALIZED', connection: setupEventsHub });
        }
    }; },
    tempLogin: function (username, password) { return function (dispatch, getState) {
        var appState = getState();
        if (appState && appState.gameData && appState.gameData.connection) {
            appState.gameData.connection && appState.gameData.connection.invoke("TempLogin", username, password);
            dispatch({ type: 'TEMP_LOGIN_SENT', username: username, password: password });
        }
    }; },
    sendMovementAction: function (direction) { return function (dispatch, getState) {
        var appState = getState();
        if (appState && appState.gameData && appState.gameData.connection && appState.gameData.token) {
            //TEMPORARY
            //let cookieVal = getCookie("auth-token") as string;
            // tokenString = (cookieVal !== undefined) ? cookieVal : "";
            //console.log("about to send movement");
            var movementActionDTO = {
                Username: "admin1" /*appState.gameData.playerData.username*/,
                Token: appState.gameData.token,
                Direction: direction
            };
            appState.gameData.connection && appState.gameData.connection.invoke("SendMovement", movementActionDTO);
            dispatch({ type: 'SEND_MOVEMENT', direction: direction });
        }
    }; }
};
// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
var unloadedState = {
    isLoading: false, playerData: {
        username: "", position: { coords: { x: 0, y: 0 }, direction: Direction.Right }
    }
};
exports.reducer = function (state, incomingAction) {
    if (state === undefined) {
        return unloadedState;
    }
    var action = incomingAction;
    switch (action.type) {
        case 'INITIALIZED':
            return {
                isLoading: false,
                connection: action.connection,
                playerData: {
                    username: "", position: { coords: { x: 0, y: 0 }, direction: Direction.Right }
                }
            };
        case 'TEMP_LOGIN_SENT':
            return __assign(__assign({}, state), { playerData: __assign(__assign({}, state.playerData), { username: action.username }) });
        case 'RECEIVE_TOKEN':
            return __assign(__assign({}, state), { token: action.token });
        case 'RECEIVE_POSITIONS_UPDATE':
            return __assign(__assign({}, state), { currentGameState: { playerDict: action.playerDict, objectList: action.objectList } });
        case 'SEND_MOVEMENT':
            return state;
        case 'SET_USERNAME':
            return __assign(__assign({}, state), { playerData: __assign(__assign({}, state.playerData), { username: action.username }) });
        default:
            return state;
    }
};
//# sourceMappingURL=GameData.js.map