import * as React from 'react';
import * as Phaser from 'phaser';
import "./GameStartRenderer.css";
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../../store';
import * as GameDataStore from "../../store/GameData";



type GameDataProps =
    GameDataStore.GameDataState &
    typeof GameDataStore.actionCreators &
    RouteComponentProps<{}>;


//Phaser Variables
var game: Phaser.Game;
var cursors: any;
var player: any;

class GameStartRenderer extends React.PureComponent<GameDataProps> {

    constructor(props: any) {
        super(props);
        this.update = this.update.bind(this);
    }
    async componentDidMount () {

        var x = this.props;
        await this.props.initialize();
        setTimeout(() => { this.props.tempLogin("admin1", "password"); }, 1000);



        game = new Phaser.Game({
            type: Phaser.AUTO,
            width: window.innerWidth,
            height: window.innerHeight,
            physics: {
                default: "arcade",
                arcade: {
                    gravity: { y: 0 },
                    debug: false,
                },
            },
            parent: "gameCanvas",
            scene: {
                preload: this.preload,
                create: this.create,
                update: this.update,
            },
        });
        //var controls = null;
    }

    preload(this: Phaser.Scene) {
       // this.load.image("playerThing", playerThing);

        this.load.image('ground', require("./testGround.jpg").default);
        this.load.image('playerThing', require("./testBuilder.png").default);
        
    }

    create(this: Phaser.Scene) {

        cursors = this.input.keyboard.createCursorKeys();
        this.add.image(100, 300, "ground");
        player = this.physics.add.sprite(100, 450, "playerThing");
        
        this.add.text(
            500,
            550,
            "*Test Grounds*", {
            font: "40px Arial",
                fill: "#001DFF"
        }
        );
    }

    update() {
        
        if (cursors.up.isDown) { this.props.sendMovementAction(GameDataStore.Direction.Up);}
        if (cursors.left.isDown) { this.props.sendMovementAction(GameDataStore.Direction.Left); }
        if (cursors.down.isDown) { this.props.sendMovementAction(GameDataStore.Direction.Down); }
        if (cursors.right.isDown) { this.props.sendMovementAction(GameDataStore.Direction.Right); }

    /*
    //0 = Up Move
    //1 = Left Move
    //2 = Back Move
    //3 = Right Move
    

        var actions = new Array(4);

        actions[0] = cursors.up.isDown ? 1 : 0;
        actions[1] = cursors.left.isDown ? 1 : 0;
        actions[2] = cursors.down.isDown ? 1 : 0;
        actions[3] = cursors.right.isDown ? 1 : 0;

        if (actions[0] == 1) {
            console.log("Up");
        }
        if (actions[1] == 1) {
            console.log("Left");
        }
        if (actions[2] == 1) {
            console.log("Back");
        }
        if (actions[3] == 1) {
            console.log("Right");
        }
        */
        //THIS IS JUST FOR TESTING
        player.setVelocityX(
            (cursors.left.isDown ? -160 : 0) + (cursors.right.isDown ? 160 : 0)
        );
        player.setVelocityY(
            (cursors.up.isDown ? -160 : 0) + (cursors.down.isDown ? 160 : 0)
        );
        //THIS IS JUST FOR TESTING
    }



    public render() {
        return (
     
            <div id="gameCanvas" />);
    }  
};

export default connect(
    (state: ApplicationState) => state.gameData,
    GameDataStore.actionCreators
)(GameStartRenderer as any);