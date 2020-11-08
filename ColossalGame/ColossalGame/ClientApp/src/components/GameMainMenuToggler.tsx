import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as Phaser from 'phaser';
import * as GameMainMenuTogglerStore from "../store/GameMainMenuToggler";
import MainMenu from './MainMenu';
import GameStartRenderer2 from './gameComponents/GameStartRenderer2';
import UserAuth from './UserAuth';

type GameMainMenuTogglerProps =
    GameMainMenuTogglerStore.GameMainMenuTogglerState &
    typeof GameMainMenuTogglerStore.actionCreators &
    RouteComponentProps<{}>;


class GameMainMenuToggler extends React.PureComponent<GameMainMenuTogglerProps> {

    componentDidMount() {
        //Test "logging in"
        //this.props.toggleLoggedIn();
        
    }

    render() {

        

        if (this.props.loggedIn) {
            return (this.props.inGame ? <GameStartRenderer2 /> : <MainMenu />);
        } else {
            return (<UserAuth/>);
        }
    }
};


export default connect(
    (state: ApplicationState) => state.gameMainMenuToggler,
    GameMainMenuTogglerStore.actionCreators
)(GameMainMenuToggler as any);