﻿import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as Phaser from 'phaser';
import * as GameMainMenuTogglerStore from "../store/GameMainMenuToggler";
import MainMenu from './MainMenu';
import GameStartRenderer from './gameComponents/GameStartRenderer';

type GameMainMenuTogglerProps =
    GameMainMenuTogglerStore.GameMainMenuTogglerState &
    typeof GameMainMenuTogglerStore.actionCreators &
    RouteComponentProps<{}>;


class GameMainMenuToggler extends React.PureComponent<GameMainMenuTogglerProps> {

    componentDidMount() {
        //Test "logging in"
        this.props.toggleLoggedIn();
        
    }

    render() {

        

        if (this.props.loggedIn) {
            return (this.props.inGame ? <GameStartRenderer /> : <MainMenu />);
        } else {
            return (<div>YES LETS LOG IN </div>);
        }
    }
};


export default connect(
    (state: ApplicationState) => state.gameMainMenuToggler,
    GameMainMenuTogglerStore.actionCreators
)(GameMainMenuToggler as any);