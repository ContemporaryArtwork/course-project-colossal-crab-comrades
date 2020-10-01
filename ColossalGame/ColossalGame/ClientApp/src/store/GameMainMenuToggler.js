"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.reducer = exports.actionCreators = void 0;
// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).
//WHAT IS AVAILABLE TO THE FRONT END!!!!!!!!!!!!!!!!!!!!!
exports.actionCreators = {
    toggleGame: function () { return ({ type: 'TOGGLE_GAME' }); }
};
// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
var defaultState = { inGame: false };
exports.reducer = function (state, incomingAction) {
    if (state === undefined) {
        return defaultState;
    }
    var action = incomingAction;
    switch (action.type) {
        case 'TOGGLE_GAME':
            return { inGame: !state.inGame };
        default:
            return state;
    }
};
//# sourceMappingURL=GameMainMenuToggler.js.map