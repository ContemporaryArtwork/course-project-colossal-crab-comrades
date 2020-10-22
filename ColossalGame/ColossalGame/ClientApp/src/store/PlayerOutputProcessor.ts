import { Action, Reducer } from 'redux';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface PlayerOutputProcessorState {
    playerPosX: number;
    playerPosY: number;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
// Use @typeName and isActionType for type detection that works even after serialization/deserialization.

export interface UpdatePlayerPosAction {
    type: 'UPDATE_PLAYER_POS';
    playerPosX: number;
    playerPosY: number;
}


// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
export type KnownAction = UpdatePlayerPosAction;


// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

//WHAT IS AVAILABLE TO THE FRONT END!!!!!!!!!!!!!!!!!!!!!
export const actionCreators = {

    updatePlayerPos: () => ({ type: 'UPDATE_PLAYER_POS' } as UpdatePlayerPosAction),
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const defaultState: PlayerOutputProcessorState = { playerPosX: 0.0, playerPosY: 0.0 };

export const reducer: Reducer<PlayerOutputProcessorState> = (state: PlayerOutputProcessorState | undefined, incomingAction: Action): PlayerOutputProcessorState => {
    if (state === undefined) {
        return defaultState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'UPDATE_PLAYER_POS':
            return { playerPosX: 100.0, playerPosY: 100.0 };
        default:
            return state;
    }
};