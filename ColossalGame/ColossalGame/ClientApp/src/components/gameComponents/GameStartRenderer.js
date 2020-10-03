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
var GameMainMenuToggler = /** @class */ (function (_super) {
    __extends(GameMainMenuToggler, _super);
    function GameMainMenuToggler() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    GameMainMenuToggler.prototype.componentDidMount = function () {
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
    };
    GameMainMenuToggler.prototype.preload = function () {
        //Test Image
        this.load.image("testSoldierGuy", "../../assets/gameAssets/testSoldierGuy.png");
    };
    GameMainMenuToggler.prototype.create = function () {
        this.add.image(400, 100, "testSoldierGuy");
        this.add.image(100, 100, "testSoldierGuy");
    };
    GameMainMenuToggler.prototype.update = function () {
    };
    GameMainMenuToggler.prototype.render = function () {
        return (React.createElement("div", { id: "gameCanvas" }));
    };
    return GameMainMenuToggler;
}(React.Component));
exports.default = GameMainMenuToggler;
;
//# sourceMappingURL=GameStartRenderer.js.map