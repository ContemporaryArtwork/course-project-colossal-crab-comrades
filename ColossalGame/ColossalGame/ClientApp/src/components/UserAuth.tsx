import * as React from 'react';
import { connect } from 'react-redux';
import { Button } from 'reactstrap';
import { title } from 'process';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as GameMainMenuTogglerStore from "../store/GameMainMenuToggler";
import MainMenu from './MainMenu';
import GameStartRenderer from './gameComponents/GameStartRenderer';
import "./UserAuth.css";
import BGVideo from "../assets/mainMenu/BGVideo.mp4";
//import BG from "../assets/mainMenu/mainBackground.jpg";
//import test from "../assets/mainMenu/test.png";
import BG from "../assets/test/undraw_personalization_triu.png";
import test from "../assets/test/undraw_profile_pic_ic5t.png";


type GameMainMenuTogglerProps =
    GameMainMenuTogglerStore.GameMainMenuTogglerState &
    typeof GameMainMenuTogglerStore.actionCreators &
    RouteComponentProps<{}>;

/*
interface IProps {
}

interface IState {
    username?: string;
    password?: string;
}
*/

//class UserAuth extends React.PureComponent<GameMainMenuTogglerProps, IProps, IState> {
class UserAuth extends React.PureComponent<GameMainMenuTogglerProps> {
    /*
    constructor(props: IProps) {
        super(props);
        this.state = { username: "", password: ""}

    }
    */


    componentDidMount() {
        //Test "logging in"
        //this.props.toggleLoginPage();
        this.goToMainMenu = this.goToMainMenu.bind(this);
    }

    goToMainMenu = (e: React.MouseEvent<HTMLElement>): void => {       
            e.preventDefault();
        this.props.toggleLoggedIn();
    }

    toggleLogInPage = () => {
        this.props.toggleLoginPage();
    }

    usernameChangeHandler = (e: React.ChangeEvent<HTMLInputElement>): void => {
        this.setState({ username: e.target.value });       
    }

    passwordChangeHandler = (e: React.ChangeEvent<HTMLInputElement>): void => {
        this.setState({ password: e.target.value });
    }

    submitLogIn() {
        /*
        fetch(
            "https://www.getpostman.com/collections/743c07eb0b37c8b0a2b6",
            {
                method: "post",
                body: JSON.stringify({
                    //username: this.state.username,
                   // password: this.state.password,
                }),
            }
        )
            .then((res) => res.json())
            .then(
                (result) => {
                    console.log(result.message);
                },
                (error) => {
                    // alert("CURSES! FOILED AGAIN!");
                }
            );
            */
    }


    render() {

        return (

            <div>
                <div className="loginBox"/>
                    <h1>Login</h1>
                <div className="usernameTextbox">
                    <input type="text" placeholder="Username" onChange={this.usernameChangeHandler} />
                    </div> 

                    <div className="passwordTextbox">
                    <input type="password" placeholder="Password" onChange={this.passwordChangeHandler} />
                    </div>  

                <input className="btn" type="button" name="" value="Sign in" onClick={this.goToMainMenu} />
            </div>
            );
    }            
}


export default connect(
    (state: ApplicationState) => state.gameMainMenuToggler,
    GameMainMenuTogglerStore.actionCreators
)(UserAuth as any);