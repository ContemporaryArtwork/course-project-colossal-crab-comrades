import { Action, Reducer } from 'redux';
import { AppThunkAction } from './';
import { HubConnection } from '@microsoft/signalr';
import { setupSignalRConnection } from "../SignalR/SingalR"
import getCookie from "../Helpers/GetCookies"

// -----------------
// STATE - This defines the type of data maintained in the Redux store.


export interface GameObjectModel {

    yPos: number,
    xPos: number
}
export interface PlayerModel {
    Username: string,
    yPos: number,
    xPos: number,
}

export interface GameLogicMessage {
    objectList: GameObjectModel[],
    playerDict: Map<string, PlayerModel>
}



export enum Direction {
    Down = 0,
    Up,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight
}

export interface MovementAction {
    Username: string,
    Token: string,
    Direction: Direction
}
export interface SpawnAction {
    Username: string,
    Token: string,

}

export interface Coordinates {
    x: number,
    y: number
}

export interface Position {
    coords: Coordinates,
    direction: Direction
}

export interface PlayerData {
    username: string,
    position: Position,
    classType: boolean
}

export interface GameDataState {
    isLoading: boolean,
    token?: string,
    connection?: HubConnection,
    playerData: PlayerData,
    currentGameState: GameLogicMessage
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
// Use @typeName and isActionType for type detection that works even after serialization/deserialization.

export interface InitializeAction {
    type: 'INITIALIZED';
    connection: HubConnection;
}
export interface SendMovementAction {
    type: 'SEND_MOVEMENT';
    direction: Direction;
}
export interface SpawnPlayerRequestAction {
    type: 'SPAWN_PLAYER_REQUEST';
}
export interface SetUsernameAction {
    type: 'SET_USERNAME';
    username: string;
}
export interface TempLoginAction {
    type: 'TEMP_LOGIN_SENT';
    username: string,
    password: string
}
export interface ReceivedTokenAction {
    type: 'RECEIVE_TOKEN';
    token: string
}
export interface ReceivePositionsUpdateAction {
    type: 'RECEIVE_POSITIONS_UPDATE';
    objectList: GameObjectModel[],
    playerDict: Map<string, PlayerModel>
}
export interface ToggleClassAction {
    type: 'TOGGLE_CLASS';
}


// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
export type KnownAction = ToggleClassAction | InitializeAction | SendMovementAction | SpawnPlayerRequestAction | SetUsernameAction | TempLoginAction | ReceivedTokenAction | ReceivePositionsUpdateAction;

const SignalRActionsList: string[] = ["RECEIVED_STRING", "RECEIVE_TOKEN", "RECEIVE_POSITIONS_UPDATE"];
// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {

    toggleClass: () => ({ type: 'TOGGLE_CLASS'} as ToggleClassAction),

    setUsername: (name: string) => ({ type: 'SET_USERNAME', username: name } as SetUsernameAction),

    initialize: (): AppThunkAction<KnownAction> => async (dispatch, getState) => { //Made method Async so game would wait for the signalr connection to be established.
        // Only load data if it's something we don't already have (and are not already loading)
        const appState = getState();
        console.log(appState);
        if (appState && appState.gameData) {                               // might be caps
            const setupEventsHub: HubConnection = await (await setupSignalRConnection('/hubs/gamedata', SignalRActionsList))(dispatch);
            dispatch({ type: 'INITIALIZED', connection: setupEventsHub });
        }
    },

    tempLogin: (username: string, password: string): AppThunkAction<KnownAction> => (dispatch, getState) => { //get rid of this once login functionality is verified to work through frontend.
        const appState = getState();
        if (appState && appState.gameData && appState.gameData.connection) {

            appState.gameData.connection && appState.gameData.connection.invoke("TempLogin", username,password)

            dispatch({ type: 'TEMP_LOGIN_SENT', username: username, password: password });
        }
    },

    sendMovementAction: (direction: Direction): AppThunkAction<KnownAction> => (dispatch, getState) => {
        const appState = getState();
        let token = getCookie("auth-token");
        if (appState && appState.gameData && appState.gameData.connection && appState.gameData.playerData.username && token) {

            const movementActionDTO: MovementAction = {
                Username: appState.gameData.playerData.username,
                Token: token,
                Direction: direction
            }
            appState.gameData.connection && appState.gameData.connection.invoke("SendMovement", movementActionDTO)

            dispatch({ type: 'SEND_MOVEMENT', direction: direction });
        }
    },
    spawnPlayerAction: (): AppThunkAction<KnownAction> => (dispatch, getState) => { //Used to request to spawn the player. This method attempts to spawn the player using the username and auth-token cookies.
        const appState = getState();
        let token = getCookie("auth-token");
        if (appState && appState.gameData && appState.gameData.connection && appState.gameData.playerData.username && token) {

            const spawnActionDTO: SpawnAction = {
                Username: appState.gameData.playerData.username,
                Token: token,

            }
            appState.gameData.connection && appState.gameData.connection.invoke("SpawnPlayer", spawnActionDTO)

            dispatch({ type: 'SPAWN_PLAYER_REQUEST' });
        }
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: GameDataState = {
    isLoading: false,playerData: {
        username: "", position: {
            coords: { x: 0, y: 0 }, direction: Direction.Right,
        }, classType: false
    },
    currentGameState: {
        objectList: [],
        playerDict: new Map<string,PlayerModel>(),
    } 
};

export const reducer: Reducer<GameDataState> = (state: GameDataState | undefined, incomingAction: Action): GameDataState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'INITIALIZED':
            return {
                isLoading: false,
                connection: action.connection,
                playerData: {
                    username: "", position: { coords: { x: 0, y: 0 }, direction: Direction.Right }, classType: false
                }, 
                currentGameState: {
                    objectList: [],
                    playerDict: new Map<string, PlayerModel>(),
                } 
            };
        case 'TEMP_LOGIN_SENT':
            return {
                ...state, playerData: { ...state.playerData, username: action.username }
            };
        case 'RECEIVE_TOKEN':
            return {
                ...state, token: action.token
            };
        case 'RECEIVE_POSITIONS_UPDATE':
            return {
                ...state, currentGameState: { playerDict: makeMapIfNot(action.playerDict), objectList: action.objectList }
            };
        case 'SEND_MOVEMENT':
            return state;
        case 'SPAWN_PLAYER_REQUEST':
            return state;
        case 'SET_USERNAME':
            return {
                ...state, playerData: { ...state.playerData, username: action.username }
            };
        case 'TOGGLE_CLASS':
            return {

                ...state, playerData: { ...state.playerData, classType: !state.playerData.classType}
            };
        default:
            return state;
    }
};

function makeMapIfNot(input: Map<string, PlayerModel> | object): Map<string, PlayerModel> {
    if (typeof (input) == "object") {
        return new Map<string, PlayerModel>(Object.entries(input));
    }
    return input as Map<string, PlayerModel>;
}