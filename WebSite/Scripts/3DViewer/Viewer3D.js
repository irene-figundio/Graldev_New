function Viewer3DTranslation() {

};


var Viewer3D = function () {

    var _translation = null;

    var _render3DObject = function (canvasId, objectPath, objectFile) {
        var canvas = document.getElementById(canvasId);

        var engine = null;
        var scene = null;
        var sceneToRender = null;
        var createDefaultEngine = function () { return new BABYLON.Engine(canvas, true, { preserveDrawingBuffer: false, stencil: true }); };
        var createDefaultScene = function () {
            // Setup the scene
            var scene = new BABYLON.Scene(engine);
            var camera = new BABYLON.ArcRotateCamera(
                "camera1",
                -(Math.PI / 2),
                Math.PI / 2,
                75,
                new BABYLON.Vector3(0, 0, 0),
                scene
            );
            var light = new BABYLON.HemisphericLight("light1", new BABYLON.Vector3(0, 1, 0), scene);
            scene.clearColor = new BABYLON.Color4(0, 0, 0, 0.0000000000000001);
            return scene;
        };
        var createScene = function (scene) {
            BABYLON.SceneLoader.ImportMesh(
                undefined,
                objectPath,
                objectFile,
                scene,
                function (
                    meshes,
                    particleSystems,
                    skeletons,
                    animationList
                ) {
                    scene.createDefaultCameraOrLight(true);

                    let asset = meshes[0];
                    scene.registerBeforeRender(function () {
                    asset.rotate(BABYLON.Axis.Y, Math.PI / 140, BABYLON.Space.LOCAL);
                    });
                }
            );

            return scene;
        };

        engine = createDefaultEngine();
        if (!engine) throw 'engine should not be null.';
        scene = createDefaultScene();
        scene = createScene(scene);
        sceneToRender = scene

        // Start rendering the scene based on the engine render loop.
        engine.runRenderLoop(function () {
            if (sceneToRender) {
                sceneToRender.render();
            }
        });

        // Resize
        window.addEventListener("resize", function () {
            engine.resize();
        });
    };

    return {
        init: function (canvasId, objectPath, objectFile) {
            _render3DObject(canvasId, objectPath, objectFile);
        }
    };

}();