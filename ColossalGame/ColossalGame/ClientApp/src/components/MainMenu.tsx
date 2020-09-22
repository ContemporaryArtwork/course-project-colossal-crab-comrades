import * as React from 'react';
import "./MainMenu.css";


import MainBackground from "../assets/mainMenu/mainBackground.jpg";
import BGVideo from "../assets/mainMenu/BGVideo.mp4";

export default class MainMenu extends React.Component {
    state = {
        test: 0
    };

    render() {
        return (
            <body>
                <section className="showcase">
                    <div className="video-container">                        
                        <video src={BGVideo} loop autoPlay muted />
                    </div>


                    <div className="content">
                        <h1>Pre Alpha CSE 442 Project</h1>
                        <h3>Founders (A-Z): Eoghan Mccarroll, Jacob Santoni, Joshua Lacey, Kyle Pellechia, Zachary Wagner </h3>
                        <a href="#about" className="btn">Log In / Create Account</a>
                        <a href="#about" className="btn">Settings</a>
                    </div>
                </section>

            
                <section id="about">
                    <h1>About</h1>
                    <p>Yee yee</p>
                    <h2> haha yes yes -will probably get rid of this section, idk it looks cool tho.</h2>
                </section>
               


            </body>
        );
    }
}