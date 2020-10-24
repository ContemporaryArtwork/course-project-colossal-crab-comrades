import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as GameMainMenuTogglerStore from "../store/GameMainMenuToggler";
import MainMenu from './MainMenu';
import GameStartRenderer from './gameComponents/GameStartRenderer';
import "./UserAuth.css";
import BGVideo from "../assets/mainMenu/BGVideo.mp4";

type GameMainMenuTogglerProps =
    GameMainMenuTogglerStore.GameMainMenuTogglerState &
    typeof GameMainMenuTogglerStore.actionCreators &
    RouteComponentProps<{}>;


class UserAuth extends React.PureComponent<GameMainMenuTogglerProps> {

    render() {
        return (
            <body>
                <div className="enclosing">

                    <div className="video-container">
                        <video src={BGVideo} loop autoPlay muted />
                    </div>

                    <div className="containMain">
                        <div className="content">
                            <h1>Pre Alpha CSE 442 Project</h1>
                            <h3>Founders (A-Z): Eoghan Mccarroll, Jacob Santoni, Joshua Lacey, Kyle Pellechia, Zachary Wagner </h3>
                        </div>
                    </div>
                </div>
            </body>
        );
    }
}


export default connect(
    (state: ApplicationState) => state.gameMainMenuToggler,
    GameMainMenuTogglerStore.actionCreators
)(UserAuth as any);