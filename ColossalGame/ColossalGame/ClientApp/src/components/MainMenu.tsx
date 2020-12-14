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
import getCookie from "../Helpers/GetCookies"


//Router Import
import { Redirect } from "react-router-dom";

//HTTP? Import
import { request } from 'http';


type GameMainMenuTogglerProps =
    GameMainMenuTogglerStore.GameMainMenuTogglerState &
    typeof GameMainMenuTogglerStore.actionCreators &
    RouteComponentProps<{}>;

interface MenuState {
    showSettings?: boolean;
    showLoadout?: boolean;
}


class MainMenu extends React.PureComponent<GameMainMenuTogglerProps, MenuState> {
    constructor(props: any) {
        super(props);
        this.state = {
            showSettings: false,
            showLoadout: false
        }
        this.ToggleSettings = this.ToggleSettings.bind(this);
        this.OpenLoadout = this.OpenLoadout.bind(this);
        this.ChooseHeavy = this.ChooseHeavy.bind(this);
        this.ChooseBrawler = this.ChooseBrawler.bind(this);
        //this.ChooseBuilder = this.ChooseBuilder.bind(this);
        this.closeOpenMenu = this.closeOpenMenu.bind(this);
    }


    ToggleSettings() {
        this.setState({ showSettings: !this.state.showSettings });
    }

    OpenLoadout() {
        this.setState({ showLoadout: !this.state.showLoadout });
    }

    closeOpenMenu() {
        this.setState({ showLoadout: false });
        this.setState({ showSettings: false });
    }

    ChooseHeavy() {
        var heav = document.getElementsByClassName("classColor") as HTMLCollectionOf<HTMLElement>;
        heav[0].style.backgroundColor = "purple";
        heav[0].textContent = "HEAVY";
        this.setState({ showLoadout: !this.state.showLoadout });
        this.props.sendPlayerClass("heavy")
    }

    ChooseBrawler() {
        var braw = document.getElementsByClassName("classColor") as HTMLCollectionOf<HTMLElement>;
        braw[0].style.backgroundColor = "red";
        braw[0].textContent = "BRAWLER";
        this.setState({ showLoadout: !this.state.showLoadout });
        this.props.sendPlayerClass("brawler")
    }

    //ChooseBuilder() {
    //    var bui = document.getElementsByClassName("classColor") as HTMLCollectionOf<HTMLElement>;
    //    bui[0].style.backgroundColor = "yellowgreen";
    //    bui[0].textContent = "BUILDER";
    //    this.setState({ showLoadout: !this.state.showLoadout });
    //}

    render() {
        const req = request(
            {
                path: '/api/loggedIn',
                method: 'GET',
            },
            response => {
                console.log(response.statusCode); // 200
                
            }
        );
        
        if (document.cookie == "") {
            return <Redirect to="/signup" />
        }

        else {
            return (
                <body>
                    <div className="enclosing">

                        <div className="video-container">
                            <video src={BGVideo} loop autoPlay muted />
                        </div>

                        <div className="containMain">
                            <img src={SettingsButton} className="settingsButton" onClick={this.ToggleSettings}/>
                        </div>

                        <div className="containMain">
                            <div className="content">
                                <h1 className="projectTitle">Pre Alpha CSE 442 Project</h1>
                                <h3>Founders (A-Z): Eoghan Mccarroll, Jacob Santoni, Joshua Lacey, Kyle Pellechia, Zachary Wagner </h3>
                            </div>
                            <div>
                                <div className="alignButtons">
                                    <div className="classColor">BRAWLER</div>
                                    <button className="classSelectButton" onClick={this.OpenLoadout} > Select Class </button>
                                </div>
                                <div className="alignButtons">
                                    <button className="startGameButton" onClick={() => { this.props.toggleGame(); }}> Start Game </button>
                                </div>
                            </div>
                        </div>

                        {this.state.showSettings &&
                            <div className="settings"> <img src={SettingsPanel} />
                                <span className="closeButton" onClick={this.closeOpenMenu}></span>
                            </div>
                        }

                        {this.state.showLoadout &&
                        <div className="loadout">
                            <span className="closeButton" onClick={this.closeOpenMenu}></span>
                            <div className="signContainer">
                                <div className="classContainer">
                                    <div className="Heavy"></div>
                                    <p> Class Summary: <br />
                                        Pros:<br />
                                        - More Health <br />
                                        - Faster rate of fire<br />
                                        Cons:<br />
                                        - moves slower<br />
                                        - deals less damage<br />
                                    </p>
                                    <button className="loadButton" onClick={this.ChooseHeavy}>HEAVY</button>
                                </div>

                                <div className="classContainer">
                                    <div className="Brawler"></div>
                                    <p> Class Summary: <br />
                                            Pros:<br />
                                            - Deals more damage <br />
                                            - moves faster<br /> 
                                            Cons:<br />
                                            - slower rate of fire<br />
                                    </p>
                                    <button className="loadButton" onClick={this.ChooseBrawler}>BRAWLER</button>
                                </div>

                                {/*
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
                                */}
                            </div>
                        </div>
                        }

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