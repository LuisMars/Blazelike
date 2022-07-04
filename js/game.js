const game = {
    instance: false
};
window.init = (dotNetHelper) => {
    console.log("Initializing");
    game.instance = dotNetHelper;
};
window.onkeydown = (e) => {
    if (!game.instance) {
        console.log("game.instance is false");
        return;
    }
    if (!e.repeat) {
        game.instance.invokeMethodAsync('InvokeMove', e.keyCode);
    }
};