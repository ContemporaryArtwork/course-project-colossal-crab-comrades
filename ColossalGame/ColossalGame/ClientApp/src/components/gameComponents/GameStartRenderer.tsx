import * as React from 'react';
import * as Phaser from 'phaser';
import "./GameStartRenderer.css";




export default class GameMainMenuToggler extends React.Component {

    componentDidMount () {

        var game = new Phaser.Game({
            type: Phaser.AUTO,
            width: 800,
            height: 600,
            parent: "gameCanvas",
            scene: {
                preload: this.preload,
                create: this.create,
                update: this.update,
            },
        });
        var controls = null;
    }

    preload(this: Phaser.Scene) {

        //Test Image
        this.load.image("testSoldierGuy", "../../assets/gameAssets/testSoldierGuy.png");
    }
    create(this: Phaser.Scene) {




        this.add.image(400, 100, "testSoldierGuy");
        this.add.image(100, 100, "testSoldierGuy");
    }
    update() {

    }

    public render() {
        return (<div id="gameCanvas" />);
    }  
};