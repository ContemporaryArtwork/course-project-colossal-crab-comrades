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




interface IState {
    loginPageVisible?: boolean;
    statusMSG?: string;
}


class UserAuth extends React.PureComponent<GameMainMenuTogglerProps, IState> {
    
    constructor(props: any) {
        super(props);
        this.state = { loginPageVisible: false, statusMSG: ""}
    }
    


    componentDidMount() {
        this.goToMainMenu = this.goToMainMenu.bind(this);
        this.flipLogInPage = this.flipLogInPage.bind(this);
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

            //FETCH CALL
            const result = fetch(form.action, {
                method: form.method,
                body: new FormData(form) as any,
            }).then((response: Response) => { return response.json(); })
                .catch(error => {
                    console.log(error);
                });
            result.then(output => {

                //----------------------AFTER THE FETCH CALL-------------------->>>
                console.log(output);

                console.log(output.message);
                console.log(sessionStorage.getItem("username"));
                //----------------------AFTER THE FETCH CALL-------------------->>>
                let z = document.cookie;
                let carr = z.split(';');
                let firstElement = carr[0];
                console.log(firstElement.split("=")[1]);
            });
        }
        this.props.toggleLoggedIn();
        
    }

    submitSignUp = (e: React.FormEvent<HTMLElement>): void => {

        e.preventDefault();

        const tempElement: HTMLElement | null = document.getElementById("signUpForm");
        if (tempElement == undefined) {
            //It's undefined!
        } else {

            const form = tempElement as HTMLFormElement;

            
            //FETCH CALL
            const result = fetch(form.action, {
                method: form.method,
                body: new FormData(form) as any,
            }).then((response: Response) => {  return response.json(); } )
                .catch(error => {
                    console.log(error);
                });
                result.then(output => {

//----------------------AFTER THE FETCH CALL-------------------->>>
                console.log(output);

                    if (output.message == "Username already exists") {
                        console.log("nope nope nope");
                    }

                console.log(output.message);
//----------------------AFTER THE FETCH CALL-------------------->>>
                
                });


        }
        this.props.toggleLoggedIn();
    }


    render() {

        return (<div className="enclosing">  
            {this.state.loginPageVisible ? this.renderLogIn() : this.renderSignUp()}
        </div>);
    }


    renderSignUp() {
        return (
            <div className="logSign">
                <ul className="switchUp">
                    <li value="Log In" onClick={this.flipLogInPage}> Log In</li>
                    <li value="Register" onClick={this.flipLogInPage}> Register</li>
                </ul>   
                
                <div className="container">
                    <div className="signupBox" />
                    <form id="signUpForm" action="api/signup" method="post" onSubmit={this.submitSignUp}>

                        <input type="text" id="Username" name="Username" placeholder="Username" />
                        <input type="password" id="Password" name="Password" placeholder="Password" />

                        <button type="submit" name="" value="Create Account" >Create Account</button>

                    </form>
                </div>
            </div>
        );
    }

    renderLogIn() {
        return (
            <div className="logSign">
                <ul className="switchUp">
                    <li value="Log In" onClick={this.flipLogInPage}> Log In</li>
                    <li value="Register" onClick={this.flipLogInPage}> Register</li>
                </ul>  

                <div className="container">
                    <div className="loginBox" />
                    <form id="logInForm" action="api/login" method="post" onSubmit={this.submitLogIn}>

                        <input type="text" id="Username" name="Username" placeholder="Username" />
                        <input type="password" id="Password" name="Password" placeholder="Password" />

                        <button type="submit" name="" value="Log In" >Log In</button>

                    </form>
                </div>
            </div>
        );
    }

    flipLogInPage = (e: React.MouseEvent<HTMLElement>): void => {       
        this.setState({ loginPageVisible: !this.state.loginPageVisible });
    }
}


export default connect(
    (state: ApplicationState) => state.gameMainMenuToggler,
    GameMainMenuTogglerStore.actionCreators
)(UserAuth as any);