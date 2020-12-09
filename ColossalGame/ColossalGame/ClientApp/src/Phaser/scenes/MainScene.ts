import * as Phaser from 'phaser'
import { RouteComponentProps } from 'react-router';
import Game from '../Game'
import * as GameDataStore from "../../store/GameData";



//Test Big Bug
//import testBugJson from "../../assets/gameAssets/animation/TentacleMothSheet.json";
import testBug from "../../assets/gameAssets/animation/Spritesheet.png";
import { Console } from 'console';
 


type GameDataProps = //Rather than defining her, perhaps grab straight from the GameStartRenderer2 page.
    GameDataStore.GameDataState &
    typeof GameDataStore.actionCreators &
    RouteComponentProps<{}>;

var cursors: Phaser.Types.Input.Keyboard.CursorKeys;

export default class MainScene extends Phaser.Scene {
    private _gameObj: Game;
    private _hostingComponent: React.PureComponent<GameDataProps>;

    private _playerNameToContainerMap: Map<string, Phaser.GameObjects.Container>;


    constructor(config: string | Phaser.Types.Scenes.SettingsConfig, gameObj:Game) {
        super(config)
        this._gameObj = gameObj;
        this._hostingComponent = this._gameObj.react as React.PureComponent<GameDataProps>; //hostingComponent allows us to access the store of the GameStartRender2 page.

        this._playerNameToContainerMap = new Map<string, Phaser.GameObjects.Container>();
    }

    preload() {
        this.load.image('ground', require("../../assets/gameAssets/testingGround2.png").default);
        this.load.image('playerThing', require("../../components/gameComponents/testBuilder.png").default);
        this.load.image('spawnPad', require("../../assets/gameAssets/SpawnPlatform.png").default);

        //this.load.atlas("tentacleBug", require(testBug).default ,  require(testBugJson).default);
        this.load.spritesheet("testBug", testBug, { frameWidth: 400, frameHeight: 400 });
        this.load.spritesheet("player", require("../../assets/gameAssets/animation/player/HeavySpriteSheet.png").default, { frameWidth: 300, frameHeight: 300 });

    }

    create() {

        
        

        cursors = this.input.keyboard.createCursorKeys();
        this.add.image(0, 0, "ground");
        this.add.image(1024, 0, "ground");
        this.add.image(1024, 1024, "ground");
        this.add.image(0, 1024, "ground");
        this.add.image(-1024, 0, "ground");
        this.add.image(-1024, 1024, "ground");
        this.add.image(0, -1024, "ground");
        this.add.image(-1024, -1024, "ground");
        this.add.image(1024, -1024, "ground");
        var pad = this.add.image(0, 0, "spawnPad");
        pad.setScale(.75);
        
        this.add.text(
            500,
            550,
            "*Test Grounds YESyesyes*", {
            font: "40px Arial",
            fill: "#001DFF"
        }
        );

        let currGameState = this._hostingComponent.props.currentGameState;

        let dict: Map<string, GameDataStore.PlayerModel> = currGameState.playerDict;

        //Populate the scene with the players in the game at this moment in time. If in a future message we receive new players, they will be added to the scene in the update method.
        dict.forEach((value: GameDataStore.PlayerModel, key: string) => {

            let curPlayerContainer = this.add.container(value.xPos, value.yPos); //Player sprite and text is contained in a container. 
            //This allows for the player sprite and text to move together. Only need to update postion of the container.

            //curPlayerContainer.x = value.xPos;
            //curPlayerContainer.y = value.yPos;

            var curPlayer: any;  //This is of type any since this is how Josh had it. collideWorldBounds and enableBody are not part of Phaser.Physics.Arcade.Sprite, maybe thats why.
            //In future we should probably figure out how to set collideWorldBounds and collideBody.
            curPlayer = this.physics.add.sprite(0, 0, "player");
            curPlayer.collideWorldBounds = true;
            curPlayer.enableBody = true;

            curPlayer.setName("playerSprite"); //Set name of entities in container. Perhaps this will be useful later for changing image of sprite.
            curPlayer.setScale(.75);

            let curText = this.add.text(0, 0 - (curPlayer.height / 4), key, { font: "16px Arial", fill: "#ffffff", backgroundColor: "#808080", align: "center"});
            curText.x = curText.x - (curText.width / 2);
            curText.y = curText.y - 20;

            curText = curText.setName("playerText");
            curPlayerContainer.add(curPlayer);
            curPlayerContainer.add(curText);

            if (this._hostingComponent.props.playerData.username == key) {
                //Found player in the create method. This means player spawned by time create was called. Attach camera to this container so that the camera follows the player.
                this.cameras.main.startFollow(curPlayerContainer);
            }


            this._playerNameToContainerMap.set(key, curPlayerContainer); //Add player's container to a dictionary of usernames-> player containers.


            //Player SPritesheet
            this.anims.create({

                key: 'playerWalk',
                repeat: -1,
                frames: this.anims.generateFrameNames('player', { start: 1, end: 30 })

            });
            this.anims.create({

                key: 'playerIdle',
                repeat: -1,
                frames: this.anims.generateFrameNames('player', { start: 1, end: 1 })

            });
            curPlayer.setScale(.75);


        });

        //For testing bugs!
        var testingBug: any;
        testingBug = this.add.sprite(10, -500, "testBug", 0);
        this.anims.create({

            key: 'fly',
            repeat: -1,
            frames: this.anims.generateFrameNames('testBug', { start: 1, end: 23 })

        });
        testingBug.play('fly');
        //testingBug.setScale(2);
        //For testing bugs!





    }

