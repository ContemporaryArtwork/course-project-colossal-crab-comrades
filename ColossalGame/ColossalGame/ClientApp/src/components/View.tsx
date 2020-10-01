import * as React from 'react';
import MainMenu from './MainMenu';


export interface ViewState {

    isLoading: boolean;
    inGame: boolean;

}

export default class View extends React.Component {



    render() {
        var i = 0;
        if (i != 0) {
            return (<div>
                hi
                </div>);
        } else {
            return (<MainMenu/>);
        }
    }
}