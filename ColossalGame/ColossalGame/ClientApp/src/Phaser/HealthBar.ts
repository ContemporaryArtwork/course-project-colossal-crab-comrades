import * as Phaser from 'phaser'

//Health bar code heavily based off the official phaser 3 example 
//https://phaser.io/examples/v3/view/game-objects/graphics/health-bars-demo

export default class HealthBar extends Phaser.GameObjects.Graphics{
    //bar: Phaser.GameObjects.Graphics;
    x: number;
    y: number;
    value: number;
    p: number;
    width: number;

    constructor(scene: Phaser.Scene, x:number, y:number, width:number) {
        super(scene);
        //this.bar = new Phaser.GameObjects.Graphics(scene);

        this.x = x-(width/4);
        this.y = y;
        this.value = 100;
        this.p = 76 / 100;

        this.width = width;

        this.draw();

        //scene.add.existing(this.bar);
    }

    setHealthBar(amount:number) {

        if (this.value < 0) {
            this.value = 0;
        }
        else {
            this.value = amount;
        }

        this.draw();

        return (this.value === 0);
        
    }
 
    draw() {
        this.clear();

        //  BG
        this.fillStyle(0x000000);
        this.fillRect(this.x, this.y, this.width, 16-4); // was 80

        //  Health

        this.fillStyle(0xffffff);
        this.fillRect(this.x + 2, this.y + 2, this.width-4, 12-4);

        if (this.value < 30) {
            this.fillStyle(0xff0000);
        }
        else {
            this.fillStyle(0x00ff00);
        }

        var d = Math.floor(this.p * this.value);

        this.fillRect(this.x + 2, this.y + 2, d, 12-4);
    }

}