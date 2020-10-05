"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var Phaser = require("phaser");
require("./GameStartRenderer.css");
//Phaser Variables
var game;
var cursors;
var player;
var GameMainMenuToggler = /** @class */ (function (_super) {
    __extends(GameMainMenuToggler, _super);
    function GameMainMenuToggler() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    GameMainMenuToggler.prototype.componentDidMount = function () {
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
    };
    GameMainMenuToggler.prototype.preload = function () {
        this.load.image("player", './gameComponents/testBuilder.png');
    };
    GameMainMenuToggler.prototype.create = function () {
        cursors = this.input.keyboard.createCursorKeys();
        player = this.physics.add.sprite(100, 450, "player");
        this.add.text(100, 500, "Well text certainly works...", {
            font: "40px Arial",
            fill: "#ffffff"
        });
        this.add.text(500, 520, "YEAHHHHH", {
            font: "40px Arial",
            fill: "#ffffff"
        });
    };
    GameMainMenuToggler.prototype.update = function () {
        //THIS IS JUST FOR TESTING
        player.setVelocityX((cursors.left.isDown ? -160 : 0) + (cursors.right.isDown ? 160 : 0));
        player.setVelocityY((cursors.up.isDown ? -160 : 0) + (cursors.down.isDown ? 160 : 0));
        //THIS IS JUST FOR TESTING
    };
    GameMainMenuToggler.prototype.render = function () {
        return (React.createElement("div", { id: "gameCanvas" }));
    };
    return GameMainMenuToggler;
}(React.Component));
exports.default = GameMainMenuToggler;
;
//# sourceMappingURL=GameStartRenderer.js.map