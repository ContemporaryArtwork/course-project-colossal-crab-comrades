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


interface IProps {

}

interface IState {
    loginPageVisible?: boolean;
}


class UserAuth extends React.PureComponent<GameMainMenuTogglerProps, IState> {
    
    constructor(props: any) {
        super(props);
        this.state = { loginPageVisible: true }
    }
    


    componentDidMount() {
        this.goToMainMenu = this.goToMainMenu.bind(this);
    }

    goToMainMenu = (e: React.MouseEvent<HTMLElement>): void => {       
        e.preventDefault();
        console.log("hi");
        this.props.toggleLoggedIn();
    }

    toggleLogInPage = () => {
        this.props.toggleLoginPage();
    }

    submitLogIn = (e: React.FormEvent<HTMLElement>): void => {

        e.preventDefault();
        
        const tempElement: HTMLElement | null = document.getElementById("logInForm");
        if (tempElement == undefined) {
            //It's undefined!
        } else {
            
            const form = tempElement as HTMLFormElement;

            console.log(form);

            const result = fetch(form.action, {
                method: form.method,                
                body: new FormData(form) as any,
            })
                .then((response: Response) => { console.log("Logging in..."); response.json()})
                .then(json => json)
                .catch(error => console.log(error));
        }
    }

    submitSignUp = (e: React.FormEvent<HTMLElement>): void => {

        e.preventDefault();

        const tempElement: HTMLElement | null = document.getElementById("signUpForm");
        if (tempElement == undefined) {
            //It's undefined!
        } else {

            const form = tempElement as HTMLFormElement;

            console.log(form);

            const result = fetch(form.action, {
                method: form.method,
                body: new FormData(form) as any,
            })
                .then((response: Response) => { console.log("Signing Up..."); response.json() })
                .then(json => json)
                .catch(error => console.log(error));
        }
    }


    render() {
        if (this.state.loginPageVisible) {
            return (this.renderLogIn());
        } else {
            return (this.renderSignUp());
        }
    }


    renderSignUp() {
        return (
            <div>
                <div className="signupBox" />
                <h1>Create an Account</h1>
                <form id="signUpForm" action="api/login" method="post" onSubmit={this.submitSignUp}>

                    <input type="text" id="Username" name="Username" value="testAccount" />
                    <input type="text" id="Password" name="Password" value="12345" />

                    <button type="submit" name="" value="Create Account" >Submit</button>

                </form>
            </div>
        );
    }

    renderLogIn() {
        return (
            <div>
                <div className="loginBox" />
                <h1>Login</h1>
                <form id="logInForm" action="api/login" method="post" onSubmit={this.submitLogIn}>

                    <input type="text" id="Username" name="Username" value="testAccount" />
                    <input type="text" id="Password" name="Password" value="12345" />

                    <button type="submit" name="" value="Log In" >Submit</button>

                </form>
            </div>
        );
    }
}


export default connect(
    (state: ApplicationState) => state.gameMainMenuToggler,
    GameMainMenuTogglerStore.actionCreators
)(UserAuth as any);