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
            this.setState({ errorText: message });
            return false;
        }

    }



    render() {
        return (<div>
            {this.renderSignUp()}
            <p style={{ color: '#FF0008' }}>{this.state.errorText}</p>
        </div>);
    }


    renderSignUp() {
        return (
            <div>
                <input type="button" value="Switch to Log In" onClick={this.doRedirect} />
                <div className="signupBox" />
                <h1 className = "userAuthH1">Create an Account</h1>
                <form id="signUpForm" action="api/signup" method="post" onSubmit={this.submitSignUp}>

                    <input type="text" id="Username" name="Username" />
                    <input type="password" id="Password" name="Password" />

                    <button type="submit" name="" value="Create Account" >Submit</button>

                </form>
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