import * as React from 'react';
import "./MainMenu.css";

//Redux Imports
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as GameMainMenuTogglerStore from "../store/GameMainMenuToggler";

//Graphics Imports
import MainBackground from "../assets/mainMenu/mainBackground.jpg";
import BGVideo from "../assets/mainMenu/BGVideo.mp4";
import SettingsButton from "../assets/mainMenu/Settings Button.png";
import SettingsPanel from "../assets/mainMenu/SettingsPanel.png";
import LoginButton from "../assets/mainMenu/GoogleIcon.png";

//Cookie Import
import  getCookie from "../Helpers/GetCookies"

type GameMainMenuTogglerProps =
    GameMainMenuTogglerStore.GameMainMenuTogglerState &
    typeof GameMainMenuTogglerStore.actionCreators &
    RouteComponentProps<{}>;



class MainMenu extends React.PureComponent<GameMainMenuTogglerProps> {

    ToggleSettings() {
        var settingsPanel = document.getElementsByClassName("settings") as HTMLCollectionOf<HTMLElement>;
        if (settingsPanel[0].style.display === "none") {
            settingsPanel[0].style.display = "block";
        } else {
            settingsPanel[0].style.display = "none";
        }
    }

    SignIn() {
        var log = document.getElementsByClassName("loginPanel") as HTMLCollectionOf<HTMLElement>;
        if (log[0].style.display === "none") {
            log[0].style.display = "block";
        } else {
            log[0].style.display = "none";
        }
    }

    SignUp() {
        var signUp = document.getElementsByClassName("registerPanel") as HTMLCollectionOf<HTMLElement>;
        var log = document.getElementsByClassName("loginPanel") as HTMLCollectionOf<HTMLElement>;
        if (signUp[0].style.display === "none") {
            signUp[0].style.display = "block";
            log[0].style.display = "none";
        } else {
            signUp[0].style.display = "none";
        }
    }

    SendData() {
        var signUp = document.getElementsByClassName("registerPanel") as HTMLCollectionOf<HTMLElement>;
        var log = document.getElementsByClassName("loginPanel") as HTMLCollectionOf<HTMLElement>;
        if (signUp[0].style.display === "none") {
            signUp[0].style.display = "none";
            log[0].style.display = "none";
        } else {
            signUp[0].style.display = "none";
        }
    }

    OpenLoadout() {
        var load = document.getElementsByClassName("loadout") as HTMLCollectionOf<HTMLElement>;
        if (load[0].style.display === "none") {
            load[0].style.display = "block";
        } else {
            load[0].style.display = "none";
        }
    }

    ChooseHeavy() {
        var heav = document.getElementsByClassName("classColor") as HTMLCollectionOf<HTMLElement>;
        var load = document.getElementsByClassName("loadout") as HTMLCollectionOf<HTMLElement>;
        heav[0].style.backgroundColor = "purple";
        heav[0].textContent = "HEAVY";
        load[0].style.display = "none"
    }

    ChooseBrawler() {
        var braw = document.getElementsByClassName("classColor") as HTMLCollectionOf<HTMLElement>;
        var load = document.getElementsByClassName("loadout") as HTMLCollectionOf<HTMLElement>;
        braw[0].style.backgroundColor = "red";
        braw[0].textContent = "BRAWLER";
        load[0].style.display = "none"
    }

    ChooseBuilder() {
        var bui = document.getElementsByClassName("classColor") as HTMLCollectionOf<HTMLElement>;
        var load = document.getElementsByClassName("loadout") as HTMLCollectionOf<HTMLElement>;
        bui[0].style.backgroundColor = "yellowgreen";
        bui[0].textContent = "BUILDER";
        load[0].style.display = "none"
    }

    render() {

        if (document.cookie == null) {
            return <div>hi</div>
        } else {
            return (
                <body>
                    <div className="enclosing">

                        <div className="video-container">
                            <video src={BGVideo} loop autoPlay muted />
                        </div>

                        <div className="containMain">
                            <button className="settingsButton" onClick={this.ToggleSettings}>
                                <img src={SettingsButton} />
                            </button>

                            <button className="loginOpen" onClick={this.SignIn}> Login </button>
                        </div>

                        <div className="containMain">
                            <div className="content">
                                <h1>Pre Alpha CSE 442 Project</h1>
                                <h3>Founders (A-Z): Eoghan Mccarroll, Jacob Santoni, Joshua Lacey, Kyle Pellechia, Zachary Wagner </h3>
                            </div>
                            <div>
                                <div className="alignButtons">
                                    <div className="classColor"></div>
                                    <button className="classSelectButton" onClick={this.OpenLoadout} > Select Class </button>
                                </div>
                                <div className="alignButtons">
                                    <button className="startGameButton" onClick={() => { this.props.toggleGame(); }}> Start Game </button>
                                </div>
                            </div>
                        </div>



                        <div className="loadout">
                            <div className="signContainer">
                                <div className="classContainer">
                                    <div className="Heavy"></div>
                                    <p> Class Summary: <br />
                                    Pros:<br />
                                    - More Health <br />
                                    Cons:<br />
                                    - shorter firing range<br />
                                    Abilities: <br />
                                    - Bulk Up: gains a shield <br />for 5 seconds
                                </p>
                                    <button className="loadButton" onClick={this.ChooseHeavy}>HEAVY</button>
                                </div>

                                <div className="classContainer">
                                    <div className="Brawler"></div>
                                    <p> Class Summary: <br />
                                    Pros:<br />
                                    - faster rate of fire <br />
                                    Cons:<br />
                                    - No special attributes<br />
                                    Abilities: <br />
                                    - Frenzy: gains rate of fire <br /> increase for 5 seconds
                                </p>
                                    <button className="loadButton" onClick={this.ChooseBrawler}>BRAWLER</button>
                                </div>

                                <div className="classContainer">
                                    <div className="Builder"></div>
                                    <p> Class Summary: <br />
                                    Pros:<br />
                                    - build structures to protect you<br />
                                    Cons:<br />
                                    - Not as much firepower <br />
                                    Abilities: <br />
                                    - Hammer time: No cooldown on <br /> building for 5 seconds
                                </p>
                                    <button className="loadButton" onClick={this.ChooseBuilder}>BUILDER</button>
                                </div>
                            </div>
                        </div>

                        <div className="settings"> <img src={SettingsPanel} />  </div>

                        <div className="loginPanel">
                            <div className="signContainer">
                                <label>Email</label><br></br>
                                <input
                                    type="text"
                                    placeholder="Email"
                                ></input><br></br>
                                <label>Password</label><br></br>
                                <input
                                    type="text"
                                    placeholder="Password"
                                ></input><br></br>
                                <button className="loginButton" onClick={this.SendData}> LOGIN </button>
                                <button className="registerButton" onClick={this.SignUp}>Don't have an account? Click here to register</button>
                            </div>
                        </div>

                        <div className="registerPanel">
                            <div className="signContainer">
                                <label>Email</label><br></br>
                                <input
                                    type="text"
                                    placeholder="Email"
                                ></input><br></br>
                                <label>Password</label><br></br>
                                <input
                                    type="text"
                                    placeholder="Password"
                                ></input><br></br>
                                <label>Confirm Password</label><br></br>
                                <input
                                    type="text"
                                    placeholder="Repeat Password"
                                ></input><br></br>
                                <button className="loginButton" onClick={this.SendData}> REGISTER </button>
                            </div>
                        </div>
                    </div>
                </body>
            );
        }
        



    }
}


export default connect(
    (state: ApplicationState) => state.gameMainMenuToggler,
    GameMainMenuTogglerStore.actionCreators
)(MainMenu as any);