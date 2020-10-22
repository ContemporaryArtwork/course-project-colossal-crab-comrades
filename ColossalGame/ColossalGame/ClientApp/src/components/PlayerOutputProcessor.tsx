import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as Phaser from 'phaser';
import * as PlayerOutputProcessorStore from "../store/PlayerOutputProcessor";


type PlayerOutputProcessorProps =
    PlayerOutputProcessorStore.PlayerOutputProcessorState &
    typeof PlayerOutputProcessorStore.actionCreators &
    RouteComponentProps<{}>;


class PlayerOutputProcessor extends React.PureComponent<PlayerOutputProcessorProps> {

    
    render() {
        return (
            <div>hi</div>
        );
    }
   
};


export default connect(
    (state: ApplicationState) => state.playerOutputProcessor,
    PlayerOutputProcessorStore.actionCreators
)(PlayerOutputProcessor as any);