import * as React from 'react';
import MainMenu from './MainMenu';


export default class View extends React.Component {
    state = {
        inGame: true
    };


    render() {
        if (this.state.inGame) {
            return (<div>


                <canvas id="myCanvas" width="1000" height="1000" style={{ border: '10px solid #FF0800' }}>
                    Your browser does not support the HTML canvas tag.</canvas>


                <script>
                    var c = document.getElementById("myCanvas");
                    var ctx = c.getContext("2d");
                    ctx.moveTo(0,0);
                    ctx.lineTo(200,100);
                    ctx.stroke();
                </script>



                </div>);
        } else {
            return (<MainMenu/>);
        }
    }
}