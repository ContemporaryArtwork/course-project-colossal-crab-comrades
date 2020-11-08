import * as React from 'react';
import * as Phaser from 'phaser';
import "./GameStartRenderer.css";
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../../store';
import * as GameDataStore from "../../store/GameData";
import Game from '../../Phaser/Game';



type GameDataProps =
    GameDataStore.GameDataState &
    typeof GameDataStore.actionCreators &
    RouteComponentProps<{}>;


//Phaser Variables
var game: Phaser.Game;

class GameStartRenderer2 extends React.PureComponent<GameDataProps> {

    constructor(props: any) {
        super(props);
    }
    async componentDidMount() {

        var x = this.props;
        await this.props.initialize();
        setTimeout(() => { this.props.tempLogin("admin1", "passworD1$"); }, 1000);

        game = new Game(this);


    }

    public render() {
        return (

            <div id="gameCanvas" />);
    }
};

export default connect(
    (state: ApplicationState) => state.gameData,
    GameDataStore.actionCreators
)(GameStartRenderer2 as any);