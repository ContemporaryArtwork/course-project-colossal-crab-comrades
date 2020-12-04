import * as Phaser from 'phaser'
import { RouteComponentProps } from 'react-router';
import Game from '../Game'
import * as GameDataStore from "../../store/GameData";



//Test Big Bug
//import testBugJson from "../../assets/gameAssets/animation/TentacleMothSheet.json";
import testBug from "../../assets/gameAssets/animation/Spritesheet.png";
import { Console } from 'console';
import { PlayerExportModel, BulletExportModel } from '../../store/GameData';
 


type GameDataProps = //Rather than defining her, perhaps grab straight from the GameStartRenderer2 page.
    GameDataStore.GameDataState &
    typeof GameDataStore.actionCreators &
    RouteComponentProps<{}>;

var cursors: Phaser.Types.Input.Keyboard.CursorKeys;
var spacebar: Phaser.Input.Keyboard.Key;

export default class MainScene extends Phaser.Scene {
    private _gameObj: Game;
    private _hostingComponent: React.PureComponent<GameDataProps>;

    private _playerNameToContainerMap: Map<string, Phaser.GameObjects.Container>;

    private _gameObjectsOnScreen: Map<number, Phaser.GameObjects.Container>;


    constructor(config: string | Phaser.Types.Scenes.SettingsConfig, gameObj:Game) {
        super(config)
        this._gameObj = gameObj;
        this._hostingComponent = this._gameObj.react as React.PureComponent<GameDataProps>; //hostingComponent allows us to access the store of the GameStartRender2 page.

        this._playerNameToContainerMap = new Map<string, Phaser.GameObjects.Container>();
        this._gameObjectsOnScreen = new Map<number, Phaser.GameObjects.Container>();
    }

    preload() {
        this.load.image('ground', require("../../assets/gameAssets/testingGround2.png").default);
        this.load.image('playerThing', require("../../components/gameComponents/testBuilder.png").default);
        this.load.image('spawnPad', require("../../assets/gameAssets/SpawnPlatform.png").default);
        this.load.image('jesse', require("../../assets/gameAssets/Jesse_Hartloff.jpg").default);

        //this.load.atlas("tentacleBug", require(testBug).default ,  require(testBugJson).default);
        this.load.spritesheet("testBug", testBug, { frameWidth: 400, frameHeight: 400 });
        this.load.spritesheet("player", require("../../assets/gameAssets/animation/player/ForwardWalkSpritesheet.png").default, { frameWidth: 300, frameHeight: 300 });

    }

