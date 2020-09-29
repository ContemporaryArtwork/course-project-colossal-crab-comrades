import * as React from 'react';
import MainMenu from './MainMenu';


export default class View extends React.Component {
    state = {
        inGame: true
    };


    render() {
        if (!this.state.inGame) {
            return (<div>yeeheeyee</div>);
        } else {
            return (<MainMenu/>);
        }
    }
}