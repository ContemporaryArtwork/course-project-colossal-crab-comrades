import * as React from 'react';
import { connect } from 'react-redux';
import { Button } from 'reactstrap';
import { title } from 'process';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as GameMainMenuTogglerStore from "../store/GameMainMenuToggler";
import MainMenu from './MainMenu';
import GameStartRenderer from './gameComponents/GameStartRenderer2';
import "./UserAuth.css";

import BGVideo from "../assets/mainMenu/BGVideo.mp4";
//import BG from "../assets/mainMenu/mainBackground.jpg";
//import test from "../assets/mainMenu/test.png";
import BG from "../assets/test/undraw_personalization_triu.png";
import test from "../assets/test/undraw_profile_pic_ic5t.png";

import { Link } from "react-router-dom";
import { NavLink } from "react-router-dom";



type GameMainMenuTogglerProps =
    GameMainMenuTogglerStore.GameMainMenuTogglerState &
    typeof GameMainMenuTogglerStore.actionCreators &
    RouteComponentProps<{}>;




interface IState {
    loginPageVisible?: boolean;
    statusMSG?: string;
    errorText?: string;
}


class UserAuth extends React.PureComponent<GameMainMenuTogglerProps, IState> {
    
    constructor(props: any) {
        super(props);
        this.state = { loginPageVisible: false, statusMSG: "", errorText: "" }
    }
    


    componentDidMount() {
        this.flipLogInPage = this.flipLogInPage.bind(this);
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
                    console.log(output.message);
                    console.log(output.errorCode);
                    console.log(output.status);

                    if (this.verifySubmit(output.message, output.status, output.errorCode)) {
                        //switch route to log in page.
                    }               
//----------------------AFTER THE FETCH CALL-------------------->>>               
                });
        }
        
    }

    verifySubmit(message: string, status: string, code: string): boolean {

        if (status == "ok") {        
            return true;
        } else {
            this.setState({ errorText: message });
            return false;
        }

    }



    render() {

        if (document.cookie == "") {
            return <div>hi</div>
        } else {
            return (<div>
                {this.state.loginPageVisible ? this.renderLogIn() : this.renderSignUp()}
                <p style={{ color: '#FF0008' }}>{this.state.errorText}</p>
            </div>);
        }


    }


    renderSignUp() {
        return (
            <div>
                <input type="button" value="Switch to Log In" onClick={this.flipLogInPage}/>
                <div className="signupBox" />
                <h1>Create an Account</h1>
                <form id="signUpForm" action="api/signup" method="post" onSubmit={this.submitSignUp}>

                    <input type="text" id="Username" name="Username"  />
                    <input type="password" id="Password" name="Password"  />

                    <button type="submit" name="" value="Create Account" >Submit</button>

                </form>
            </div>
        );
    }

    renderLogIn() {
        return (
            <div>
                <Button component={Link} to="/login">Switch to Create an Account</Button>
                <input type="button" value="Switch to Create an Account" onClick={this.flipLogInPage} />
                <div className="loginBox" />
                <h1>Login</h1>
                <form id="logInForm" action="api/login" method="post" onSubmit={this.submitLogIn}>

                    <input type="text" id="Username" name="Username"  />
                    <input type="password" id="Password" name="Password"  />

                    <button type="submit" name="" value="Log In" >Submit</button>

                </form>
            </div>
        );
    }

    flipLogInPage = (e: React.MouseEvent<HTMLElement>): void => {
        console.log("yee");
        this.setState({ loginPageVisible: !this.state.loginPageVisible });       
    }
}


export default connect(
    (state: ApplicationState) => state.gameMainMenuToggler,
    GameMainMenuTogglerStore.actionCreators
)(UserAuth as any);