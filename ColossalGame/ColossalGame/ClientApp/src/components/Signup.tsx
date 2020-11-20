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
import BGIMG from "../assets/Login_Signup/CityRuins.png";

import BGVideo from "../assets/mainMenu/BGVideo.mp4";
//import BG from "../assets/mainMenu/mainBackground.jpg";
//import test from "../assets/mainMenu/test.png";
import BG from "../assets/test/undraw_personalization_triu.png";
import test from "../assets/test/undraw_profile_pic_ic5t.png";

import { Link } from "react-router-dom";
import { NavLink } from "react-router-dom";

//Router Import
import { Redirect } from "react-router-dom";
import { useHistory } from 'react-router-dom'





type GameMainMenuTogglerProps =
    GameMainMenuTogglerStore.GameMainMenuTogglerState &
    typeof GameMainMenuTogglerStore.actionCreators &
    RouteComponentProps<{}>;




interface IState {
    statusMSG?: string;
    errorText?: string;
}


class Signup extends React.PureComponent<GameMainMenuTogglerProps, IState> {

    constructor(props: any) {
        super(props);
        this.state = {statusMSG: "", errorText: "" }
    }
    toggleLogInPage = () => {
        this.props.toggleLoginPage();
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
            }).then((response: Response) => { return response.json(); })
                .catch(error => {
                    console.log(error);
                });
            result.then(output => {
                if (this.verifySubmit(output.message, output.status, output.errorCode)) {
                    //Go to login
                    this.props.history.push(`/login`);
                }             
            });
        }

    }

    verifySubmit(message: string, status: string, code: string): boolean {

        if (status == "ok") {
            return true;
        } else {


            if (code == "UserAlreadyExists") {
                this.setState({ errorText: "This user already exists. Try submitting a unique username."});
            }
            if (code == "BadPassword") {
                this.setState({ errorText: "Your password is not strong enough. Your password must contain: 8 characters, 1 uppercase, 1 lowercase, 1 special character." });
            }
            if (code == "BadUsername") {
                this.setState({ errorText: "Try using a different username."});
            }
            if (code == "Unknown") {
                this.setState({ errorText: "It appears that we have encountered an unknown problem. Please try again. (Ah, the mysterious unknown!)" });
            }



            //this.setState({ errorText: message });
            return false;
        }

    }



    render() {
        return (<div className="logSignEnclosing">
            <div className="bg-container">
                <img src={BGIMG} />
            </div>
            {this.renderSignUp()}
        </div>);
    }


    renderSignUp() {
        return (
            <div className="logSign">
                <ul className="switchUp">
                    <li value="Log In" onClick={this.doRedirect}> Log In</li>
                    <li value="Register" style={{ background: '#008000', color: '#FFFFFF'}}> Register</li>
                </ul>

                <div className="formContainer">
                    <form id="signUpForm" action="api/signup" method="post" onSubmit={this.submitSignUp}>

                        <input type="text" id="Username" name="Username" placeholder="Username" />
                        <input type="password" id="Password" name="Password" placeholder="Password" />

                        <button className="logSignBut" type="submit" name="" value="Create Account" >Create Account</button>

                    </form>

                    <p style={{ color: '#FF0008' }}>{this.state.errorText}</p>

                </div>
            </div>
        );
    }

    doRedirect = () => {       
        this.props.history.push(`/login`);
    }
}


export default connect(
    (state: ApplicationState) => state.gameMainMenuToggler,
    GameMainMenuTogglerStore.actionCreators
)(Signup as any);