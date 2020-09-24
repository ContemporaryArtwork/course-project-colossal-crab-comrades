/// <reference types="react-scripts" />


//Declared mp4 module so that ts recognises mp4 videos (Josh)
declare module '*.mp4' {
    const src: string;
    export default src;
}