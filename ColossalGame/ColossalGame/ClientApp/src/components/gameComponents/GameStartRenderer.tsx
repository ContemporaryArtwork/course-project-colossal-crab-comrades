import * as React from 'react';
import * as Phaser from 'phaser';
import "./GameStartRenderer.css";
//import playerThing from "./testBuilder.png";

//Phaser Variables
var game: Phaser.Game;
var cursors: any;
var player: any;

export default class GameMainMenuToggler extends React.Component {

    componentDidMount () {

        game = new Phaser.Game({
            type: Phaser.AUTO,
            width: 800,
            height: 600,
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
       
        this.load.image('playerThing', require("./testBuilder.png").default);
    }

    create(this: Phaser.Scene) {

        cursors = this.input.keyboard.createCursorKeys();
        player = this.physics.add.sprite(100, 450, "playerThing");

        this.add.text(
            100,
            500,
            "Well text certainly works...", {
            font: "40px Arial",
            fill: "#ffffff"
        }
        );
        this.add.text(
            500,
            520,
            "YEAHHHHH", {
            font: "40px Arial",
            fill: "#ffffff"
        }
        );
    }


    update() {

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
        return (<div id="gameCanvas" />);
    }  
};