"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.reducer = exports.actionCreators = void 0;
// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).
//WHAT IS AVAILABLE TO THE FRONT END!!!!!!!!!!!!!!!!!!!!!
exports.actionCreators = {
    updatePlayerPos: function () { return ({ type: 'UPDATE_PLAYER_POS' }); },
};
// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
var defaultState = { playerPosX: 0.0, playerPosY: 0.0 };
exports.reducer = function (state, incomingAction) {
    if (state === undefined) {
        return defaultState;
    }
    var action = incomingAction;
    switch (action.type) {
        case 'UPDATE_PLAYER_POS':
            return { playerPosX: 100.0, playerPosY: 100.0 };
        default:
            return state;
    }
};
//# sourceMappingURL=PlayerOutputProcessor.js.map