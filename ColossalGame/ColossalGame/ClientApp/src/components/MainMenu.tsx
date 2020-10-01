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
        load[0].style.display = "none"
    }

    ChooseBrawler() {
        var braw = document.getElementsByClassName("classColor") as HTMLCollectionOf<HTMLElement>;
        var load = document.getElementsByClassName("loadout") as HTMLCollectionOf<HTMLElement>;
        braw[0].style.backgroundColor = "red";
        load[0].style.display = "none"
    }

    ChooseBuilder() {
        var bui = document.getElementsByClassName("classColor") as HTMLCollectionOf<HTMLElement>;
        var load = document.getElementsByClassName("loadout") as HTMLCollectionOf<HTMLElement>;
        bui[0].style.backgroundColor = "yellowgreen";
        load[0].style.display = "none"
    }
    
        
    render() {
        return (
            <body>
                <section className="enclosing">

                    <button className="settingsButton" onClick={this.ToggleSettings}>
                        <img src={SettingsButton} />
                    </button>

                    <div className="settings"> <img src={SettingsPanel} />  </div>

                    <button className="loginOpen" onClick={this.SignIn}> Login </button>

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

                    <div className="video-container">                        
                        <video src={BGVideo} loop autoPlay muted />
                    </div>

                    <div className="content">
                        <h3>Founders (A-Z): Eoghan Mccarroll, Jacob Santoni, Joshua Lacey, Kyle Pellechia, Zachary Wagner </h3>
                        <button className="classSelectButton" onClick={this.OpenLoadout} > Select Class </button>
                        <div className="classColor"></div>
                        <button className="classSelect" onClick={() => { this.props.toggleGame(); }}> Start Game </button>
                    </div>

                    <div className="loadout">
                        <div className="signContainer">
                            <div className="classContainer">
                                <div className="Heavy"></div>
                                <button className="loadButton" onClick={this.ChooseHeavy}>HEAVY</button>
                            </div>

                            <div className="classContainer">
                                <div className="Brawler"></div>
                                <button className="loadButton" onClick={this.ChooseBrawler}>BRAWLER</button>
                            </div>

                            <div className="classContainer">
                                <div className="Builder"></div>
                                <button className="loadButton" onClick={this.ChooseBuilder}>BUILDER</button>
                            </div>
                        </div>
                    </div>

                    

                </section>
            </body>
        );
    }
}


export default connect(
    (state: ApplicationState) => state.gameMainMenuToggler,
    GameMainMenuTogglerStore.actionCreators
)(MainMenu as any);