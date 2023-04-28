// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import { dotnet } from './dotnet.js'

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

setModuleImports('main.js', {
    window: {
        location: {
            href: () => globalThis.window.location.href
        }
    }
});

await dotnet.run();

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

if (false) {
    const canvas = document.querySelector('#myCanvas');

    const gl = canvas.getContext('webgl');

    if (gl !== null) {
	gl.clearColor(0.0, 0.0, 0.9, 1.0);
	gl.clear(gl.COLOR_BUFFER_BIT);
    }
} else {
    exports.MyClass.Draw('#myCanvas');
}

const text = exports.MyClass.Greeting();
console.log(text);

document.getElementById('out').innerHTML = text;

