import { Action, Reducer } from 'redux';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface GameMainMenuTogglerState {
    inGame: boolean;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
// Use @typeName and isActionType for type detection that works even after serialization/deserialization.

export interface ToggleGameAction {
    type: 'TOGGLE_GAME';
}



// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
export type KnownAction = ToggleGameAction;


// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

//WHAT IS AVAILABLE TO THE FRONT END!!!!!!!!!!!!!!!!!!!!!
export const actionCreators = {

    toggleGame: () => ({ type: 'TOGGLE_GAME' } as ToggleGameAction)
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const defaultState: GameMainMenuTogglerState = { inGame: false };

export const reducer: Reducer<GameMainMenuTogglerState> = (state: GameMainMenuTogglerState | undefined, incomingAction: Action): GameMainMenuTogglerState => {
    if (state === undefined) {
        return defaultState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'TOGGLE_GAME':
            return {inGame: !state.inGame};
        default:
            return state;
    }
};