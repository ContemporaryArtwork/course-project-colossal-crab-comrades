import * as React from 'react';
import "./MainMenu.css";

import MainBackground from "../assets/mainMenu/mainBackground.jpg";
import BGVideo from "../assets/mainMenu/BGVideo.mp4";
import SettingsButton from "../assets/mainMenu/Settings Button.png";
import SettingsPanel from "../assets/mainMenu/SettingsPanel.png";
import LoginButton from "../assets/mainMenu/GoogleIcon.png";

export default class MainMenu extends React.Component {
    state = {

    };

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
                        <h1>Pre Alpha CSE 442 Project</h1>
                        <h3>Founders (A-Z): Eoghan Mccarroll, Jacob Santoni, Joshua Lacey, Kyle Pellechia, Zachary Wagner </h3> 
                        <button className="classSelect"> Select Class </button>
                    </div>

                </section>
            </body>
        );
    }
}