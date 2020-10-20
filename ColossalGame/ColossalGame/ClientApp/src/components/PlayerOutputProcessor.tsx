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
            this.props.inGame ? <GameStartRenderer /> : <MainMenu />
        );
    }
};


export default connect(
    (state: ApplicationState) => state.PlayerOutputProcessor,
    PlayerOutputProcessorStore.actionCreators
)(PlayerOutputProcessor as any);