    create() {

        
        

        cursors = this.input.keyboard.createCursorKeys();
        spacebar = this.input.keyboard.addKey('SPACE');
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
        pad.setScale(.5);
        
        this.add.text(
            500,
            550,
            "*Test Grounds YESyesyes*", {
            font: "40px Arial",
            fill: "#001DFF"
        }
        );

        let currGameState = this._hostingComponent.props.currentGameState;

        let dict: Map<string, GameDataStore.PlayerExportModel> = currGameState.playerDict;

        //Populate the scene with the players in the game at this moment in time. If in a future message we receive new players, they will be added to the scene in the update method.
        dict.forEach((value: GameDataStore.PlayerExportModel, key: string) => {

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
            curPlayer.setScale(.5);

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
            curPlayer.setScale(.5);


        });

        let gameobjList = currGameState.objectList;

        gameobjList.forEach((value) => {

            if (isProjectile(value)) {

                var projectileObj = value as BulletExportModel;

                let bulletContainer = this.add.container(projectileObj.xPos, projectileObj.yPos);

                var bulletSprite: any;  //This is of type any since this is how Josh had it. collideWorldBounds and enableBody are not part of Phaser.Physics.Arcade.Sprite, maybe thats why.
                //In future we should probably figure out how to set collideWorldBounds and collideBody.
                bulletSprite = this.add.sprite(0, 0, "jesse");
                

                bulletSprite.setName("bulletSprite");
                bulletSprite.setScale(.1);
                bulletContainer.add(bulletSprite);

                this._gameObjectsOnScreen.set(projectileObj.id, bulletContainer);
            }

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
        const spacebarPressed = spacebar.isDown;
        
        if (up && left) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.UpLeft); }
        else if (up && right) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.UpRight); }
        else if (down && left) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.DownLeft); }
        else if (down && right) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.DownRight); }
        else if (up) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.Up); }
        else if (left) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.Left); }
        else if (down) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.Down); }
        else if (right) { this._hostingComponent.props.sendMovementAction(GameDataStore.Direction.Right); }
        else if (spacebarPressed) { this._hostingComponent.props.fireWeaponAction(90 * (Math.PI / 180)); }
        
        


        //Update Position of all entities in the game using the current data in playerDict
        var gameState = this._hostingComponent.props.currentGameState;

        if (gameState && gameState.playerDict.size > 0) {

            //Update screen, with new data sent from server. The data from the server is contained in gameState. 


            //Loop through the player dict, if the player has already been added to the screen, update their coords, else add new sprite onto the screen for that player.
            var dict: Map<string, GameDataStore.PlayerExportModel> = gameState.playerDict;

            dict.forEach((value: GameDataStore.PlayerExportModel, key: string) => {

                if (!this._playerNameToContainerMap.has(key)) {
                    //This is a new player that is not currently in the scene
                    //We must create the phaser container for this player

                    let curPlayerContainer = this.add.container(value.xPos, value.yPos);

                    var curPlayer: any;
                    curPlayer = this.physics.add.sprite(0, 0, "player");
                    curPlayer.collideWorldBounds = true;
                    curPlayer.enableBody = true;





                    curPlayer.setName("playerSprite");
                    curPlayer.setScale(.5);
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
                        var xPos = value.xPos;
                        var yPos = value.yPos;

                        var p = <Phaser.Physics.Arcade.Sprite>playerContainer.getByName("playerSprite");
                       
                        
                        if (this._hostingComponent.props.playerData.username == key) {
                           
                            if (up || down || left || right) {
                                //console.log("MOVING");
                                //p.tint = 0xFF000C;

                            } else {
                                //console.log("NOT MOVING");
                                //p.tint = 0xffffff;
                                //p.play('playerWalk');
                                //^^^ THIS MAKES NO SENSE. WHEN ITS NOT MOVING, PLAY THE ANIMATION...OKAY!
                            }
                        } else {

                            if (xPos - playerContainer.x != 0 && yPos - playerContainer.y) {
                                //p.play('playerWalk');
                            } else {
                                
                            }

                            /*
                            if (xPos - playerContainer.x > 0) {
                                //Moving right
                                p.tint = 0xFF000C;
                                p.play('playerWalk');

                            } else if (xPos - playerContainer.x < 0) {
                                p.tint = 0xFF000C;
                                p.play('playerWalk');
                            }

                            if (yPos - playerContainer.y > 0) {
                                p.tint = 0xFF000C;
                                p.play('playerWalk');
                            } else if (yPos - playerContainer.y < 0) {
                                p.tint = 0xFF000C;
                                p.play('playerWalk');
                            }
                            */

                        }

                        xPos = Phaser.Math.Interpolation.Bezier([xPos, playerContainer.x], .8);
                        yPos = Phaser.Math.Interpolation.Bezier([yPos, playerContainer.y], .8);
                        playerContainer.x = xPos;
                        playerContainer.y = yPos;
                    }
                }


            });

            let gameobjList = gameState.objectList;

            gameobjList.forEach((value) => {

                if (isProjectile(value)) {

                    

                    var projectileObj = value as BulletExportModel;


                    if (!this._gameObjectsOnScreen.has(projectileObj.id)) {
                        //Does not have this ID rendered yet

                        let bulletContainer = this.add.container(projectileObj.xPos, projectileObj.yPos);

                        var bulletSprite: any;  //This is of type any since this is how Josh had it. collideWorldBounds and enableBody are not part of Phaser.Physics.Arcade.Sprite, maybe thats why.
                        //In future we should probably figure out how to set collideWorldBounds and collideBody.
                        bulletSprite = this.add.sprite(0, 0, "jesse");

                        bulletSprite.setName("bulletSprite");
                        bulletSprite.setScale(.1);
                        bulletContainer.add(bulletSprite);

                        this._gameObjectsOnScreen.set(projectileObj.id, bulletContainer);
                    }
                    else {

                        let bulletContainer =  this._gameObjectsOnScreen.get(projectileObj.id);

                        if (bulletContainer) {
                            var xPos = projectileObj.xPos;
                            var yPos = projectileObj.yPos;

                            xPos = Phaser.Math.Interpolation.Bezier([xPos, bulletContainer.x], .8);
                            yPos = Phaser.Math.Interpolation.Bezier([yPos, bulletContainer.y], .8);
                            bulletContainer.x = xPos;
                            bulletContainer.y = yPos;
                        }
                        else {
                            console.log("its null in update. have id but no conainter");
                        }

                        
                    }



                }

            });

            //Cleanup loop
            var ids: number[] = [];
            this._gameObjectsOnScreen.forEach((value: Phaser.GameObjects.Container, key: number) => {
                let item = gameobjList.find(i => isProjectile(i) && i.id === key);
                if (item == undefined) {
                    value.destroy();
                    ids.push(key);
                }
            });
            ids.forEach((key: number) => {
                this._gameObjectsOnScreen.delete(key);
            });
        }

        /*this._gameObjectsOnScreen.forEach((value, key) => {
            
            console.log("Key:"+key+ " Pos:"+value.x +","+ value.y);
        });*/


    }
}

function isProjectile(gameObj: PlayerExportModel | BulletExportModel): gameObj is BulletExportModel {
    return (gameObj as BulletExportModel).bulletType !== undefined;
}