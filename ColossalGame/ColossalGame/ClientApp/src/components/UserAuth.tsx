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


    componentDidMount() {
        //Test "logging in"
        //this.props.toggleLoginPage();
        this.goToMainMenu = this.goToMainMenu.bind(this);
    }

    goToMainMenu = (e: React.FormEvent<HTMLFormElement>) => {        
            e.preventDefault();
            alert("FIRST");
            this.props.toggleLoggedIn();
            alert("SECOND");
        }

    render() {
        if (this.props.loginPage) {
            return (
                <body>
                    <div className="enclosing">

                        <div className="video-container">
                            <video src={BGVideo} loop autoPlay muted />
                        </div>

                        <div className="containMain">
                            <div className="content">

                                <div className="loginForm">
                                    <form onSubmit={this.goToMainMenu}>
                                        <label htmlFor="fname">Username:</label><br />
                                        <input type="text" id="fname" name="fname" placeholder="Awesome Username" /><br />
                                        <label htmlFor="lname">Password:</label><br />
                                        <input type="text" id="lname" name="lname" placeholder="Super Secure Password" /><br /><br />
                                        <input type="submit" value="Submit"/>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                </body>
            );
        } else {
            return (<div>hi</div>);
        }
        }
}


export default connect(
    (state: ApplicationState) => state.gameMainMenuToggler,
    GameMainMenuTogglerStore.actionCreators
)(UserAuth as any);