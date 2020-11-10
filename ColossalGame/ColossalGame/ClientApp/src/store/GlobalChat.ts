import { Action, Reducer } from 'redux';
import { AppThunkAction } from './';
import { HubConnection } from '@microsoft/signalr';
import { setupSignalRConnection } from "../SignalR/SingalR"

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface Message {
    user: string
    message: string
    timestamp: string
}

export interface GlobalChatState {
    isLoading: boolean;
    messages: Message[],
    inputMessage: string,
    username: string,
    connection?: HubConnection
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
// Use @typeName and isActionType for type detection that works even after serialization/deserialization.

export interface InitializeAction {
    type: 'INITIALIZED';
    connection: HubConnection;
}
export interface SendMessageAction {
    type: 'SEND_MESSAGE';
    messageText: string;
}
export interface SetUsernameAction {
    type: 'SET_USERNAME';
    username: string;
}
export interface SetInputMessageAction {
    type: 'SET_INPUT_MESSAGE';
    inputmessage: string;
}
export interface ReceivedMessageAction {
    type: 'RECEIVED_MESSAGE';
    message: Message;
}


// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
export type KnownAction = InitializeAction | SendMessageAction | SetUsernameAction | SetInputMessageAction | ReceivedMessageAction;

const SignalRActionsList: string[] = ["SEND_MESSAGE", "RECEIVED_MESSAGE"]
// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {

    setUsername: (name: string) => ({ type: 'SET_USERNAME', username: name } as SetUsernameAction),
    setInputMessage: (inputMessage: string) => ({ type: 'SET_INPUT_MESSAGE', inputmessage: inputMessage } as SetInputMessageAction),

    initialize: (): AppThunkAction<KnownAction> => async (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        const appState = getState();
        console.log(appState);
        if (appState && appState.globalchat) {
            const setupEventsHub: HubConnection = await (await setupSignalRConnection('/hubs/globalChat', SignalRActionsList))(dispatch);
            dispatch({ type: 'INITIALIZED', connection: setupEventsHub });
        }
    },

    frontendSentMessage: (inputMessage: string): AppThunkAction<KnownAction> => (dispatch, getState) => {
        const appState = getState();
        if (appState && appState.globalchat && appState.globalchat.connection) {

            const messageDTO: Message = {
                user: appState.globalchat.username,
                message: inputMessage,
                timestamp: new Date().toLocaleString()
            }
            appState.globalchat.connection && appState.globalchat.connection.invoke("SendMessage", messageDTO)

            dispatch({ type: 'SEND_MESSAGE', messageText: inputMessage });
        }
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: GlobalChatState = { messages: [], inputMessage: "", isLoading: false, username: ""};

export const reducer: Reducer<GlobalChatState> = (state: GlobalChatState | undefined, incomingAction: Action): GlobalChatState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'INITIALIZED':
            return { messages: [], inputMessage: "", isLoading: false, username: "", connection: action.connection };
        case 'SEND_MESSAGE':
            return { ...state, inputMessage: "" };
        case 'RECEIVED_MESSAGE':
            return { ...state, messages: [...state.messages, action.message] };
        case 'SET_USERNAME':
            return { ...state, username: action.username };
        case 'SET_INPUT_MESSAGE':
            return { ...state, inputMessage: action.inputmessage };
        default:
            return state;
    }
};