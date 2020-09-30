import * as React from 'react';
import MainMenu from './MainMenu';


export default class View extends React.Component {
    state = {
        inGame: false
    };


    render() {
        if (this.state.inGame) {
            return (<div>
                hi
                </div>);
        } else {
            return (<MainMenu/>);
        }
    }
}