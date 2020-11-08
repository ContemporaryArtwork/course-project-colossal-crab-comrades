import * as Phaser from 'phaser'
import { RouteComponentProps } from 'react-router';
import Game from '../Game'
import * as GameDataStore from "../../store/GameData";


type GameDataProps =
    GameDataStore.GameDataState &
    typeof GameDataStore.actionCreators &
    RouteComponentProps<{}>;

var cursors: Phaser.Types.Input.Keyboard.CursorKeys;
var player: any;
var text: Phaser.GameObjects.Text;
var container: Phaser.GameObjects.Container;

export default class MainScene extends Phaser.Scene {
    private _gameObj: Game;
    private _hostingComponent: React.PureComponent<GameDataProps>;




    constructor(config: string | Phaser.Types.Scenes.SettingsConfig, gameObj:Game) {
        super(config)
        this._gameObj = gameObj;
        this._hostingComponent = this._gameObj.react as React.PureComponent<GameDataProps>;
    }

    preload() {
        this.load.image('ground', require("../../components/gameComponents/testGround.jpg").default);
        this.load.image('playerThing', require("../../components/gameComponents/testBuilder.png").default);
    }

    create() {
        cursors = this.input.keyboard.createCursorKeys();
        this.add.image(100, 300, "ground");

        container = this.add.container(100, 450);

        player = this.physics.add.sprite(0, 0, "playerThing");
        player.collideWorldBounds = true;
        player.enableBody = true;

        text = this.add.text(0, 0 - (player.height / 2), "admin1", { font: "16px Arial", fill: "#ffffff", backgroundColor: "#ffff00", align: "center" });
        text.x = text.x - (text.width / 2);
        text.y = text.y - 20;
        container.add(player);
        container.add(text);

        //this.add.image(54, 0, "playerThing");

        this.cameras.main.startFollow(container);

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

        if (cursors.up && cursors.up.isDown) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.Up); }
        if (cursors.left && cursors.left.isDown) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.Left); }
        if (cursors.down && cursors.down.isDown) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.Down); }
        if (cursors.right && cursors.right.isDown) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.Right); }
        //console.log(this.props.currentGameState);
        var gameState = this._hostingComponent.props.currentGameState;
        if (gameState == undefined) {

        } else if (gameState.playerDict.size > 0) {



            var dict: Map<string, GameDataStore.PlayerModel> = gameState.playerDict;
            //console.log(dict);

            //console.log(dict);

            if (dict.has("admin1")) {
                var admin: GameDataStore.PlayerModel | undefined = dict.get("admin1");
                if (admin !== undefined) {
                    //player.setPosition(admin.XPos, admin.YPos);
                    //console.log(admin);
                    //console.log(admin);
                    var xpos = admin.xPos;
                    var ypos = admin.yPos;
                    xpos = Phaser.Math.Interpolation.Bezier([xpos, container.x], .8);
                    ypos = Phaser.Math.Interpolation.Bezier([ypos, container.y], .8);
                    //console.log(xpos);
                    container.x = xpos;
                    container.y = ypos;

                }
            }



        }
    }
}