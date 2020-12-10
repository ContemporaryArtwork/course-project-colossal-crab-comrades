import * as Phaser from 'phaser'
import MainScene from './scenes/MainScene'

class Game extends Phaser.Game {
    public react: React.PureComponent;
    constructor(react: React.PureComponent) {
        const config = { //Configuration for the Phaser.Game component.
            type: Phaser.AUTO,
            scale: {
                mode: Phaser.Scale.RESIZE,
                //autoCenter: Phaser.Scale.CENTER_BOTH,
                width: '100%',
                height: '100%'
            },
            physics: {
                default: "arcade",
                arcade: {
                    gravity: { y: 0 },
                    debug: false,
                },
            },
            parent: "gameCanvas", // parent is the div which the game will be going into.
            
        }
        super(config) //This calls the Phaser.Game constructor.
        this.react = react; //This allows us to take the 'this' from the GameStartRenderer2 and access it within the Game.

        const conf: Phaser.Types.Scenes.SettingsConfig = { key: 'MainScene' };
        const s = new MainScene(conf, this); //Create our own custom Scene, passing into the scene 'this' which allows us to access the properties of the game within the scene.
        this.scene.add(conf.key as string,s,true);

    }
}

export default Game