    update() {


        //SEND Keyboard Presses To Server
        const up = cursors.up && cursors.up.isDown;
        const left = cursors.left && cursors.left.isDown;
        const down = cursors.down && cursors.down.isDown;
        const right = cursors.right && cursors.right.isDown;
        if (up && left) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.UpLeft); }
        else if (up && right) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.UpRight); }
        else if (down && left) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.DownLeft); }
        else if (down && right) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.DownRight); }
        else if (up) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.Up); }
        else if (left) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.Left); }
        else if (down) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.Down); }
        else if (right) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.Right); }
        



        //Update Position of all entities in the game using the current data in playerDict
        var gameState = this._hostingComponent.props.currentGameState;

        if (gameState && gameState.playerDict.size > 0) {

            //Update screen, with new data sent from server. The data from the server is contained in gameState. 


            //Loop through the player dict, if the player has already been added to the screen, update their coords, else add new sprite onto the screen for that player.
            var dict: Map<string, GameDataStore.PlayerModel> = gameState.playerDict;

            dict.forEach((value: GameDataStore.PlayerModel, key: string) => {

                if (!this._playerNameToContainerMap.has(key)) {
                    //This is a new player that is not currently in the scene
                    //We must create the phaser container for this player

                    let curPlayerContainer = this.add.container(value.xPos, value.yPos);

                    var curPlayer: any;
                    curPlayer = this.physics.add.sprite(0, 0, "player");
                    curPlayer.collideWorldBounds = true;
                    curPlayer.enableBody = true;





                    curPlayer.setName("playerSprite");
                    curPlayer.setScale(.75);
                    let curText = this.add.text(0, 0 - (curPlayer.height / 4), key, { font: "16px Arial", fill: "#ffffff", backgroundColor: "#808080", align: "center" });
                    curText.x = curText.x - (curText.width / 2);
                    curText.y = curText.y - 20;

                    curText = curText.setName("playerText");
                    
                    curPlayerContainer.add(curPlayer);
                    curPlayerContainer.add(curText);

                    if (this._hostingComponent.props.playerData.username == key) {
                        //Found player in the update loop. Attach camera to this container so that the camera follows the player.
                        this.cameras.main.startFollow(curPlayerContainer);
                    }

                    this._playerNameToContainerMap.set(key, curPlayerContainer); //Add player's container to a dictionary of usernames-> player containers.
                }
                else {
                    //Player exists in the scene, so update their coordinates with the new ones.
                    let playerContainer = this._playerNameToContainerMap.get(key)

                    if (playerContainer) {
                        var xpos = value.xPos;
                        var ypos = value.yPos;

                        var p = <Phaser.Physics.Arcade.Sprite>playerContainer.getByName("playerSprite");
                       
                        
                        if (this._hostingComponent.props.playerData.username == key) {
                           
                            if (up || down || left || right) {
                                //console.log("MOVING");
                                //p.tint = 0xFF000C;

                            } else {
                                //console.log("NOT MOVING");
                                //p.tint = 0xffffff;
                                p.play('playerWalk');
                                //^^^ THIS MAKES NO SENSE. WHEN ITS NOT MOVING, PLAY THE ANIMATION...OKAY!
                            }
                        } else {

                            if (xpos - playerContainer.x != 0 && ypos - playerContainer.y) {
                                p.play('playerWalk');
                            } else {
                                
                            }

                            /*
                            if (xpos - playerContainer.x > 0) {
                                //Moving right
                                p.tint = 0xFF000C;
                                p.play('playerWalk');

                            } else if (xpos - playerContainer.x < 0) {
                                p.tint = 0xFF000C;
                                p.play('playerWalk');
                            }

                            if (ypos - playerContainer.y > 0) {
                                p.tint = 0xFF000C;
                                p.play('playerWalk');
                            } else if (ypos - playerContainer.y < 0) {
                                p.tint = 0xFF000C;
                                p.play('playerWalk');
                            }
                            */

                        }

                        xpos = Phaser.Math.Interpolation.Bezier([xpos, playerContainer.x], .8);
                        ypos = Phaser.Math.Interpolation.Bezier([ypos, playerContainer.y], .8);
                        playerContainer.x = xpos;
                        playerContainer.y = ypos;
                    }
                }


            });
        }
    }
}