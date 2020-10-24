import * as WeatherForecasts from './WeatherForecasts';
import * as Counter from './Counter';
import * as GlobalChat from './GlobalChat';
import * as GameMainMenuToggler from './GameMainMenuToggler';
import * as GameData from './GameData';

// The top-level state object
export interface ApplicationState {
    counter: Counter.CounterState | undefined;
    weatherForecasts: WeatherForecasts.WeatherForecastsState | undefined;
    globalchat: GlobalChat.GlobalChatState | undefined;
    gameMainMenuToggler: GameMainMenuToggler.GameMainMenuTogglerState | undefined;
    gameData: GameData.GameDataState | undefined;
}

// Whenever an action is dispatched, Redux will update each top-level application state property using
// the reducer with the matching name. It's important that the names match exactly, and that the reducer
// acts on the corresponding ApplicationState property type.
export const reducers = {
    counter: Counter.reducer,
    weatherForecasts: WeatherForecasts.reducer,
    globalchat: GlobalChat.reducer,
    gameMainMenuToggler: GameMainMenuToggler.reducer,
    gameData: GameData.reducer
};

// This type can be used as a hint on action creators so that its 'dispatch' and 'getState' params are
// correctly typed to match your store.
export interface AppThunkAction<TAction> {
    (dispatch: (action: TAction) => void, getState: () => ApplicationState): void;
}