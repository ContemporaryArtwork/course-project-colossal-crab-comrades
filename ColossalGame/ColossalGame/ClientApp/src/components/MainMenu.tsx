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
        
    render() {
        return (
            <body>
                <section className="enclosing">

                    <button className="settingsButton" onClick={this.ToggleSettings}>
                        <img src={SettingsButton} />
                    </button>

                    <button className="loginOpen" onClick={this.SignIn}>
                        <img src={LoginButton} />
                    </button>

                    <div className="loginPanel">
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
                        <button className="loginButton"> LOGIN </button>
                    </div>

                    <div className="settings"> <img src={SettingsPanel} />  </div>

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