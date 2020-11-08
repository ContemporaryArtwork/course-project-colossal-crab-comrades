import * as Phaser from 'phaser'
import MainScene from './scenes/MainScene'

class Game extends Phaser.Game {
    public react: React.PureComponent;
    constructor(react: React.PureComponent) {
        const config = {
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
            
        }
        super(config)
        this.react = react;

        const conf: Phaser.Types.Scenes.SettingsConfig = { key: 'MainScene' };
        const s = new MainScene(conf, this);
        this.scene.add(conf.key as string,s,true);

    }
}

export default Game